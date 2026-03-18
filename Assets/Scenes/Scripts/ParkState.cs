using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    // Metoda Enter jest wywo³ywana, gdy samochód wchodzi w stan parkowania. W tej metodzie mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. zatrzymanie samochodu, ustawienie odpowiednich parametrów itp.
    public void Enter(CarController car)
    {
        // Wypisywanie komunikatu o rozpoczêciu manewru parkowania (bieg wsteczny) do konsoli
        Debug.Log("FSM: Zaczynam manewr parkowania (Bieg wsteczny)...");
    }

    // Metoda UpdateState - wywo³ywana w ka¿dej klatce, gdy samochód znajduje siê w stanie parkowania. Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. poruszanie siê do ty³u, skrêt itp.
    public void UpdateState(CarController car)
    {
        // Na razie tylko stoimy i trzymamy hamulec
        car.verticalInput = 0f;       // Ustawiamy wartoœæ wejœcia pionowego na 0, co oznacza, ¿e samochód bêdzie sta³ w miejscu, bez ruchu do przodu lub do ty³u.
        car.horizontalInput = 0f;     // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie sta³ w miejscu, bez skrêtu w lewo lub w prawo.
        car.breakInput = 1f;          // Ustawiamy wartoœæ wejœcia hamulca na 1, co oznacza, ¿e samochód bêdzie hamowa³ z pe³n¹ si³¹, co pozwoli mu zatrzymaæ siê w miejscu podczas parkowania.
    }

    // Metoda Exit jest wywo³ywana, gdy samochód opuszcza stan parkowania. Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas opuszczania tego stanu, np. przygotowanie samochodu do jazdy itp.
    public void Exit(CarController car)
    {
        
    }
}
