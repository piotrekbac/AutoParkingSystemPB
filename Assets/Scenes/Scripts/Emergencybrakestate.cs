using UnityEngine;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
//
// SuperStan awaryjny - implementacja Pushdown Automaton (PDF str. 8)
// Przerywa DOWOLNY aktywny stan gdy wykryje przeszkodê z przodu.
// Po ust¹pieniu przeszkody WRACA do dok³adnie tego samego stanu który przerwa³
// (stan jest przechowywany na "stosie" - w polu interruptedState).

public class EmergencyBrakeState : ICarState
{
    // Stan który przerwaliœmy - wrócimy do niego gdy droga siê zwolni
    private readonly ICarState interruptedState;

    // Ile sekund czekamy po ust¹pieniu przeszkody zanim wznowimy jazdê
    private const float CLEAR_WAIT_SECONDS = 1.5f;
    private float clearTimer = 0f;

    public EmergencyBrakeState(ICarState interrupted)
    {
        interruptedState = interrupted;
    }

    public void Enter(CarController car)
    {
        Debug.LogWarning("FSM [EMERGENCY]: Przeszkoda! Zatrzymujê pojazd.");
        car.verticalInput = 0f;
        car.brakeInput = 1f;
        car.horizontalInput = 0f;
        clearTimer = 0f;
    }

    public void UpdateState(CarController car)
    {
        // Trzymaj auto w miejscu
        car.verticalInput = 0f;
        car.brakeInput = 1f;
        car.horizontalInput = 0f;

        bool blocked = IsFrontBlocked(car);

        if (!blocked)
        {
            // Droga siê zwolni³a - odliczamy czas bezpieczeñstwa
            clearTimer += Time.deltaTime;
            Debug.Log($"FSM [EMERGENCY]: Droga wolna, czekam {clearTimer:F1}s / {CLEAR_WAIT_SECONDS}s");

            if (clearTimer >= CLEAR_WAIT_SECONDS)
            {
                Debug.Log("FSM [EMERGENCY]: Wracam do przerwanego stanu!");
                car.ChangeState(interruptedState);
            }
        }
        else
        {
            // Przeszkoda wróci³a - resetuj odliczanie
            clearTimer = 0f;
        }
    }

    // Sprawdza czy coœ jest z przodu auta (zasiêg 3m)
    private bool IsFrontBlocked(CarController car)
    {
        Vector3 origin = car.transform.position + car.transform.forward * 2.25f;
        origin.y = car.transform.position.y + 0.3f;

        Vector3 dir = car.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 3.0f))
        {
            if (hit.collider.transform.IsChildOf(car.transform)) return false;
            Debug.DrawRay(origin, dir * hit.distance, Color.yellow);
            return true;
        }
        Debug.DrawRay(origin, dir * 3.0f, Color.green);
        return false;
    }

    public void Exit(CarController car)
    {
        Debug.Log("FSM [EMERGENCY]: Opuszczam stan awaryjny.");
        car.brakeInput = 0f;
    }
}