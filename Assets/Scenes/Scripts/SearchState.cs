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
        
    }
}
