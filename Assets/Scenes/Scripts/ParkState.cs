using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    // Prywatna zmienna parkingPhase, która mo¿e byæ u¿ywana do œledzenia fazy parkowania. Mo¿na j¹ wykorzystaæ do implementacji ró¿nych etapów manewru parkowania, takich jak ustawianie samochodu, skrêt itp. Na razie jest ustawiona na 0, co oznacza, ¿e nie ma jeszcze zdefiniowanych faz parkowania.
    private int parkingPhase = 0;

    // Metoda Enter jest wywo³ywana, gdy samochód wchodzi w stan parkowania. W tej metodzie mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. zatrzymanie samochodu, ustawienie odpowiednich parametrów itp.
    public void Enter(CarController car)
    {
        // Wypisywanie komunikatu o rozpoczêciu manewru parkowania (bieg wsteczny) do konsoli
        Debug.Log("FSM: Zaczynam manewr parkowania (Bieg wsteczny)...");

        // Upewniamy siê, ¿e startujemy od fazy 0
        parkingPhase = 0;   
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
        // Wypisywanie komunikatu o zakoñczeniu manewru parkowania do konsoli
        Debug.Log("FSM: Zakoñczy³em manewr parkowania.");
    }

    // Funkcja pomocnicza - o wiele ³atwiej liczy siê, gdy k¹t w lewo to np. -40 stopni, a prawo to +40 stopni. 
    // Unity z nautry podaje k¹ty jako 0 do 360 stopni - dlatego t¹ metod¹ u³atwiamy sobie ¿ycie oraz obliczenia
    private float GetNormalizedAngle(float angle)
    {
        angle = angle % 360;    // Upewniamy siê, ¿e k¹t jest w zakresie 0-360 stopni
    }
}
