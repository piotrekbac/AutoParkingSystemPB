using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

// Nasza klasa SearchState dziedziczy po interfejsie ICarState
public class SearchState : ICarState
{
    // Implementacja metody Enter z interfejsu ICarState. Ta metoda jest wywo³ywana, gdy samochód wchodzi w stan poszukiwania miejsca parkingowego.
    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");
    }

    // Implementacja metody UpdateState z interfejsu ICarState.
    // Ta metoda jest wywo³ywana w ka¿dej klatce, gdy samochód znajduje siê w stanie poszukiwania miejsca parkingowego.
    // Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. poruszanie siê po parkingu, skanowanie otoczenia itp.
    public void UpdateState(CarController car)
    {
        // 1. Odbieramy graczowi klawiaturê - AI bêdzie wciskaæ gaz na 30% mocy i jechaæ prosto
        car.verticalInput = 0.3f;  // Ustawiamy wartoœæ wejœcia pionowego na 0.3, co oznacza, ¿e samochód bêdzie jecha³ z 30% mocy silnika. 
        car.horizontalInput = 0f;  // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie jecha³ prosto, bez skrêtu.


        // 2. Pobieranie informacji naszego lasers 
        CarSensors sensors = car.GetComponent<CarSensors>();    // Pobieramy komponent CarSensors, który jest przypisany do samochodu

    }
}
