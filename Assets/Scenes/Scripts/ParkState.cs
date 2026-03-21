using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;

    // Dynamicznie wyliczone parametry parkowania (Okrêgi styczne z PDF)
    private float turningRadius;
    private float inflectionAngle; // K¹t przegiêcia (kiedy robimy kontrê)

    public void Enter(CarController car)
    {
        Debug.Log("FSM: [DYNAMICZNY] Zaczynam matematyczny manewr parkowania...");
        parkingPhase = -1;
        timer = 0f;

        // GEOMETRIA 2D: Wyliczanie Okrêgów Stycznych
        turningRadius = car.GetTurningRadius();

        // Z³o¿ony wzór na k¹t "z³amania" w oparciu o promieñ skrêtu auta i szerokoœæ luki (2.2 metra to g³êbokoœæ parkingu)
        float lateralShift = 2.2f;

        // Obliczenie k¹ta przegiêcia (inflection point)
        // Wzór: arccos(1 - (shift / 2R))
        inflectionAngle = Mathf.Acos(1f - (lateralShift / (2f * turningRadius))) * Mathf.Rad2Deg;

        Debug.Log($"FSM: Wyliczony promieñ skrêtu (R): {turningRadius:F2}m. K¹t kontry: {inflectionAngle:F1} stopni.");
    }

    public void UpdateState(CarController car)
    {
        float currentAngle = GetNormalizedAngle(car.transform.eulerAngles.y);

        if (parkingPhase == -1)
        {
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.breakInput = 1f;

            timer += Time.deltaTime;
            if (timer > 1.5f) parkingPhase = 0;
        }
        else if (parkingPhase == 0)
        {
            // TEORIA STEROWANIA: P-Controller dla p³ynnego cofania
            // Auto cofa dynamicznie - im bli¿ej momentu wykrêcenia, tym p³ynniej jedzie
            car.verticalInput = -0.4f;
            car.horizontalInput = 1f;
            car.breakInput = 0f;

            // Zamiast sztywnego -45, u¿ywamy wyliczonego z fizyki auta dynamicznego k¹ta!
            if (currentAngle <= -inflectionAngle)
            {
                parkingPhase = 1;
                Debug.Log("FSM: Osi¹gniêto punkt przegiêcia krzywej. Robiê KONTRE!");
            }
        }
        else if (parkingPhase == 1)
        {
            car.horizontalInput = -1f;
            car.verticalInput = -0.4f;
            car.breakInput = 0f;

            if (currentAngle >= -1f)
            {
                parkingPhase = 2;
            }
        }
        else if (parkingPhase == 2)
        {
            // TEORIA STEROWANIA: P-Controller precyzyjnego dojazdu do œrodka luki
            // Samochód jest prosto, ale mo¿e potrzebowaæ dojechaæ do ty³u/przodu by staæ na œrodku
            float errorDistance = car.transform.position.z - car.targetParkingSpot.z;

            if (Mathf.Abs(errorDistance) > 0.2f)
            {
                car.horizontalInput = 0f;
                // P-Controller d¹¿y do wyzerowania b³êdu pozycji (errorDistance)
                car.verticalInput = Mathf.Clamp(-errorDistance * 0.5f, -0.3f, 0.3f);
                car.breakInput = 0f;
            }
            else
            {
                // Idealnie w œrodku! Zaci¹gamy rêczny.
                car.horizontalInput = 0f;
                car.verticalInput = 0f;
                car.breakInput = 1f;
                Debug.Log("FSM: SUKCES! Zaparkowano centralnie z u¿yciem P-Controllera!");
            }
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Manewr parkowania zakoñczony.");
    }

    private float GetNormalizedAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) return angle - 360f;
        if (angle < -180f) return angle + 360f;
        return angle;
    }
}