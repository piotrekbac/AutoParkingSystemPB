using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState_Perpendicular : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;
    private float entryAngle = 0f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania PROSTOPAD£EGO (90 stopni)...");
        parkingPhase = -1;
        timer = 0f;
        entryAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
    }

    public void UpdateState(CarController car)
    {
        float absoluteAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
        float deltaAngle = GetNormalizedAngle(absoluteAngle - entryAngle);

        if (parkingPhase == -1)
        {
            // FAZA -1: Zatrzymanie
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.brakeInput = 1f;
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                parkingPhase = 0;
                Debug.Log("FSM: [Faza 0] £amiê kierownicê na maxa w prawo i cofam!");
            }
        }
        else if (parkingPhase == 0)
        {
            // FAZA 0: Skrêt o 90 stopni do ty³u
            car.verticalInput = -0.4f;
            car.horizontalInput = 1f; // Kierownica MAX w prawo
            car.brakeInput = 0f;

            // Sprawdzamy czy auto obróci³o siê o blisko 90 stopni wzglêdem pozycji wejœciowej
            if (deltaAngle <= -85f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: [Faza 1] K¹t osi¹gniêty (-90st). Prostujê ko³a i cofam w g³¹b luki.");
            }
        }
        else if (parkingPhase == 1)
        {
            // FAZA 1: Jazda prosto do ty³u w g³¹b zatoczki
            car.horizontalInput = 0f; // Ko³a prosto
            car.verticalInput = -0.4f;
            car.brakeInput = 0f;

            // Ogranicznik wjazdu (np. z ty³u jest œciana lub auto wyjecha³o odpowiednio g³êboko)
            // U¿ywamy sztucznego timera (w prawdziwym œwiecie u¿ylibyœmy tylnego czujnika)
            timer += Time.deltaTime;
            if (timer > 4.5f) // Cofamy przez 3.5 sekundy (dostosuj ten czas, jeœli uderza w ty³)
            {
                parkingPhase = 2;
                Debug.Log("FSM: Zaparkowano prostopadle!");
            }
        }
        else if (parkingPhase == 2)
        {
            // FAZA 2: STOP
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.brakeInput = 1f;
        }
    }

    public void Exit(CarController car) { }

    private float GetNormalizedAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) return angle - 360f;
        if (angle < -180f) return angle + 360f;
        return angle;
    }
}