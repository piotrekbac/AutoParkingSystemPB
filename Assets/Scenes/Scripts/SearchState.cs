using UnityEngine;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public class SearchState : ICarState
{
    private CarSensors sensors;

    // --- Pomiar luki ---
    private bool isMeasuringGap = false;
    private Vector3 gapStartPosition;
    private Vector3 gapEndPosition;
    private float requiredGapWidth = 5.5f;

    // --- Flagi ---
    private bool hasPassedFirstObstacle = false;
    private bool spotFound = false;

    // --- Bufor antydrganiowy (czujnik musi byæ wolny przez N klatek z rzêdu) ---
    private int freeClearFrames = 0;
    private const int REQUIRED_FREE_FRAMES = 4;

    // --- Faza dojazdu do pozycji startowej manewru ---
    // Ile metrów PRZED pocz¹tkiem drugiego bloku chcemy siê zatrzymaæ.
    // Wartoœæ ujemna = zatrzymujemy siê ZA pocz¹tkiem luki (za pierwsz¹ krawêdzi¹ drugiego bloku).
    // 0.0f = zatrzymujemy siê dok³adnie gdy czujnik wykryje drugi blok.
    // Docelowo: chcemy staæ tak, by TY£ auta by³ przy tylnej krawêdzi luki.
    private const float OVERSHOOT_TARGET = 0.2f; // metrów za krawêdzi¹ drugiego bloku
    private bool brakingPhase = false;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");
        sensors = car.GetComponent<CarSensors>();
        isMeasuringGap = false;
        hasPassedFirstObstacle = false;
        spotFound = false;
        freeClearFrames = 0;
        brakingPhase = false;
    }

    public void UpdateState(CarController car)
    {
        if (spotFound)
        {
            HandleApproachToManeuverPosition(car);
            return;
        }

        // JedŸ powoli do przodu - ma³a prêdkoœæ = lepsza precyzja zatrzymania
        car.verticalInput = 0.25f;
        car.horizontalInput = 0f;
        car.brakeInput = 0f;

        if (sensors == null) return;

        if (sensors.isObstacleDetected)
        {
            freeClearFrames = 0;

            if (!hasPassedFirstObstacle)
            {
                // Mijamy pierwszy blok - zaczynamy nas³uchiwaæ na lukê
                hasPassedFirstObstacle = true;
                Debug.Log("FSM: Min¹³em pierwszy blok. Zaczynam mierzyæ lukê.");
            }
            else if (isMeasuringGap)
            {
                // Czujnik wykry³ DRUGI blok - luka siê koñczy, zapisujemy jej koñcow¹ krawêdŸ
                gapEndPosition = car.transform.position;
                float gapWidth = Vector3.Distance(gapStartPosition, gapEndPosition);

                if (gapWidth >= requiredGapWidth)
                {
                    car.targetParkingSpot = (gapStartPosition + gapEndPosition) / 2f;
                    Debug.Log($"FSM: SUKCES! Luka {gapWidth:F2}m >= {requiredGapWidth}m. Doci¹gam do pozycji startowej.");
                    spotFound = true;
                    brakingPhase = false;
                }
                else
                {
                    Debug.Log($"FSM: Luka {gapWidth:F2}m za ma³a. Szukam dalej.");
                    isMeasuringGap = false;
                }
            }
        }
        else
        {
            if (hasPassedFirstObstacle)
            {
                freeClearFrames++;
                if (freeClearFrames >= REQUIRED_FREE_FRAMES && !isMeasuringGap)
                {
                    isMeasuringGap = true;
                    gapStartPosition = car.transform.position;
                    Debug.Log("FSM: Luka siê zaczyna - mierzê!");
                }
            }
        }
    }

    /// <summary>
    /// Po znalezieniu luki: doje¿d¿amy TYLKO o OVERSHOOT_TARGET metrów za krawêdŸ drugiego bloku,
    /// po czym natychmiast hamujemy i przechodzimy do ParkState.
    /// Ma³a prêdkoœæ + agresywne hamowanie = precyzja.
    /// </summary>
    private void HandleApproachToManeuverPosition(CarController car)
    {
        float distancePastEnd = Vector3.Distance(gapEndPosition, car.transform.position);

        if (!brakingPhase)
        {
            if (distancePastEnd < OVERSHOOT_TARGET)
            {
                // Jeszcze nie dojechaliœmy do celu - jedŸ BARDZO wolno
                car.verticalInput = 0.18f;
                car.horizontalInput = 0f;
                car.brakeInput = 0f;
            }
            else
            {
                // Osi¹gnêliœmy cel - PE£NE HAMOWANIE
                brakingPhase = true;
                car.verticalInput = 0f;
                car.brakeInput = 1f;
                Debug.Log($"FSM: Osi¹gn¹³em pozycjê {distancePastEnd:F2}m za krawêdzi¹. Hamujê!");
            }
        }
        else
        {
            // Czekamy a¿ auto faktycznie stanie (prêdkoœæ bliska zeru)
            car.verticalInput = 0f;
            car.brakeInput = 1f;
            car.horizontalInput = 0f;

            Rigidbody rb = car.GetComponent<Rigidbody>();
            float speed = rb != null ? rb.linearVelocity.magnitude : 0f;

            if (speed < 0.05f)
            {
                car.brakeInput = 1f;
                Debug.Log("FSM: Auto stoi. Startujê ParkState!");
                car.ChangeState(new ParkState());
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Zakoñczy³em etap szukania.");
    }
}