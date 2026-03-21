using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    // Prywatna zmienna parkingPhase do œledzenia fazy parkowania.
    private int parkingPhase = -1;

    // Prywatna zmienna timer do œledzenia czasu.
    private float timer = 0f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania (Bieg wsteczny)...");
        parkingPhase = -1;
        timer = 0f;
    }

    public void UpdateState(CarController car)
    {
        // Pobieramy znormalizowany k¹t
        float currentAngle = GetNormalizedAngle(car.transform.eulerAngles.y);

        if (parkingPhase == -1)
        {
            // Faza -1: Pe³ne zatrzymanie (Zabijamy pêd samochodu) 
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.breakInput = 1f;

            timer += Time.deltaTime;

            if (timer > 1.5f)
            {
                parkingPhase = 0;
                Debug.Log("FSM: Auto jest ju¿ zatrzymane. Zaczynam manewr parkowania!");
            }
        }
        else if (parkingPhase == 0)
        {
            // FAZA 0: Skrêt w prawo i cofanie
            car.verticalInput = -0.5f;
            car.horizontalInput = 1f;
            car.breakInput = 0f;

            // Czekamy, a¿ odwróci siê o 45 stopni
            if (currentAngle <= -45f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: Auto jest pod k¹tem -45 stopni. Robie KONTRÊ KIEROWNIC¥!");
            }
        }
        else if (parkingPhase == 1)
        {
            // Faza 1: Wsuwamy przód auta (skrêt w lewo i jazda do ty³u)
            car.horizontalInput = -1f;
            car.verticalInput = -0.5f;
            car.breakInput = 0f;

            // TUTAJ BY£ B£¥D! 
            // K¹t idzie od -45 w stronê zera. Zatrzymujemy, gdy jest PRAWIE zero (idealnie prosto z drog¹).
            if (currentAngle >= -1f)
            {
                parkingPhase = 2;
                Debug.Log("FSM: SUKCES! Koñczê manewr parkowania i PROSTUJÊ KO£A!");
            }
        }
        else if (parkingPhase == 2)
        {
            // Faza 2 - Auto jest ju¿ prosto, wiêc koñczymy manewr.
            // Ustawienie horizontalInput na 0f fizycznie PROSTUJE ko³a na wprost!
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.breakInput = 1f;
        }
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM: Zakoñczy³em manewr parkowania.");
    }

    // Funkcja pomocnicza - normalizacja k¹tów od -180 do 180
    private float GetNormalizedAngle(float angle)
    {
        angle = angle % 360;

        if (angle > 180f)
        {
            return angle - 360f;
        }
        if (angle < -180f)
        {
            return angle + 360f;
        }
        return angle;
    }
}