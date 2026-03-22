using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class EmergencyState : ICarState
{
    public void Enter(CarController car)
    {
        Debug.LogWarning("FSM[HFSM]: HAMOWANIE AWARYJNE! Wykryto przeszkodê z przodu!");
    }

    public void UpdateState(CarController car)
    {
        // Zatrzymujemy auto w miejscu (zachowuj¹c ew. skrêt kó³)
        car.verticalInput = 0f;
        car.brakeInput = 1f;

        // Jeœli przeszkoda sobie posz³a (pieszy przeszed³) -> ZDEJMUJEMY STAN!
        if (!car.GetComponent<CarSensors>().isFrontObstacleDetected)
        {
            Debug.Log("FSM [HFSM]: Droga wolna. Zdejmujê stan awaryjny ze stosu.");
            car.PopState(); // Wracamy do parkowania!
        }
    }

    public void Exit(CarController car) { }
}