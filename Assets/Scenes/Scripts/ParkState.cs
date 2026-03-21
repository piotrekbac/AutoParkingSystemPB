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
            if (timer > 1.0f)
            {
                parkingPhase = 0;
                timer = 0f; // Resetujemy timer dla nowej fazy Twojego pomys³u!
                Debug.Log("FSM: Zaczynam cofaæ na wprost (Faza dodana przez Piotra!)...");
            }
        }
        else if (parkingPhase == 0)
        {
            // FAZA 0 (TWÓJ POMYS£): Cofamy na prostych ko³ach, ¿eby schowaæ zderzak!
            car.verticalInput = -0.4f;       // Powolne cofanie
            car.horizontalInput = 0f;        // KIEROWNICA PROSTO!
            car.brakeInput = 0f;

            timer += Time.deltaTime;
            // Cofamy prosto przez 1 sekundê (to wystarczy, by min¹æ róg auta z przodu)
            if (timer > 1.0f)
            {
                parkingPhase = 1;
                Debug.Log("FSM: Ty³ bezpieczny! £amiê auto w prawo.");
            }
        }
        else if (parkingPhase == 1)
        {
            // FAZA 1: Wkrêcanie ty³u w lukê (skrêt w prawo)
            car.verticalInput = -0.5f;
            car.horizontalInput = 1f;        // MAX W PRAWO
            car.brakeInput = 0f;

            if (currentAngle <= -35f)
            {
                parkingPhase = 2;
                Debug.Log("FSM: K¹t -35 stopni osi¹gniêty. Robiê KONTRE w lewo!");
            }
        }
        else if (parkingPhase == 2)
        {
            // FAZA 2: Prostowanie auta w luce
            car.horizontalInput = -1f;      // MAX W LEWO
            car.verticalInput = -0.5f;
            car.brakeInput = 0f;

            if (currentAngle >= -1f)
            {
                parkingPhase = 3;
                Debug.Log("FSM: Auto jest równolegle. Uruchamiam P-Controller by wyœrodkowaæ!");
            }
        }
        else if (parkingPhase == 3)
        {
            // FAZA 3: P-Controller - WYMÓG Z PDF
            car.horizontalInput = 0f; // Prostujemy kierownicê na amen

            float errorDistance = car.transform.position.z - car.targetParkingSpot.z;

            if (Mathf.Abs(errorDistance) > 0.15f)
            {
                car.brakeInput = 0f;
                car.verticalInput = Mathf.Clamp(-errorDistance * 0.5f, -0.3f, 0.3f);
            }
            else
            {
                car.verticalInput = 0f;
                car.brakeInput = 1f;
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