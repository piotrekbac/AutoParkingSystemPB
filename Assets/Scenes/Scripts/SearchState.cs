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

    // NOWOŒÆ: Zmienna zapamiêtuj¹ca, gdzie dok³adnie jest przednie auto
    private Vector3 gapEndPosition;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");
        sensors = car.GetComponent<CarSensors>();
    }

    public void UpdateState(CarController car)
    {
        // Je¿eli ju¿ znaleŸliœmy miejsce, pozycjonujemy siê DO PRZODU
        if (spotFound)
        {
            // KLUCZOWA POPRAWKA MATEMATYCZNA:
            // Mierzymy dystans od KOÑCA luki (czyli od momentu, gdy laser uderzy³ w przednie auto)
            float distanceDrivenPastEnd = Vector3.Distance(gapEndPosition, car.transform.position);

            // Chcemy odjechaæ 2.5 metra ZA przedni murek, ¿eby zderzaki aut siê "zrówna³y"
            float distanceLeft = 2.5f - distanceDrivenPastEnd;

            // P-Controller: P³ynne zwalnianie im bli¿ej celu
            if (distanceLeft > 0.05f)
            {
                car.verticalInput = Mathf.Clamp(distanceLeft * 0.5f, 0.1f, 0.3f);
                car.horizontalInput = 0f;
                car.breakInput = 0f;
            }
            else
            {
                // Jesteœmy idealnie wyjechani do przodu! Zatrzymujemy siê!
                car.breakInput = 1f;
                car.verticalInput = 0f;
                car.ChangeState(new ParkState());
            }
            return;
        }

        // Faza Szukania
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
                    // W£AŒNIE ZNALELIŒMY DRUGIE AUTO! (Koniec luki)
                    gapEndPosition = car.transform.position;
                    float currentGapWidth = Vector3.Distance(gapStartPosition, gapEndPosition);

                    if (currentGapWidth >= requiredGapWidth)
                    {
                        // Zapisujemy idealny œrodek luki, by ParkState móg³ tam precyzyjnie dojechaæ
                        car.targetParkingSpot = (gapStartPosition + gapEndPosition) / 2f;

                        Debug.Log($"FSM: SUKCES! Znaleziono lukê (Szerokoœæ: {currentGapWidth:F2}m). Odje¿d¿am do przodu...");
                        spotFound = true;
                    }
                    else
                    {
                        Debug.Log("FSM: Luka by³a za ma³a! Ignoruje i szukam dalej...");
                    }

                    isMeasuringGap = false;
                }
            }
            else
            {
                if (hasPassedFirstObstacle == true)
                {
                    if (!isMeasuringGap)
                    {
                        isMeasuringGap = true;
                        gapStartPosition = car.transform.position;
                        Debug.Log("FSM: Zauwa¿ono pocz¹tek luki! Rozpoczynam pomiar...");
                    }
                }
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Zakoñczy³em poszukiwanie miejsca.");
    }
}