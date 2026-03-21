using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class SearchState : ICarState
{
    private CarSensors sensors;
    private bool isMeasuringGap = false;
    private Vector3 gapStartPosition;
    private float requiredGapWidth = 5.5f; // Optymalna luka dla naszego 4.5m auta
    private bool spotFound = false;
    private bool hasPassedFirstObstacle = false;
    private Vector3 gapEndPosition;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");
        sensors = car.GetComponent<CarSensors>();
    }

    public void UpdateState(CarController car)
    {
        if (spotFound)
        {
            // Mierzymy odleg³oœæ od ZDERZAKA drugiego auta
            float distanceDrivenPastEnd = Vector3.Distance(gapEndPosition, car.transform.position);

            // Musimy przejechaæ 3 metry dalej, by idealnie ustawiæ siê RÓWNOLEGLE do drugiego auta
            if (distanceDrivenPastEnd < 3.0f)
            {
                car.verticalInput = 0.2f;
                car.horizontalInput = 0f;
                car.brakeInput = 0f;
            }
            else
            {
                // Jesteœmy na pozycji startowej! Oddajemy kontrolê do ParkState
                car.brakeInput = 1f;
                car.verticalInput = 0f;
                car.ChangeState(new ParkState());
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
                        Debug.Log($"FSM: SUKCES! Znalaz³em lukê. Ma ona {currentGapWidth:F2} metrów. Ustawiam siê do cofania...");
                        spotFound = true;
                    }
                    else
                    {
                        Debug.Log($"FSM: Luka mia³a tylko {currentGapWidth:F2}m. Szukam dalej!");
                    }
                    isMeasuringGap = false;
                }
            }
            else
            {
                if (hasPassedFirstObstacle)
                {
                    if (!isMeasuringGap)
                    {
                        isMeasuringGap = true;
                        gapStartPosition = car.transform.position;
                    }
                }
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Zakoñczy³em etap szukania.");
    }
}