using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class SearchState : ICarState
{
    private CarSensors sensors;
    private bool isMeasuringGap = false;
    private Vector3 gapStartPosition;
    private Vector3 gapEndPosition;

    // Zmienne, które dostosuj¹ siê same do rodzaju mapy!
    private float requiredGapWidth;
    private float overshootTarget; // Jak daleko za lukê musimy odjechaæ

    private bool spotFound = false;
    private bool hasPassedFirstObstacle = false;

    public void Enter(CarController car)
    {
        sensors = car.GetComponent<CarSensors>();

        // UNIWERSALNOŒÆ: Mózg sam dobiera parametry na podstawie wybranej mapy!
        if (car.currentMode == CarController.ParkingMode.Perpendicular)
        {
            Debug.Log("FSM: [MAPA 1] Szukam luki PROSTOPAD£EJ (Min. 3.0m)...");
            requiredGapWidth = 3.0f; // Luka prostopad³a jest wê¿sza
            overshootTarget = 3.0f;  // Musimy odjechaæ dalej, by mieæ miejsce na z³amanie auta o 90 stopni
        }
        else
        {
            Debug.Log("FSM:[MAPA 2] Szukam luki RÓWNOLEG£EJ (Min. 5.5m)...");
            requiredGapWidth = 5.5f;
            overshootTarget = 0.5f;  // Stajemy niemal od razu za autem z przodu
        }
    }

    public void UpdateState(CarController car)
    {
        if (spotFound)
        {
            float distanceDrivenPastEnd = Vector3.Distance(gapEndPosition, car.transform.position);

            // Zatrzymujemy siê w idealnym miejscu zale¿nym od trybu parkowania
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

                // Przekazanie pa³eczki do odpowiedniego algorytmu parkowania!
                if (car.currentMode == CarController.ParkingMode.Perpendicular)
                    car.ChangeState(new ParkState_Perpendicular());
                else
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
                        Debug.Log($"FSM: SUKCES! Znalaz³em lukê ({currentGapWidth:F2}m). Odje¿d¿am do przodu...");
                        spotFound = true;
                    }
                    else
                    {
                        Debug.Log($"FSM: Luka za ma³a ({currentGapWidth:F2}m < {requiredGapWidth}m). Szukam dalej!");
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
                }
            }
        }
    }

    public void Exit(CarController car) { }
}