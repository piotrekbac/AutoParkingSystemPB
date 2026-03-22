using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarController : MonoBehaviour
{
    // NOWOŒÆ: Wybór typu parkowania (Ustawiane z poziomu Inspektora!)
    public enum ParkingMode { Parallel, Perpendicular, Dynamic }

    [Header("Tryb Mapy")]
    public ParkingMode currentMode = ParkingMode.Parallel;

    [Header("Referencje - Wheel Collidery (Fizyka)")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider; [Header("Referencje - Modele 3D kó³ (Wygl¹d)")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    [Header("Ustawienia Pojazdu")]
    public Transform centerOfMass;
    public float motorForce = 1500f;
    public float maxSteerAngle = 35f;
    public float wheelbase = 2.7f;
    public float trackWidth = 1.8f;

    // --- ZMIENNE DO STEROWANIA PRZEZ FSM ---
    public float verticalInput;
    public float horizontalInput;
    public float brakeInput;

    [HideInInspector] public Vector3 targetParkingSpot;

    private Rigidbody rb;
    private ICarState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null) rb.centerOfMass = centerOfMass.localPosition;

        // Zawsze zaczynamy od szukania miejsca, ale SearchState odczyta sobie tryb mapy z currentMode!
        ChangeState(new SearchState());
    }

    void Update()
    {
        if (currentState != null) currentState.UpdateState(this);
        UpdateWheelPoses();
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteeringAckermann();
    }

    private void HandleMotor()
    {
        rearLeftCollider.motorTorque = verticalInput * motorForce;
        rearRightCollider.motorTorque = verticalInput * motorForce;

        float currentBrakeForce = brakeInput * 3000f;
        frontLeftCollider.brakeTorque = currentBrakeForce;
        frontRightCollider.brakeTorque = currentBrakeForce;
        rearLeftCollider.brakeTorque = currentBrakeForce;
        rearRightCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteeringAckermann()
    {
        float steerAngle = horizontalInput * maxSteerAngle;

        if (Mathf.Abs(steerAngle) > 0.01f)
        {
            float turnRadius = wheelbase / Mathf.Tan(steerAngle * Mathf.Deg2Rad);
            float angleInner = Mathf.Atan(wheelbase / (turnRadius - (trackWidth / 2f))) * Mathf.Rad2Deg;
            float angleOuter = Mathf.Atan(wheelbase / (turnRadius + (trackWidth / 2f))) * Mathf.Rad2Deg;

            if (steerAngle > 0)
            {
                frontRightCollider.steerAngle = angleInner;
                frontLeftCollider.steerAngle = angleOuter;
            }
            else
            {
                frontLeftCollider.steerAngle = angleInner;
                frontRightCollider.steerAngle = angleOuter;
            }
        }
        else
        {
            frontLeftCollider.steerAngle = 0;
            frontRightCollider.steerAngle = 0;
        }
    }

    private void UpdateWheelPoses()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftVisual);
        UpdateSingleWheel(frontRightCollider, frontRightVisual);
        UpdateSingleWheel(rearLeftCollider, rearLeftVisual);
        UpdateSingleWheel(rearRightCollider, rearRightVisual);
    }

    private void UpdateSingleWheel(WheelCollider col, Transform visual)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        visual.position = pos;
        visual.rotation = rot;
    }

    public void ChangeState(ICarState newState)
    {
        if (currentState != null) currentState.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }
}