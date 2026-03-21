using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class SearchState : ICarState
{
    private CarSensors sensors;
    private bool isMeasuringGap = false;
    private Vector3 gapStartPosition;
    private float requiredGapWidth = 3.0f;
    private bool spotFound = false;
    private bool hasPassedFirstObstacle = false;

    // Dynamiczna pozycja koñca luki (zderzak przedniego auta)
    private Vector3 gapEndPosition;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: [DYNAMICZNY] Szukam miejsca parkingowego...");
        sensors = car.GetComponent<CarSensors>();
    }

    public void UpdateState(CarController car)
    {
        if (spotFound)
        {
            // P-CONTROLLER DLA ZATRZYMANIA:
            // Musimy podjechaæ do przodu tak, by ty³ naszego auta min¹³ krawêdŸ przedniego auta.
            // D³ugoœæ naszego auta to ok. 4.5m, wiêc podje¿d¿amy na z góry wyliczon¹ odleg³oœæ od pocz¹tku luki.
            float targetDriveDistance = requiredGapWidth + 2.0f; // Szerokoœæ luki + zapas na zrównanie siê aut
            float currentDistance = Vector3.Distance(gapStartPosition, car.transform.position);

            float distanceLeft = targetDriveDistance - currentDistance;

            if (distanceLeft > 0.1f)
            {
                // P-Controller: Im bli¿ej celu, tym mniejszy gaz (Zwalniamy p³ynnie, nie jedziemy "na sztywno")
                car.verticalInput = Mathf.Clamp(distanceLeft * 0.2f, 0.1f, 0.3f);
                car.horizontalInput = 0f;
                car.breakInput = 0f;
            }
            else
            {
                // Idealna pozycja startowa osi¹gniêta!
                car.breakInput = 1f;
                car.verticalInput = 0f;
                car.ChangeState(new ParkState());
            }
            return;
        }

        car.verticalInput = 0.3f;
        car.horizontalInput = 0f;
        car.breakInput = 0f;

        if (sensors != null)
        {
            if (sensors.isObstacleDetected)
            {
                hasPassedFirstObstacle = true;

                if (isMeasuringGap)
                {
                    // W³aœnie dojechaliœmy do drugiego auta! (Koniec luki)
                    gapEndPosition = car.transform.position;
                    float currentGapWidth = Vector3.Distance(gapStartPosition, gapEndPosition);

                    if (currentGapWidth >= requiredGapWidth)
                    {
                        // WYBÓR IDEALNEGO PUNKTU (Œrodek luki w przesuniêciu o wymiar auta)
                        car.targetParkingSpot = (gapStartPosition + gapEndPosition) / 2f;
                        car.targetParkingSpot += car.transform.right * -2f; // Wsuwamy punkt docelowy "w g³¹b" luki

                        Debug.Log($"FSM: ZNALEZIONO LUKÊ! Szerokoœæ: {currentGapWidth:F2}m. Wyliczono œrodek!");
                        spotFound = true;
                    }
                    else
                    {
                        Debug.Log("FSM: Luka za ma³a, szukam dalej...");
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
        Debug.Log("FSM: Pozycjonowanie zakoñczone, oddajê stery do ParkState.");
    }
}