using UnityEngine;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public class ParkState : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;
    private float entryAngle = 0f;

    // Łączny czas cofania w fazach 0+1+2 - twardy limit zapobiegający uderzeniu w tylną ścianę
    private float totalReverseTime = 0f;
    private const float MAX_REVERSE_TIME = 5.5f; // sekund cofania maksymalnie

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania...");
        parkingPhase = -1;
        timer = 0f;
        totalReverseTime = 0f;
        entryAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
        Debug.Log($"FSM ParkState: Kąt wejścia = {entryAngle:F1}°");
    }

    public void UpdateState(CarController car)
    {
        float absoluteAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
        float deltaAngle = GetNormalizedAngle(absoluteAngle - entryAngle);

        // Liczymy łączny czas cofania (fazy 0, 1, 2) - twardy limit antykolizyjny
        if (parkingPhase >= 0 && parkingPhase <= 2)
        {
            totalReverseTime += Time.deltaTime;
            if (totalReverseTime > MAX_REVERSE_TIME)
            {
                // Za długo cofamy - wymuś kontrę i zatrzymaj
                car.verticalInput = 0f;
                car.brakeInput = 1f;
                parkingPhase = 3;
                Debug.LogWarning("FSM: Limit czasu cofania! Przechodzę do P-Controllera.");
                return;
            }
        }

        // FAZA -1: Pełny stop przed manewrem
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
                Debug.Log("FSM: [Faza 0] Cofam na wprost.");
            }
        }
        // FAZA 0: Cofanie na wprost - wydłużono z 1.5s do 2.2s
        else if (parkingPhase == 0)
        {
            car.verticalInput = -0.3f;
            car.horizontalInput = 0f;
            car.brakeInput = 0f;
            timer += Time.deltaTime;
            if (timer > 1.8f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: [Faza 1] Tył wysunięty. Skręt w prawo!");
            }
        }
        // FAZA 1: Cofanie ze skrętem w prawo
        else if (parkingPhase == 1)
        {
            // Czujnik prawego boku przodu - jeśli ściana za blisko, cofamy chwilę prosto
            float frontRightDist = GetFrontRightDistance(car);
            if (frontRightDist < 0.5f)
            {
                car.verticalInput = -0.2f;
                car.horizontalInput = 0f;
                car.brakeInput = 0f;
                Debug.LogWarning($"FSM: Prawy przód za blisko ({frontRightDist:F2}m)! Cofam prosto.");
                return;
            }

            car.verticalInput = -0.35f;
            car.horizontalInput = 1f;
            car.brakeInput = 0f;
            if (deltaAngle <= -28f)
            {
                parkingPhase = 2;
                Debug.Log($"FSM: [Faza 2] Delta={deltaAngle:F1}. Kontra w lewo!");
            }
        }
        // FAZA 2: Kontra - prostowanie auta w luce
        else if (parkingPhase == 2)
        {
            car.horizontalInput = -1f;
            car.verticalInput = -0.35f;
            car.brakeInput = 0f;
            if (deltaAngle >= -3f)
            {
                parkingPhase = 3;
                Debug.Log($"FSM: [Faza 3] Rownolegle delta={deltaAngle:F1}. P-Controller!");
            }
        }
        // FAZA 3: P-Controller - wyśrodkowanie wzdłuż lokalnej osi pojazdu
        else if (parkingPhase == 3)
        {
            car.horizontalInput = 0f;
            Vector3 toTarget = car.targetParkingSpot - car.transform.position;
            float errorDistance = Vector3.Dot(toTarget, car.transform.forward);
            if (Mathf.Abs(errorDistance) > 0.15f)
            {
                car.brakeInput = 0f;
                car.verticalInput = Mathf.Clamp(errorDistance * 0.5f, -0.3f, 0.3f);
            }
            else
            {
                car.verticalInput = 0f;
                car.brakeInput = 1f;
                Debug.Log("FSM: ZAPARKOWANO PERFEKCYJNIE!");
            }
        }
    }

    // Raycast z prawego przedniego rogu - wykrywa ścianę pierwszego bloku podczas skrętu
    private float GetFrontRightDistance(CarController car)
    {
        Vector3 origin = car.transform.position
                         + car.transform.forward * 2.25f
                         + car.transform.right * 0.9f;
        origin.y = car.transform.position.y + 0.3f;
        Vector3 direction = car.transform.right;
        direction.y = 0f;
        direction.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 1.5f))
        {
            if (hit.collider.transform.IsChildOf(car.transform)) return float.MaxValue;
            Debug.DrawRay(origin, direction * hit.distance, Color.red);
            return hit.distance;
        }
        Debug.DrawRay(origin, direction * 1.5f, Color.white);
        return float.MaxValue;
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