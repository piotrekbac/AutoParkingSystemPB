using UnityEngine;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public class SearchState : ICarState
{
    private CarSensors sensors;
    private bool isMeasuringGap = false;
    private Vector3 gapStartPosition;
    private Vector3 gapEndPosition;
    private float requiredGapWidth;
    private float overshootTarget;
    private bool spotFound = false;
    private bool hasPassedFirstObstacle = false;

    public void Enter(CarController car)
    {
        sensors = car.GetComponent<CarSensors>();
        if (car.currentMode == CarController.ParkingMode.Perpendicular)
        {
            Debug.Log("FSM: [MAPA 1] Szukam luki PROSTOPADŁEJ...");
            requiredGapWidth = 3.0f;
            // Dla cofania tyłem o 90°:
            // Auto musi przejechać za ŚRODEK luki o ~połowę długości auta (2.25m)
            // żeby tylna oś znalazła się przy środku luki podczas manewru
            // gapEndPosition = koniec luki, środek = gapEnd - gapWidth/2
            // Przejeżdżamy za gapEnd o: połowa luki (≈1.5m) + połowa auta (2.25m) = ~3.5m
            overshootTarget = 5.0f;
        }
        else
        {
            Debug.Log("FSM: [MAPA 2] Szukam luki RÓWNOLEGŁEJ (Koperta)...");
            requiredGapWidth = 5.5f;
            overshootTarget = 0.5f;
        }
        isMeasuringGap = false;
        hasPassedFirstObstacle = false;
        spotFound = false;
    }

    public void UpdateState(CarController car)
    {
        if (spotFound)
        {
            float distanceDrivenPastEnd = Vector3.Distance(gapEndPosition, car.transform.position);
            if (distanceDrivenPastEnd < overshootTarget)
            {
                car.verticalInput = 0.2f;
                car.horizontalInput = 0f;
                car.brakeInput = 0f;
            }
            else
            {
                car.brakeInput = 1f;
                car.verticalInput = 0f;
                car.horizontalInput = 0f;

                // Czekamy na pełny stop przed zmianą stanu
                Rigidbody rb = car.GetComponent<Rigidbody>();
                if (rb == null || rb.linearVelocity.magnitude < 0.05f)
                {
                    if (car.currentMode == CarController.ParkingMode.Perpendicular)
                        car.ChangeState(new ParkState_Perpendicular());
                    else
                        car.ChangeState(new ParkState());
                }
            }
            return;
        }

        car.verticalInput = 0.3f;
        car.horizontalInput = 0f;
        car.brakeInput = 0f;

        if (sensors != null)
        {
            if (sensors.isObstacleDetected)
            {
                hasPassedFirstObstacle = true;
                if (isMeasuringGap)
                {
                    gapEndPosition = car.transform.position;
                    float currentGapWidth = Vector3.Distance(gapStartPosition, gapEndPosition);
                    if (currentGapWidth >= requiredGapWidth)
                    {
                        car.targetParkingSpot = (gapStartPosition + gapEndPosition) / 2f;
                        Debug.Log($"FSM: Znalazłem lukę {currentGapWidth:F2}m! Dociągam do pozycji manewru.");
                        spotFound = true;
                    }
                    else
                    {
                        Debug.Log($"FSM: Luka {currentGapWidth:F2}m za mała.");
                    }
                    isMeasuringGap = false;
                }
            }
            else
            {
                if (hasPassedFirstObstacle && !isMeasuringGap)
                {
                    isMeasuringGap = true;
                    gapStartPosition = car.transform.position;
                    Debug.Log("FSM: Luka się zaczyna.");
                }
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM SearchState: EXIT");
    }
}