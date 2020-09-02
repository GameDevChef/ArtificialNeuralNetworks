using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    protected float horizontalInput;
    protected float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    private Rigidbody rb;

    [SerializeField] private bool useBasicMovement;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;

   

    [SerializeField] private Transform rearRightWheelTransform;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = useBasicMovement;
        if (useBasicMovement)
        {
            var colliders = GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        GetInput();
        if (useBasicMovement)
            HandleBasicMovement();
        else
            HandlePhysicsMovement();
    }

    private void HandleBasicMovement()
    {
        float translation = Time.deltaTime * movementSpeed * verticalInput;
        float rotation = Time.deltaTime * rotationSpeed * horizontalInput;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    private void HandlePhysicsMovement()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    protected virtual void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();

    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot
;       wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    public void DecreaseSpeed(float value)
    {
        movementSpeed *= value;
    }
}
