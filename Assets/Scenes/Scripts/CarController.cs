using UnityEngine;
using System.Collections.Generic; // NOWOŒÆ: Potrzebne do obs³ugi Stosu (Pushdown Automata)

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarController : MonoBehaviour
{
    public enum ParkingMode { Parallel, Perpendicular, Dynamic }

    [Header("Tryb Mapy")]
    public ParkingMode currentMode = ParkingMode.Parallel;

    [Header("Referencje - Wheel Collidery")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Referencje - Modele 3D")]
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

    public float verticalInput;
    public float horizontalInput;
    public float brakeInput; [HideInInspector] public Vector3 targetParkingSpot;

    private Rigidbody rb;

    // --- NOWOŒÆ: Pushdown Automata (Automat ze stosem) ---
    public ICarState currentState;
    private Stack<ICarState> stateStack = new Stack<ICarState>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null) rb.centerOfMass = centerOfMass.localPosition;

        ChangeState(new SearchState());
    }

    void Update()
    {
        // GLOBALNY SYSTEM ANTYKOLIZYJNY (Przerwanie HFSM)
        CarSensors sensors = GetComponent<CarSensors>();
        if (sensors != null && sensors.isFrontObstacleDetected)
        {
            // Jeœli wykryto pieszego, a nie jesteœmy jeszcze w stanie awaryjnym - ZAMRA¯AMY OBECNY STAN!
            if (!(currentState is EmergencyState))
            {
                PushState(new EmergencyState());
            }
        }

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
        col.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visual.position = pos;
        visual.rotation = rot;
    }

    // Klasyczna zmiana stanu (czyœci historiê)
    public void ChangeState(ICarState newState)
    {
        if (currentState != null) currentState.Exit(this);
        stateStack.Clear();

        currentState = newState;
        stateStack.Push(newState);
        currentState.Enter(this);
    }

    // ZAMRO¯ENIE STANU (K³adzie nowy stan na wierzch stosu)
    public void PushState(ICarState newState)
    {
        currentState = newState;
        stateStack.Push(newState);
        currentState.Enter(this);
    }

    // WZNOWIENIE STANU (Zdejmuje stan awaryjny i wraca do poprzedniego)
    public void PopState()
    {
        if (stateStack.Count > 0)
        {
            ICarState oldState = stateStack.Pop();
            oldState.Exit(this);
        }

        if (stateStack.Count > 0)
        {
            currentState = stateStack.Peek();
            Debug.Log("FSM [HFSM]: Wznawiam zamro¿ony manewr!");
        }
    }
}