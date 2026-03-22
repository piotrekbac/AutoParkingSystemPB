using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania RÓWNOLEGŁEGO...");
        parkingPhase = -1;
        timer = 0f;
    }

    public void UpdateState(CarController car)
    {
        float currentAngle = GetNormalizedAngle(car.transform.eulerAngles.y);

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
                Debug.Log("FSM: Zaczynam cofać na wprost (Faza 0).");
            }
        }
        else if (parkingPhase == 0)
        {
            // FAZA 0: Cofamy na prostych kołach
            car.verticalInput = -0.4f;
            car.horizontalInput = 0f;
            car.brakeInput = 0f;

            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: Tył auta wsunął się za róg. Łamię auto w prawo (Faza 1).");
            }
        }
        else if (parkingPhase == 1)
        {
            // FAZA 1: Wkręcanie tyłu w lukę
            car.verticalInput = -0.5f;
            car.horizontalInput = 1f;
            car.brakeInput = 0f;

            if (currentAngle <= -35f)
            {
                parkingPhase = 2;
                Debug.Log("FSM: Kąt osiągnięty. Robię KONTRE w lewo! (Faza 2).");
            }
        }
        else if (parkingPhase == 2)
        {
            // FAZA 2: Prostowanie auta w luce
            car.horizontalInput = -1f;
            car.verticalInput = -0.5f;
            car.brakeInput = 0f;

            if (currentAngle >= -1f)
            {
                parkingPhase = 3;
                Debug.Log("FSM: Auto jest prosto. Zatrzymuję! (Faza 3).");
            }
        }
        else if (parkingPhase == 3)
        {
            // FAZA 3: Zatrzymanie auta
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