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

    }
}
