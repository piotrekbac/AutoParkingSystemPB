using UnityEngine;
// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana
public class ParkState : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;
    private float entryAngle = 0f;

    // Minimalna odległość tylnego zderzaka od przeszkody zanim zatrzymamy cofanie
    // Zwiększ jeśli nadal uderza, zmniejsz jeśli zatrzymuje się za wcześnie
    private const float REAR_STOP_DISTANCE = 0.8f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania...");
        parkingPhase = -1;
        timer = 0f;
        entryAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
        Debug.Log($"FSM ParkState: Kąt wejścia = {entryAngle:F1}°");
    }

    public void UpdateState(CarController car)
    {
        float absoluteAngle = GetNormalizedAngle(car.transform.eulerAngles.y);
        float deltaAngle = GetNormalizedAngle(absoluteAngle - entryAngle);

        // CZUJNIK TYLNY - działa we wszystkich fazach cofania (0, 1, 2)
        // Rzuca trzy promienie z tylnego zderzaka: środek, lewy róg, prawy róg
        // Reaguje na NAJKRÓTSZY dystans - niezależnie od kąta auta
        if (parkingPhase >= 0 && parkingPhase <= 2)
        {
            float rearDist = GetRearDistance(car);
            if (rearDist < REAR_STOP_DISTANCE)
            {
                car.verticalInput = 0f;
                car.brakeInput = 1f;
                car.horizontalInput = 0f;
                Debug.LogWarning($"FSM: STOP! Tył {rearDist:F2}m od ściany. Przechodzę do kontry.");
                // Nie przerywamy - przechodzimy do fazy 2 (kontra) żeby auto nie stało w poprzek
                if (parkingPhase == 1)
                {
                    parkingPhase = 2;
                    Debug.Log("FSM: Wymuszam przejście do fazy 2 (kontra).");
                }
                else if (parkingPhase == 0 || parkingPhase == 2)
                {
                    parkingPhase = 3;
                    Debug.Log("FSM: Wymuszam przejście do fazy 3 (P-Controller).");
                }
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
        // FAZA 0: Cofanie na wprost
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
            // Czujnik prawego boku przodu - ochrona przed pierwszym blokiem
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
                Debug.Log($"FSM: [Faza 3] Równolegle delta={deltaAngle:F1}. P-Controller!");
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

    /// Trzy raycaste z tylnego zderzaka: środek + lewy róg + prawy róg.
    /// Zwraca najkrótszy dystans - niezależnie od kąta skrętu.
    private float GetRearDistance(CarController car)
    {
        Vector3 back = -car.transform.forward;
        back.y = 0f;
        back.Normalize();
        float y = car.transform.position.y + 0.3f;

        // Trzy punkty startowe: środek tylnego zderzaka, lewy róg, prawy róg
        Vector3 centerOrigin = car.transform.position - car.transform.forward * 2.25f;
        Vector3 leftOrigin = centerOrigin - car.transform.right * 0.7f;
        Vector3 rightOrigin = centerOrigin + car.transform.right * 0.7f;
        centerOrigin.y = leftOrigin.y = rightOrigin.y = y;

        float distCenter = Raycast(centerOrigin, back, 2.0f, car, Color.magenta);
        float distLeft = Raycast(leftOrigin, back, 2.0f, car, Color.magenta);
        float distRight = Raycast(rightOrigin, back, 2.0f, car, Color.magenta);

        return Mathf.Min(distCenter, Mathf.Min(distLeft, distRight));
    }

    /// Raycast z prawego przedniego rogu - ochrona przed pierwszym blokiem.
    private float GetFrontRightDistance(CarController car)
    {
        Vector3 origin = car.transform.position
                         + car.transform.forward * 2.25f
                         + car.transform.right * 0.9f;
        origin.y = car.transform.position.y + 0.3f;
        Vector3 direction = car.transform.right;
        direction.y = 0f;
        direction.Normalize();
        return Raycast(origin, direction, 1.5f, car, Color.red);
    }

    /// Pomocnicza metoda - rzuca jeden raycast i zwraca dystans (float.MaxValue = brak hitu).
    private float Raycast(Vector3 origin, Vector3 direction, float length, CarController car, Color color)
    {
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, length))
        {
            if (hit.collider.transform.IsChildOf(car.transform)) return float.MaxValue;
            Debug.DrawRay(origin, direction * hit.distance, color);
            return hit.distance;
        }
        Debug.DrawRay(origin, direction * length, Color.white);
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