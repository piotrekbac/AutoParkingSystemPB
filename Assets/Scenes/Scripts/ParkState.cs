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
        float currentAngle = GetNormalizedAngle(car.transform.eulerAngles.y);       // Pobieramy aktualny k¹t obrotu samochodu wokó³ osi Y i normalizujemy go do zakresu -180 do 180 stopni, co u³atwia porównania k¹tów podczas parkowania.

        // Obslugujemy pocz¹tkow¹ fazê parkowania 
        if (parkingPhase == 0)
        {
            car.verticalInput = 1f;          // Ustawiamy wartoœæ wejœcia pionowego na 1, co oznacza, ¿e samochód bêdzie porusza³ siê do ty³u (bieg wsteczny). 
            car.horizontalInput = -0.3f;     // Ustawiamy wartoœæ wejœcia poziomego na -0.3, co oznacza, ¿e samochód bêdzie skrêca³ w lewo podczas poruszania siê do ty³u. Ta wartoœæ mo¿e byæ dostosowana w zale¿noœci od potrzeb i prefer
            car.breakInput = 0f;             // Ustawiamy wartoœæ wejœcia hamulca na 0, co oznacza, ¿e hamulec nie jest aktywowany podczas tej fazy parkowania. Samochód bêdzie porusza³ siê do ty³u bez hamowania.

            // Auto cofaj¹c ze skrêtem w prawo bêdzie obracaæ siê w lewo - kat staje siê ujemny 
            // Czekamy, a¿ odwróci sie o 40 stopni
            if (currentAngle <= -40f)
            {
                parkingPhase = 1;   // Przechodzimy do nastêpnej fazy parkowania, gdy samochód osi¹gnie k¹t -40 stopni, co oznacza, ¿e jest odpowiednio skrêcony w lewo podczas cofania. W tej fazie mo¿na dodaæ kolejn¹ logikê, np. kontynuowanie cofania, skrêt w prawo itp.
                Debug.Log("FSM: Auto jest pod k¹tem 40 stopni. Robie KONTRÊ KIEROWNIC¥!");      // Wypisujemy komunikat do konsoli, informuj¹cy o osi¹gniêciu k¹ta 40 stopni i koniecznoœci wykonania kontr-kierownicy, co oznacza, ¿e samochód jest odpowiednio skrêcony w lewo podczas cofania i teraz nale¿y wykonaæ skrêt w prawo
            }
        }

        // Sprawdzamy, czy samochód jest w fazie 1 parkowania, co oznacza, ¿e osi¹gn¹³ ju¿ k¹t -40 stopni podczas cofania. W tej fazie mo¿na dodaæ logikê, która bêdzie wykonywana, np. kontynuowanie cofania, skrêt w prawo itp. Na razie jest to puste, ale mo¿na je rozbudowaæ w zale¿noœci od potrzeb i preferencji dotycz¹cych manewru parkowania.
        else if (parkingPhase == 1)
        {
            // Faza 2: Wsuwamy przód auta (skrêt w lewo i jazda do ty³u)
            car.horizontalInput = -1f;      // Ustawiamy wartoœæ wejœcia poziomego na -1, co oznacza, ¿e samochód bêdzie skrêca³ maksymalnie w lewo podczas poruszania siê do ty³u. Ta wartoœæ mo¿e byæ dostosowana w zale¿noœci od potrzeb i prefer
        }
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

        // Jeœli k¹t jest wiêkszy ni¿ 180 stopni, to odejmujemy 360, aby uzyskaæ wartoœæ w zakresie -180 do 180 stopni. Dziêki temu ³atwiej bêdzie porównywaæ k¹ty podczas parkowania, poniewa¿
        // k¹t w lewo bêdzie reprezentowany jako ujemna wartoœæ, a k¹t w prawo jako dodatnia wartoœæ. Na przyk³ad, jeœli k¹t wynosi 350 stopni, to po normalizacji bêdzie
        // wynosi³ -10 stopni, co oznacza, ¿e samochód jest lekko skrêcony w lewo. Jeœli k¹t wynosi 10 stopni, to po normalizacji pozostanie 10 stopni, co oznacza, ¿e samochód jest lekko skrêcony w prawo.
        if (angle > 180f)
        {
            return angle - 360f;        // Normalizujemy k¹t do zakresu -180 do 180 stopni, co u³atwia obliczenia i porównania k¹tów podczas parkowania.
        }

        if (angle < -180f)
        {
            return angle + 360f;        // Normalizujemy k¹t do zakresu -180 do 180 stopni, co u³atwia obliczenia i porównania k¹tów podczas parkowania.
        }

        return angle;                   // Zwracamy znormalizowany k¹t, który jest teraz w zakresie -180 do 180 stopni, co u³atwia obliczenia i porównania k¹tów podczas parkowania.
    }
}
