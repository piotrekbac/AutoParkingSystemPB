using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

// Nasza klasa SearchState dziedziczy po interfejsie ICarState
public class SearchState : ICarState
{
    // Tworzê zmienn¹ typu CarSensors, która bêdzie przechowywaæ referencjê do komponentu CarSensors przypisanego do samochodu.
    private CarSensors sensors;

    // Zmienne do mierzenia luki 
    private bool isMeasuringGap = false;        // Flaga, która wskazuje, czy aktualnie mierzymy lukê miêdzy samochodami.
    private Vector3 gapStartPosition;           // Zmienna do przechowywania pozycji pocz¹tkowej.
    private float requiredGapWidth = 3.0f;      // Zmienna okreœlaj¹ca wymagan¹ szerokoœæ luki. 
    private bool spotFound = false;             // Flaga, która wskazuje, czy znaleŸliœmy odpowiednie miejsce.

    // Flaga, która wskazuje, czy samochód min¹³ ju¿ pierwsz¹ przeszkodê.
    private bool hasPassedFirstObstacle = false;

    // Implementacja metody Enter z interfejsu ICarState.
    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");
        sensors = car.GetComponent<CarSensors>();
    }

    // Implementacja metody UpdateState z interfejsu ICarState.
    public void UpdateState(CarController car)
    {
        // Je¿eli ju¿ znaleŸliœmy miejsce, nic wiêcej nie robimy - tylko stoimy i pozycjonujemy siê
        if (spotFound)
        {
            float distanceDrivenPastSpot = Vector3.Distance(gapStartPosition, car.transform.position) - requiredGapWidth;

            // Podje¿d¿amy dodatkowe 5 metrów do przodu
            if (distanceDrivenPastSpot < 0f)
            {
                car.verticalInput = 0.2f;
                car.horizontalInput = 0f;
                car.breakInput = 0f;
            }
            // Zatrzymujemy samochód i przechodzimy do stanu parkowania 
            else
            {
                car.breakInput = 1f;
                car.verticalInput = 0f;
                car.ChangeState(new ParkState());
            }
            return;
        }

        // Odbieramy graczowi klawiaturê - AI bêdzie wciskaæ gaz na 30% mocy
        car.verticalInput = 0.3f;
        car.horizontalInput = 0f;
        car.breakInput = 0f;

        if (sensors != null)
        {
            if (sensors.isObstacleDetected)
            {
                // Ustawiamy flagê hasPassedFirstObstacle na true, co oznacza, ¿e samochód min¹³ ju¿ pierwsz¹ przeszkodê!
                hasPassedFirstObstacle = true;

                // Widzimy œcianê/inne auto 
                if (isMeasuringGap)
                {
                    Debug.Log("FSM: Luka by³a za ma³a! Ignoruje i szukam dalej...");
                    isMeasuringGap = false;
                }
            }
            else
            {
                // TUTAJ JEST KLUCZOWA ZMIANA!
                // Zanim zaczniemy cokolwiek mierzyæ, sprawdzamy czy minêliœmy ju¿ pierwsz¹ przeszkodê!
                if (hasPassedFirstObstacle == true)
                {
                    // Widzimy pust¹ przestrzeñ (laser jest zielony) i minêliœmy auto!
                    if (!isMeasuringGap)
                    {
                        isMeasuringGap = true;
                        gapStartPosition = car.transform.position;
                        Debug.Log("FSM: Zauwa¿ono pocz¹tek luki! Rozpoczynam pomiar...");
                    }
                    else
                    {
                        // Samochód jedzie wzd³u¿ luki, a my mierzymy odleg³oœæ
                        float currentGapWidth = Vector3.Distance(gapStartPosition, car.transform.position);

                        if (currentGapWidth >= requiredGapWidth)
                        {
                            Debug.Log($"FSM: SUKCES! Znalaz³em idealne miejsce (Szerokoœæ: {currentGapWidth:F2}m). Zatrzymujê auto!");
                            spotFound = true;
                        }
                    }
                }
                else
                {
                    // Auto jest na samym starcie gry przed pierwsz¹ kostk¹. 
                    // Laser jest zielony, ale IGNORUJEMY to, bo nie minêliœmy jeszcze ¿adnego auta.
                }
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Zakoñczy³em poszukiwanie miejsca.");
    }
}