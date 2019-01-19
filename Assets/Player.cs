using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TouchControlsKit;

[RequireComponent(typeof(BlockController))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public WallController wallController;

    [System.Serializable]
    public class PlayerShape
    {
        public WallBlock.WallType wallType;
        public GameObject shape;
    }

    public WallBlock.WallType currentType;
    private GameObject currentShape;
    public List<PlayerShape> playerShapes;
    private Dictionary<WallBlock.WallType, GameObject> playerObjectDictionary = new Dictionary<WallBlock.WallType, GameObject>();

    public ParticleSystem boostParticles;

    public int numberOfTypesToPredict = 3;
    public WallBlock.WallType[] nextTypeList;

    public bool invincible;
    public int maxMistakes;
    public int remainingMistakes;
    public int score;
    public int wallsHit;
    public int goalsHit;
    public float accuracy;

    public float moveSensitivity;
    public float minX, maxX, minY, maxY;
    public float minMagToRotate = 0.1f;

    public BlockController blockController;
    ScreenShake screenShake;
    
    Vector3 gyroVector;
    public float gyroSensitivity = 2;
    public float boostingSensitivity = 0.5f;
    public Vector3 gyroCalibration = Vector3.zero;
    public Vector3 gyroDeadzone = new Vector3(0.01f, 0.01f, 0.01f);
    public float rotateSpeed = 8;
    public bool invertRotation = false;
    public bool invertGyroX = false;
    public bool invertGyroY = false;
    private Gyroscope gyro;

    public bool doubleClickFreeze, doubleClickReset;
    private bool freezeGyro;

    public Transform playerObject;
    public bool compensateForPlayerRotation = false;

    private AudioSource audioSource;
    public LayerMask failureLayers;
    public LayerMask successLayers;

    [System.Serializable]
    public class AudioClipDetail
    {
        public AudioClip audioClip;
        public float minPitch = 1, maxPitch = 1, minVolume = 1, maxVolume = 1;

        public float GetVolume()
        {
            return Random.Range(minVolume, maxVolume);
        }

        public float GetPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }

        public void Play(AudioSource audioSource)
        {
            float volume = GetVolume();
            float pitch = GetPitch();
            if (volume > 0 && pitch > 0)
            {
                audioSource.volume = volume;
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            }
        }
    }
    public AudioClipDetail failureClipDetail, successClipDetail, rotateClipDetail, boostClipDetail;

    public float failureShakeDuration, failureShakeAmount;

    private MyGameManager myGameManager;
    private LevelManager levelManager;

    void ResetGyro()
    {
        gyroVector = Vector3.forward;
    }

    void Start()
    {
        myGameManager = MyGameManager.instance;
        levelManager = LevelManager.instance;
        blockController = GetComponent<BlockController>();
        screenShake = ScreenShake.instance;
        audioSource = GetComponent<AudioSource>();

        Debug.Log("Setting up gyro... " + SystemInfo.supportsGyroscope);
        Util.ResetGyro();
        ResetGyro();
        gyro = Input.gyro;
        UpdateGyroSensitivity();
        Debug.Log("Gyro set");

        InitializeShapes();

        remainingMistakes = maxMistakes;
    }

    private UnityAction settingsListener;
    void OnEnable()
    {
        settingsListener = new UnityAction(UpdateSettings);
        EventManager.StartListening(Options.SETTINGS_CHANGED, settingsListener);
    }
    void OnDisable()
    {
        EventManager.StopListening(Options.SETTINGS_CHANGED, settingsListener);
    }
    void UpdateSettings()
    {
        UpdateInversion();
        UpdateGyroSensitivity();
    }

    void UpdateInversion()
    {
        invertRotation = PlayerPrefs.GetInt(Options.INVERT_ROTATION) == 1;
        invertGyroX = PlayerPrefs.GetInt(Options.INVERT_GYRO_X) == 1;
        invertGyroY = PlayerPrefs.GetInt(Options.INVERT_GYRO_Y) == 1;
    }
    void UpdateGyroSensitivity()
    {
        gyroSensitivity = PlayerPrefs.GetFloat(Options.GYRO_SENSITIVITY);

        gyroCalibration.x = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'x');
        gyroCalibration.y = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'y');
        gyroCalibration.z = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'z');

        swipeSensitivity = PlayerPrefs.GetFloat(Options.ROTATE_SENSITIVITY);
        //swipeSensitivity = 25;
        //rotationSlowdownDeadzone = PlayerPrefs.GetFloat(Options.ROTATE_SENSITIVITY);

        if (invertRotation)
        {
            swipeSensitivity *= -1;
        }
    }

    public void AddMistake()
    {
        if (remainingMistakes < maxMistakes)
        {
            maxMistakes++;
        }
    }

    void InitializeShapes()
    {
        foreach (PlayerShape playerShape in playerShapes)
        {
            playerObjectDictionary.Add(playerShape.wallType, playerShape.shape);
            playerShape.shape.SetActive(false);
        }

        currentType = Util.RandomEnumValue<WallBlock.WallType>(WallBlock.WallType.NONE);

        playerObjectDictionary.TryGetValue(currentType, out currentShape);
        currentShape.SetActive(true);

        nextTypeList = new WallBlock.WallType[numberOfTypesToPredict];
        for (int i = 0; i < numberOfTypesToPredict; i++)
        {
            nextTypeList[i] = Util.RandomEnumValue<WallBlock.WallType>(WallBlock.WallType.NONE);
        }
    }
    void Update()
    {
        if (myGameManager.isPaused)
        {
            return;
        }

        //RotatePlayer();

        //SwipeMove();
        //RotateWorld();
        //GyroMove();
        //JoystickMove();
        
        GyroMove();
        JoystickMove();
        SwipeRotate();

        AdjustSpeed();
    }

    void SwitchShape()
    {
        WallBlock.WallType nextType = nextTypeList[0];
        for(int i=0; i < nextTypeList.Length - 1; i++)
        {
            nextTypeList[i] = nextTypeList[i + 1];
        }
        nextTypeList[nextTypeList.Length-1] = Util.RandomEnumValue<WallBlock.WallType>(WallBlock.WallType.NONE);

        if (currentType != nextType)
        {
            currentShape.SetActive(false);
            playerObjectDictionary.TryGetValue(nextType, out currentShape);
            currentShape.SetActive(true);

            currentType = nextType;
        }
    }

    void JoystickMove()
    {
        Vector3 moveInput = Vector3.ClampMagnitude(new Vector3(Util.GetAxis("Horizontal"), Util.GetAxis("Vertical")), 1);
        if (moveInput.magnitude > 0.01f)
        {
            gyroVector.x = moveInput.y * moveSensitivity;
            gyroVector.y = -moveInput.x * moveSensitivity;
        }
    }
    
    void RotatePlayer()
    {
        float horiz = Util.GetAxis("Horizontal Right");
        Vector3 desiredRotation = playerObject.eulerAngles;
        desiredRotation.z = 0;
        bool left = TCKInput.GetAction("Left", EActionEvent.Press) || horiz < 0;
        bool right = TCKInput.GetAction("Right", EActionEvent.Press) || horiz > 0;
        if (left && right)
        {
            desiredRotation.z = 180;
        }
        else if (left)
        {
            desiredRotation.z = 90;
        }
        else if (right)
        {
            desiredRotation.z = -90;
        }
        Quaternion desiredQuat = Quaternion.Euler(desiredRotation);
        playerObject.rotation = Quaternion.Lerp(playerObject.rotation, desiredQuat, rotateSpeed * Time.deltaTime);
    }

    void RotateWorld()
    {
        if (TCKInput.GetAction("Left", EActionEvent.Down))
        {
            RotateLeft();
        }
        if (TCKInput.GetAction("Right", EActionEvent.Down))
        {
            RotateRight();
        }
    }

    void RotateLeft()
    {
        Rotate(-90);
    }

    void RotateRight()
    {
        Rotate(90);
    }

    void Rotate(float angleAdjustAmount)
    {
        if (invertRotation)
        {
            angleAdjustAmount *= -1;
        }
        float desiredAngle = blockController.GetAdjustedDesiredAngle() + angleAdjustAmount;
        if (desiredAngle < 0)
        {
            desiredAngle += 360;
        }
        if (desiredAngle >= 360)
        {
            desiredAngle -= 360;
        }
        rotateClipDetail.Play(audioSource);
        blockController.SetDesiredAngle(desiredAngle);
    }

    void StartBoost()
    {
        if (!wallController.boosting)
        {
            wallController.IncreaseCurrentSpeed();
            if (wallController.boosting)
            {
                //freezeGyro = true;
                boostClipDetail.Play(audioSource);
                boostParticles.Play();
            }
        }
    }

    void StopBoost()
    {
        if (wallController.boosting)
        {
            //freezeGyro = false;
            wallController.DecreaseCurrentSpeed();
        }
    }

    private Vector2 swipeStart = Vector2.zero;
    private Vector2 swipeMove = Vector2.zero;
    public bool swiping = false;
    private string touchpad = "Touchpad";
    public float doubleTapTime = 0.5f;
    public float doubleTapDeadZone = 0.5f;
    private float firstTapTime;
    void SwipeMove()
    {
        ETouchPhase touchPhase = TCKInput.GetTouchPhase(touchpad);
        switch (touchPhase)
        {
            case ETouchPhase.Began:
                if (Time.time - firstTapTime <= doubleTapTime)
                {
                    swiping = false;
                    StartBoost();
                }
                else
                {
                    firstTapTime = Time.time;
                    swiping = true;
                    swipeStart = TCKInput.GetAxis(touchpad);
                    swipeMove = swipeStart;
                }

                break;
            case ETouchPhase.Stationary:
                if (swiping)
                {
                    swipeStart = TCKInput.GetAxis(touchpad);
                    swipeMove = swipeStart;
                }
                break;
            case ETouchPhase.Moved:
                if (swiping)
                {
                    swipeMove = TCKInput.GetAxis(touchpad);
                    HandleSwipe();
                }
                break;
            case ETouchPhase.Ended:
                ResetSwipe();
                //StopBoost();
                break;
        }
    }

    public string leftTouchPad = "left";
    public string rightTouchPad = "right";
    private Vector2 leftSwipeStart, rightSwipeStart;
    public float swipeSensitivity = 5;
    private bool leftSwiping, rightSwiping;
    private float firstLeftTapTime, firstRightTapTime;
    private float leftTapMovement, rightTapMovement;
    public float currentRotateLeftSpeed, currentRotateRightSpeed;
    public float rotateDrag = 0.5f;
    public float rotationSlowdownMinSpeedDeadzone = 0.1f;
    public float rotationSlowdownDeadzone = 15f;
    void SwipeRotate()
    {
        ETouchPhase leftTouchPhase = TCKInput.GetTouchPhase(leftTouchPad);
        switch (leftTouchPhase)
        {
            case ETouchPhase.Began:
            case ETouchPhase.Stationary:
            case ETouchPhase.Moved:
                if (!leftSwiping)
                {
                    if (Time.time - firstLeftTapTime <= doubleTapTime && leftTapMovement <= doubleTapDeadZone)
                    {
                        StartBoost();
                    }
                    else
                    {
                        leftTapMovement = 0;
                        firstLeftTapTime = Time.time;
                    }
                }

                Vector2 touchPadInput = TCKInput.GetAxis(leftTouchPad);
                leftTapMovement += touchPadInput.magnitude;
                currentRotateLeftSpeed = touchPadInput.y * swipeSensitivity;
                blockController.SetDesiredAngle(blockController.GetDesiredAngleRaw() + currentRotateLeftSpeed);
                leftSwiping = true;
                break;
            case ETouchPhase.Ended:
            case ETouchPhase.NoTouch:
                leftSwiping = false;
                break;
        }
        ETouchPhase rightTouchPhase = TCKInput.GetTouchPhase(rightTouchPad);
        switch (rightTouchPhase)
        {
            case ETouchPhase.Began:
            case ETouchPhase.Stationary:
            case ETouchPhase.Moved:
                if (!rightSwiping)
                {
                    if (Time.time - firstRightTapTime <= doubleTapTime && rightTapMovement <= doubleTapDeadZone)
                    {
                        StartBoost();
                    }
                    else
                    {
                        rightTapMovement = 0;
                        firstRightTapTime = Time.time;
                    }
                }

                Vector2 touchPadInput = TCKInput.GetAxis(rightTouchPad);
                rightTapMovement += touchPadInput.magnitude;
                currentRotateRightSpeed = touchPadInput.y * swipeSensitivity;
                blockController.SetDesiredAngle(blockController.GetDesiredAngleRaw() + currentRotateRightSpeed);
                rightSwiping = true;
                break;
            case ETouchPhase.Ended:
            case ETouchPhase.NoTouch:
                rightSwiping = false;
                break;
        }

        if (!leftSwiping && !rightSwiping)
        {
            HandleSwipeEnd();
        }
    }

    void HandleSwipeEnd()
    {
        //if (Mathf.Approximately(currentRotateLeftSpeed, 0) && Mathf.Approximately(currentRotateRightSpeed, 0))
        //{
        //    blockController.LockInAngle();
        //} else
        //{
        //    currentRotateLeftSpeed = SlowDownRotation(currentRotateLeftSpeed);
        //    currentRotateRightSpeed = SlowDownRotation(currentRotateRightSpeed);
        //    blockController.SetDesiredAngle(blockController.GetDesiredAngleRaw() + currentRotateLeftSpeed + currentRotateRightSpeed);
        //}


        //float currentRotateSpeed = currentRotateLeftSpeed + currentRotateRightSpeed;
        //float additionalAngle = 2 * currentRotateSpeed * currentRotateSpeed / (rotateDrag * 2);
        //blockController.SetDesiredAngle(blockController.GetDesiredAngleRaw() + additionalAngle);
        //blockController.LockInAngle();
        //currentRotateLeftSpeed = 0;
        //currentRotateRightSpeed = 0;


        float currentRotateSpeed = currentRotateLeftSpeed + currentRotateRightSpeed;
        if (currentRotateSpeed > rotationSlowdownMinSpeedDeadzone)
        {
            if (blockController.GetDesiredAngleRaw() > rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() < 90)
            {
                blockController.SetDesiredAngle(90);
            } else if (blockController.GetDesiredAngleRaw() > 90 + rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() < 180)
            {
                blockController.SetDesiredAngle(180);
            } else if (blockController.GetDesiredAngleRaw() > 180 + rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() < 270)
            {
                blockController.SetDesiredAngle(270);
            } else if (blockController.GetDesiredAngleRaw() > 270 + rotationSlowdownDeadzone)
            {
                blockController.SetDesiredAngle(0);
            }
        }
        if (currentRotateSpeed < -rotationSlowdownMinSpeedDeadzone)
        {
            if (blockController.GetDesiredAngleRaw() < 360 - rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() > 270)
            {
                blockController.SetDesiredAngle(270);
            }
            else if (blockController.GetDesiredAngleRaw() < 270 - rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() > 180)
            {
                blockController.SetDesiredAngle(180);
            }
            else if (blockController.GetDesiredAngleRaw() < 180 - rotationSlowdownDeadzone && blockController.GetDesiredAngleRaw() > 90)
            {
                blockController.SetDesiredAngle(90);
            }
            else if (blockController.GetDesiredAngleRaw() < 90 - rotationSlowdownDeadzone)
            {
                blockController.SetDesiredAngle(0);
            }
        }
        currentRotateLeftSpeed = 0;
        currentRotateRightSpeed = 0;
        blockController.LockInAngle();


        //if (slowDownRoutine == null)
        //{
        //    slowDownRoutine = StartCoroutine(SlowDown());
        //}
    }
    Coroutine slowDownRoutine;
    private IEnumerator SlowDown()
    {
        float currentRotateSpeed = currentRotateLeftSpeed + currentRotateRightSpeed;
        float rotateTime = currentRotateSpeed / rotateDrag;
        while (rotateTime > 0)
        {
            if (leftSwiping || rightSwiping)
            {
                yield break;
            }

            currentRotateLeftSpeed = SlowDownRotation(currentRotateLeftSpeed);
            currentRotateRightSpeed = SlowDownRotation(currentRotateRightSpeed);
            currentRotateSpeed = currentRotateLeftSpeed + currentRotateRightSpeed;

            rotateTime -= Time.deltaTime;
            blockController.SetDesiredAngle(blockController.GetDesiredAngleRaw() + currentRotateSpeed);
            yield return null;
        }
        blockController.LockInAngle();
        currentRotateLeftSpeed = 0;
        currentRotateRightSpeed = 0;
        slowDownRoutine = null;
    }

    float SlowDownRotation(float rotation)
    {
        if (rotation > 0)
        {
            rotation = Mathf.Clamp(rotation - rotateDrag * Time.deltaTime, 0, rotation);
        } else if (rotation < 0)
        {
            rotation = Mathf.Clamp(rotation + rotateDrag * Time.deltaTime, rotation, 0);
        }
        return rotation;
    }

    public float swipeDistance;
    void HandleSwipe()
    {
        Vector2 swipe = swipeMove - swipeStart;
        //if (Mathf.Abs(swipe.y) > Mathf.Abs(swipe.x))
        //{
        //    if (swipe.y > swipeDistance)
        //    {
        //        //swipe up
        //        StartBoost();
        //        ResetSwipe();
        //    }
        //    else if (swipe.y < -swipeDistance)
        //    {
        //        //swipe down
        //    }
        //}
        //else
        //{
            if (swipe.x > swipeDistance)
            {
                //swipe right
                RotateLeft();
                ResetSwipe();
            }
            else if (swipe.x < -swipeDistance)
            {
                //swipe left
                RotateRight();
                ResetSwipe();
            }
        //}
    }

    void ResetSwipe()
    {
        swipeStart = Vector2.zero;
        swipeMove = Vector2.zero;
        swiping = false;
    }

    void GyroMove()
    {
        Vector3 desiredPosition;
        bool resetGyro = TCKInput.GetAction("Recenter", EActionEvent.Down) || (doubleClickReset && TCKInput.GetAction("Left", EActionEvent.Press) && TCKInput.GetAction("Right", EActionEvent.Press));
        if (resetGyro)
        {
            ResetGyro();

            desiredPosition = new Vector3(0, 0, blockController.blockHolder.transform.position.z);
            blockController.desiredPosition = desiredPosition;
            gyroVector = Vector3.zero;
        }

        bool updateGyro = !doubleClickFreeze || !(TCKInput.GetAction("Left", EActionEvent.Press) && TCKInput.GetAction("Right", EActionEvent.Press));
        if (!freezeGyro && updateGyro)
        {
            Vector3 gyroInput = gyro.rotationRateUnbiased + gyroCalibration;

            //Vector3 gyroInput = Input.gyro.rotationRate;
            //Vector3 gyroInput = Input.acceleration;
            float sensitivity = gyroSensitivity;
            if (wallController.boosting)
            {
                sensitivity *= boostingSensitivity;
            }
            if (Mathf.Abs(gyroInput.x) > gyroDeadzone.x)
            {
                if (invertGyroX)
                {
                    gyroVector.x = - gyroInput.x * sensitivity;
                } else
                {
                    gyroVector.x = gyroInput.x * sensitivity;
                }
            }
            if (Mathf.Abs(gyroInput.y) > gyroDeadzone.y)
            {
                if (invertGyroY)
                {
                    gyroVector.y = - gyroInput.y * sensitivity;
                }
                else
                {
                    gyroVector.y = gyroInput.y * sensitivity;
                }
            }
            if (Mathf.Abs(gyroInput.z) > gyroDeadzone.z)
            {
                gyroVector.z = gyroInput.z * sensitivity;
            }
        }
        
        float x = -gyroVector.y;
        float y = gyroVector.x;

        //if (compensateForPlayerRotation)
        //{
        //    float desiredAngle = blockController.desiredAngle;
        //    if (Mathf.Approximately(desiredAngle, 0))
        //    {
        //        ;
        //    }
        //    else if (Mathf.Approximately(desiredAngle, 90))
        //    {
        //        float tempY = y;
        //        y = x;
        //        x = -tempY;
        //    }
        //    else if (Mathf.Approximately(desiredAngle, 180))
        //    {
        //        y = -y;
        //        x = -x;
        //    }
        //    else if (Mathf.Approximately(desiredAngle, 270))
        //    {
        //        float tempY = y;
        //        y = -x;
        //        x = tempY;
        //    }
        //}

        //Vector3 desiredPosition = new Vector3(x, y, transform.position.z);

        desiredPosition = blockController.blockHolder.transform.position + blockController.blockHolder.transform.up * y + blockController.blockHolder.transform.right * x;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        blockController.desiredPosition = desiredPosition;
    }

    void KillPlayer()
    {
        levelManager.OnPlayerDeath();
    }

    public void LoseMistake()
    {
        if (remainingMistakes > 0)
        {
            remainingMistakes--;
        }
    }

    public void GainMistake()
    {
        if (remainingMistakes < maxMistakes)
        {
            remainingMistakes++;
        }
    }

    public void RefillMistakes()
    {
        remainingMistakes = maxMistakes;
    }

    public void HitWall()
    {
        failureClipDetail.Play(audioSource);
        screenShake.Shake(failureShakeDuration, failureShakeAmount);

        AddAccuracy(0);
        wallsHit++;

        LoseMistake();

        if (remainingMistakes > 0 || invincible)
        {
            wallController.HitWall(false, 0, this);
            StopBoost();
        } else
        {
            //Game over
            levelManager.OnPlayerDeath();
        }
    }

    public void HitGoal(float hitAccuracy)
    {
        AddAccuracy(hitAccuracy);
        goalsHit++;
        
        successClipDetail.Play(audioSource);
        SwitchShape();
        wallController.HitWall(true, hitAccuracy, this);
        StopBoost();
    }

    public int TotalWallCount()
    {
        return wallsHit + goalsHit;
    }

    //TODO check this math
    public void AddAccuracy(float hitAccuracy)
    {
        accuracy = (accuracy * TotalWallCount() + hitAccuracy) / (TotalWallCount() + 1);
    }

    void AdjustSpeed()
    {
        if (TCKInput.GetAction("Faster", EActionEvent.Down))
        {
            StartBoost();
        }
        //if (TCKInput.GetAction("Faster", EActionEvent.Up))
        //{
        //    StopBoost();
        //}
    }
}
