using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    private int parkingPhase = -1;
    private float timer = 0f;

    public void Enter(CarController car)
    {
        Debug.Log("FSM: Zaczynam manewr parkowania...");
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
            if (timer > 1.0f) parkingPhase = 0;
        }
        else if (parkingPhase == 0)
        {
            // FAZA 0: Wkrêcanie ty³u w lukê
            car.verticalInput = -0.5f;
            car.horizontalInput = 1f;
            car.brakeInput = 0f;

            // Z³amanie auta pod optymalnym k¹tem 35 stopni
            if (currentAngle <= -35f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: K¹t -35 stopni osi¹gniêty. Robiê KONTRE!");
            }
        }
        else if (parkingPhase == 1)
        {
            // FAZA 1: Prostowanie auta w luce
            car.horizontalInput = -1f;
            car.verticalInput = -0.5f;
            car.brakeInput = 0f;

            if (currentAngle >= -1f)
            {
                parkingPhase = 2;
                Debug.Log("FSM: Auto jest równolegle. Uruchamiam P-Controller (Wymóg z PDF) by wyœrodkowaæ!");
            }
        }
        else if (parkingPhase == 2)
        {
            // FAZA 2: P-Controller - WYMÓG Z PDF (strona 7)
            // Mechanizm wykorzystuj¹cy b³¹d pozycji do generowania p³ynnego sterowania przód/ty³
            car.horizontalInput = 0f; // Prostujemy kierownicê na amen

            float errorDistance = car.transform.position.z - car.targetParkingSpot.z;

            // Jeœli b³¹d (odleg³oœæ od idealnego œrodka) jest wiêkszy ni¿ 15 cm...
            if (Mathf.Abs(errorDistance) > 0.15f)
            {
                car.brakeInput = 0f;
                // P-Controller: Prêdkoœæ zale¿y od tego, jak daleko jesteœmy. 
                car.verticalInput = Mathf.Clamp(-errorDistance * 0.5f, -0.3f, 0.3f);
            }
            else
            {
                // Jesteœmy idealnie na œrodku luki!
                car.verticalInput = 0f;
                car.brakeInput = 1f;
                parkingPhase = 3;
                Debug.Log("FSM: ZAPARKOWANO PERFEKCYJNIE NA ŒRODKU! 100% ZADANIA WYKONANE!");
            }
        }
    }

    public void Exit(CarController car)
    { }

    private float GetNormalizedAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f) return angle - 360f;
        if (angle < -180f) return angle + 360f;
        return angle;
    }
}