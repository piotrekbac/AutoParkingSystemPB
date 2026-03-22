using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState_Perpendicular : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;
    private float phase0StartAngle = 0f;
    private const float REVERSE_SPEED = -0.35f;
    private const float CAR_HALF_LEN = 2.25f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania PROSTOPADŁEGO TYŁEM...");
        parkingPhase = -1;
        timer = 0f;
        phase0StartAngle = GetNorm(car.transform.eulerAngles.y);
    }

    public void UpdateState(CarController car)
    {
        float absAngle = GetNorm(car.transform.eulerAngles.y);
        float deltaPhase = GetNorm(absAngle - phase0StartAngle);

        if (parkingPhase == -1)
        {
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.brakeInput = 1f;
            timer += Time.deltaTime;

            if (timer > 1.0f)
            {
                parkingPhase = 0;
                timer = 0f;
                phase0StartAngle = GetNorm(car.transform.eulerAngles.y);
                Debug.Log("FSM Perp [Faza 0]: Cofam ze skrętem w prawo (tył w prawo = w lukę).");
            }
        }
        else if (parkingPhase == 0)
        {
            // Cofanie ze skrętem w PRAWO → tył idzie w PRAWO → w lukę
            car.verticalInput = REVERSE_SPEED;
            car.horizontalInput = 1f;
            car.brakeInput = 0f;

            // ZMIANA 1: Zwiększamy kąt z -83 na -89 stopni, żeby auto obróciło się równolegle do innych aut!
            if (deltaPhase <= -89f)
            {
                parkingPhase = 1;
                timer = 0f;
                Debug.Log($"FSM Perp [Faza 1]: {deltaPhase:F1}°. Aktywuję asystenta toru jazdy i cofam w głąb.");
            }
        }
        else if (parkingPhase == 1)
        {
            // ZMIANA 2: P-Controller dla Kierownicy! 
            // Zamiast ustawiać koła "na sztywno" na 0, auto samo pilnuje, żeby mieć równe 90 stopni od pozycji startowej.
            float targetAngle = GetNorm(phase0StartAngle - 90f);
            float headingError = GetNorm(absAngle - targetAngle);

            // Mikro-korekty: jeśli auto jest krzywo, lekko skręca by wyrównać tor jazdy.
            car.horizontalInput = Mathf.Clamp(headingError * 0.05f, -1f, 1f);
            car.verticalInput = REVERSE_SPEED;
            car.brakeInput = 0f;

            timer += Time.deltaTime;

            float rearDist = GetRearDist(car);
            float rightDist = GetRightDist(car);
            bool aligned = rightDist < 3.5f;

            if (rearDist < 1.0f || timer > 3.5f || aligned)
            {
                parkingPhase = 2;
                Debug.Log($"FSM Perp [Faza 2]: STOP! tył={rearDist:F2}m right={rightDist:F2}m t={timer:F1}s");
            }
        }
        else if (parkingPhase == 2)
        {
            car.horizontalInput = 0f;
            car.verticalInput = 0f;
            car.brakeInput = 1f;
            Debug.Log("FSM Perp: ZAPARKOWANO PROSTOPADLE TYŁEM! ✓");
        }
    }

    private float GetRearDist(CarController car)
    {
        Vector3 origin = car.transform.position - car.transform.forward * CAR_HALF_LEN;
        origin.y = car.transform.position.y + 0.3f;
        Vector3 dir = -car.transform.forward;
        dir.y = 0f; dir.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 4f))
        {
            if (hit.collider.transform.IsChildOf(car.transform)) return float.MaxValue;
            Debug.DrawRay(origin, dir * hit.distance, Color.magenta);
            return hit.distance;
        }
        Debug.DrawRay(origin, dir * 4f, Color.cyan);
        return float.MaxValue;
    }

    private float GetRightDist(CarController car)
    {
        Vector3 origin = car.transform.position + car.transform.right * 1.0f;
        origin.y = car.transform.position.y + 0.3f;
        Vector3 dir = car.transform.right;
        dir.y = 0f; dir.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, 5f))
        {
            if (hit.collider.transform.IsChildOf(car.transform)) return float.MaxValue;
            Debug.DrawRay(origin, dir * hit.distance, Color.yellow);
            return hit.distance;
        }
        Debug.DrawRay(origin, dir * 5f, Color.white);
        return float.MaxValue;
    }

    public void Exit(CarController car) { }

    private float GetNorm(float a)
    {
        a %= 360f;
        if (a > 180f) return a - 360f;
        if (a < -180f) return a + 360f;
        return a;
    }
}