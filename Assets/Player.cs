﻿using System.Collections;
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

    public float moveSensitivity;
    public float minX, maxX, minY, maxY;
    public float minMagToRotate = 0.1f;

    public BlockController blockController;
    ScreenShake screenShake;
    
    Vector3 gyroVector;
    public float gyroSensitivity = 2;
    public Vector3 gyroCalibration = Vector3.zero;
    public Vector3 gyroDeadzone = new Vector3(0.01f, 0.01f, 0.01f);
    public float rotateSpeed = 8;
    public bool invertRotation = false;

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

        Util.ResetGyro();
        ResetGyro();
        UpdateGyroSensitivity();

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
    }
    void UpdateGyroSensitivity()
    {
        gyroSensitivity = PlayerPrefs.GetFloat(Options.GYRO_SENSITIVITY);

        gyroCalibration.x = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'x');
        gyroCalibration.y = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'y');
        gyroCalibration.z = PlayerPrefs.GetFloat(Options.GYRO_CALIBRATION + 'z');
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

        SwipeMove();
        RotateWorld();
        GyroMove();
        JoystickMove();

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
            gyroVector.x += moveInput.y * moveSensitivity;
            gyroVector.y += -moveInput.x * moveSensitivity;
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
        float desiredAngle = blockController.desiredAngle + angleAdjustAmount;
        if (desiredAngle < 0)
        {
            desiredAngle += 360;
        }
        if (desiredAngle >= 360)
        {
            desiredAngle -= 360;
        }
        rotateClipDetail.Play(audioSource);
        blockController.desiredAngle = desiredAngle;
    }

    void StartBoost()
    {
        //freezeGyro = true;
        boostClipDetail.Play(audioSource);
        boostParticles.Play();
        wallController.IncreaseCurrentSpeed();
    }

    void StopBoost()
    {
        //freezeGyro = false;
        //wallController.DecreaseCurrentSpeed();
    }

    private Vector2 swipeStart = Vector2.zero;
    private Vector2 swipeMove = Vector2.zero;
    public bool swiping = false;
    private string Touchpad = "Touchpad";
    public float doubleTapTime = 0.5f;
    private float firstTapTime;
    void SwipeMove()
    {
        ETouchPhase touchPhase = TCKInput.GetTouchPhase(Touchpad);
        switch(touchPhase)
        {
            case ETouchPhase.Began:
                if (Time.time - firstTapTime <= doubleTapTime)
                {
                    swiping = false;
                    StartBoost();
                } else
                {
                    firstTapTime = Time.time;
                    swiping = true;
                    swipeStart = TCKInput.GetAxis(Touchpad);
                    swipeMove = swipeStart;
                }

                break;
            case ETouchPhase.Stationary:
                if (swiping)
                {
                    swipeStart = TCKInput.GetAxis(Touchpad);
                    swipeMove = swipeStart;
                }
                break;
            case ETouchPhase.Moved:
                if (swiping)
                {
                    swipeMove = TCKInput.GetAxis(Touchpad);
                    HandleSwipe();
                }
                break;
            case ETouchPhase.Ended:
                ResetSwipe();
                StopBoost();
                break;
        }
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
        bool resetGyro = TCKInput.GetAction("Recenter", EActionEvent.Down) || (doubleClickReset && TCKInput.GetAction("Left", EActionEvent.Press) && TCKInput.GetAction("Right", EActionEvent.Press));
        if (resetGyro)
        {
            ResetGyro();
        }

        bool updateGyro = !doubleClickFreeze || !(TCKInput.GetAction("Left", EActionEvent.Press) && TCKInput.GetAction("Right", EActionEvent.Press));
        if (!freezeGyro && updateGyro)
        {
            Vector3 gyroInput = Input.gyro.rotationRateUnbiased + gyroCalibration;

            //Vector3 gyroInput = Input.gyro.rotationRate;
            //Vector3 gyroInput = Input.acceleration;
            if (Mathf.Abs(gyroInput.x) > gyroDeadzone.x)
            {
                gyroVector.x += gyroInput.x * gyroSensitivity;
            }
            if (Mathf.Abs(gyroInput.y) > gyroDeadzone.y)
            {
                gyroVector.y += gyroInput.y * gyroSensitivity;
            }
            if (Mathf.Abs(gyroInput.z) > gyroDeadzone.z)
            {
                gyroVector.z += gyroInput.z * gyroSensitivity;
            }
        }
        
        float x = -gyroVector.y;
        float y = gyroVector.x;

        if (compensateForPlayerRotation)
        {
            float desiredAngle = blockController.desiredAngle;
            if (Mathf.Approximately(desiredAngle, 0))
            {
                ;
            }
            else if (Mathf.Approximately(desiredAngle, 90))
            {
                float tempY = y;
                y = x;
                x = -tempY;
            }
            else if (Mathf.Approximately(desiredAngle, 180))
            {
                y = -y;
                x = -x;
            }
            else if (Mathf.Approximately(desiredAngle, 270))
            {
                float tempY = y;
                y = -x;
                x = tempY;
            }
        }

        Vector3 desiredPosition = new Vector3(x, y, transform.position.z);

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        blockController.desiredPosition = desiredPosition;
    }

    void KillPlayer()
    {
        levelManager.OnPlayerDeath();
    }

    public void HitWall()
    {
        failureClipDetail.Play(audioSource);
        screenShake.Shake(failureShakeDuration, failureShakeAmount);

        if (!invincible)
        {
            remainingMistakes--;
        }

        if (remainingMistakes > 0)
        {
            wallController.HitWall(false, this);
            StopBoost();
        } else
        {
            //Game over
            levelManager.OnPlayerDeath();
        }
    }

    public void HitGoal()
    {
        if (remainingMistakes < maxMistakes)
        {
            remainingMistakes++;
        }
        successClipDetail.Play(audioSource);
        SwitchShape();
        wallController.HitWall(true, this);
        StopBoost();
    }

    void AdjustSpeed()
    {
        if (TCKInput.GetAction("Faster", EActionEvent.Down))
        {
            StartBoost();
        }
        if (TCKInput.GetAction("Faster", EActionEvent.Up))
        {
            StopBoost();
        }
    }
}
