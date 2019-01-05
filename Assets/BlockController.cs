using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public float moveAcceleration = 5;
    public float moveDecceleration = 5;
    public float maxMoveSpeed = 10;
    public Vector3 desiredPosition;
    public float minDistanceDifference = 0.01f;
    private Vector3 moveDirection;
    
    public float rotationSpeed;
    public float desiredAngle;
    public float minAngleDifference = 0.1f;

    public GameObject blockHolder;
    
    void Start()
    {
        moveDirection = Vector3.zero;
        desiredPosition = transform.position;
        
        desiredAngle = 0;
    }
    
    void FixedUpdate()
    {
        Move();

        Rotate();
    }

    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, maxMoveSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        Quaternion desiredRotation = Quaternion.Euler(0, 0, desiredAngle);
        blockHolder.transform.rotation = Quaternion.Lerp(blockHolder.transform.rotation, desiredRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
