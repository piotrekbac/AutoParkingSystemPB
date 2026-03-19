using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class ParkState : ICarState
{
    // Prywatna zmienna parkingPhase, która mo¿e byæ u¿ywana do œledzenia fazy parkowania. Mo¿na j¹ wykorzystaæ do implementacji ró¿nych etapów manewru parkowania, takich jak ustawianie samochodu, skrêt itp. Na razie jest ustawiona na 0, co oznacza, ¿e nie ma jeszcze zdefiniowanych faz parkowania.
    private int parkingPhase = -1;

    // Prywatna zmienna timer, która mo¿e byæ u¿ywana do œledzenia czasu spêdzonego w danym stanie parkowania. Mo¿na j¹ wykorzystaæ do implementacji opóŸnieñ, czasowych warunków przejœcia do innych stanów itp. Na razie jest ustawiona na 0, co oznacza, ¿e nie ma jeszcze zdefiniowanych opóŸnieñ ani warunków czasowych dla tego stanu parkowania.
    private float timer = 0f;

    // Metoda Enter jest wywo³ywana, gdy samochód wchodzi w stan parkowania. W tej metodzie mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. zatrzymanie samochodu, ustawienie odpowiednich parametrów itp.s
    public void Enter(CarController car)
    {
        // Wypisywanie komunikatu o rozpoczêciu manewru parkowania (bieg wsteczny) do konsoli
        Debug.Log("FSM: Zaczynam manewr parkowania (Bieg wsteczny)...");

        // Upewniamy siê, ¿e startujemy od fazy -1
        parkingPhase = -1;

        // Resetujemy timer, jeœli chcemy go u¿ywaæ do œledzenia czasu spêdzonego w stanie parkowania. Na razie jest to tylko przygotowanie do ewentualnego wykorzystania tego timera w przysz³oœci, np. do implementacji opóŸnieñ czy warunków czasowych dla tego stanu parkowania.
        timer = 0f;
    }

    // Metoda UpdateState - wywo³ywana w ka¿dej klatce, gdy samochód znajduje siê w stanie parkowania. Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. poruszanie siê do ty³u, skrêt itp.
    public void UpdateState(CarController car)
    {
        float currentAngle = GetNormalizedAngle(car.transform.eulerAngles.y);       // Pobieramy aktualny k¹t obrotu samochodu wokó³ osi Y i normalizujemy go do zakresu -180 do 180 stopni, co u³atwia porównania k¹tów podczas parkowania.

        // Sprawdzamy, czy samochód jest w fazie -1 parkowania, co oznacza, ¿e nie zosta³ jeszcze zdefiniowany etap parkowania. W tej fazie mo¿na dodaæ logikê, która bêdzie wykonywana, np. przygotowanie samochodu do parkowania, ustawienie odpowiednich parametrów itp. Na razie jest to puste, ale mo¿na je rozbudowaæ w zale¿noœci od potrzeb i preferencji dotycz¹cych manewru parkowania.
        if (parkingPhase == -1)
        {
             // Faza -1: Pe³ne zatrzymanie (Zabijamy pêd samochodu) 

        }


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
            car.verticalInput = -0.3f;      // Ustawiamy wartoœæ wejœcia pionowego na -0.3, co oznacza, ¿e samochód bêdzie porusza³ siê do przodu (bieg do przodu) z mniejsz¹ prêdkoœci¹. Ta wartoœæ mo¿e byæ dostosowana w zale¿noœci od potrzeb i prefer
            car.breakInput = 0f;            // Ustawiamy wartoœæ wejœcia hamulca na 0

            // Auto zaczyna siê prostowaæ - wiêc k¹t wraca z -40 stopni, z powrotem do 0 stopni. 
            // Sprawdzamy czy k¹t jest bardzo blisko zera (idealnie prosto z drog¹)
            if (currentAngle >= -2f || currentAngle > 0f)
            {
                parkingPhase = 2;   // Przechodzimy do nastêpnej fazy parkowania, gdy samochód osi¹gnie k¹t bliski 0 stopni, co oznacza, ¿e jest prawie prosto wzglêdem drogi. W tej fazie mo¿na dodaæ kolejn¹ logikê, np. kontynuowanie jazdy do przodu, skrêt w lewo itp.
                Debug.Log("FSM: SUKCES! Koñczê manewr parkowania!");      // Wypisujemy komunikat do konsoli, informuj¹cy o osi¹gniêciu k¹ta bliskiego 0 stopni i zakoñczeniu manewru parkowania, co oznacza, ¿e samochód jest prawie prosto wzglêdem drogi i teraz
            }
        }

        // Sprawdzamy, czy samochód jest w fazie 2 parkowania, co oznacza, ¿e osi¹gn¹³ ju¿ k¹t bliski 0 stopni podczas cofania. W tej fazie mo¿na dodaæ logikê, która bêdzie wykonywana, np. kontynuowanie jazdy do przodu, skrêt w lewo itp. Na razie jest to puste, ale mo¿na je rozbudowaæ w zale¿noœci od potrzeb i preferencji dotycz¹cych manewru parkowania.
        else if (parkingPhase == 2)
        {
            // Faza 3 - Auto jest ju¿ prosto, wiêc mo¿emy zakoñczyæ manewr parkowania - ustawiamy wszystkie wejœcia na 0, aby zatrzymaæ samochód
            car.horizontalInput = 0f;     // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód nie bêdzie skrêca³ podczas tej fazy parkowania. Samochód bêdzie porusza³ siê prosto do przodu lub do ty³u, w zale¿noœci od ustawienia wejœcia pionowego.
            car.verticalInput = 0f;       // Ustawiamy wartoœæ wejœcia pionowego na 0, co oznacza, ¿e samochód nie bêdzie porusza³ siê do przodu ani do ty³u podczas tej fazy parkowania. Samochód bêdzie zatrzymany.
            car.breakInput = 1f;          // Ustawiamy wartoœæ wejœcia hamulca na 1, co oznacza, ¿e hamulec jest aktywowany podczas tej fazy parkowania. Samochód bêdzie zatrzymany i nie bêdzie siê porusza³, co jest odpowied
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
