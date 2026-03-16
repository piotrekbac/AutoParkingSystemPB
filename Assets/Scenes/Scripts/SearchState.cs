using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

// Nasza klasa SearchState dziedziczy po interfejsie ICarState
public class SearchState : ICarState
{
    // Tworzê zmienn¹ typu CarSensors, która bêdzie przechowywaæ referencjê do komponentu CarSensors przypisanego do samochodu.
    // Ten komponent bêdzie odpowiedzialny za dostarczanie informacji o wykrytych przeszkodach i odleg³oœci do nich, co pozwoli na podejmowanie
    // decyzji dotycz¹cych poruszania siê samochodu podczas poszukiwania miejsca parkingowego.
    private CarSensors sensors;


    // Zmienne do mierzenia luki 
    private bool isMeasuringGap = false;        // Flaga, która wskazuje, czy aktualnie mierzymy lukê miêdzy samochodami.
    private Vector3 gapStartPosition;           // Zmienna do przechowywania pozycji pocz¹tkowej, od której zaczynamy mierzyæ lukê miêdzy samochodami.
    private float requiredGapWidth = 3.0f;      // Zmienna okreœlaj¹ca wymagan¹ szerokoœæ luki miêdzy samochodami, która jest potrzebna do zaparkowania. 
    private bool spotFound = false;             // Flaga, która wskazuje, czy znaleŸliœmy odpowiednie miejsce parkingowe (lukê miêdzy samochodami) podczas poszukiwania.

    // Implementacja metody Enter z interfejsu ICarState. Ta metoda jest wywo³ywana, gdy samochód wchodzi w stan poszukiwania miejsca parkingowego.
    public void Enter(CarController car)
    {
        Debug.Log("FSM: Rozpoczynam poszukiwanie miejsca...");

        // Pobieramy komponent CarSensors, który jest przypisany do samochodu, i przypisujemy go do zmiennej sensors.
        // Dziêki temu bêdziemy mogli korzystaæ z informacji o wykrytych przeszkodach i odleg³oœci do nich podczas aktualizacji stanu w metodzie UpdateState.
        sensors = car.GetComponent<CarSensors>();    
    }

    // Implementacja metody UpdateState z interfejsu ICarState.
    // Ta metoda jest wywo³ywana w ka¿dej klatce, gdy samochód znajduje siê w stanie poszukiwania miejsca parkingowego.
    // Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas tego stanu, np. poruszanie siê po parkingu, skanowanie otoczenia itp.
    public void UpdateState(CarController car)
    {
        // Je¿eli ju¿ znaleŸliœmy miejsce, nic wiêcej nie robimy - tylko stoimy 
        if (spotFound)
        {
            car.verticalInput = 0f;         // Ustawiamy wartoœæ wejœcia pionowego na 0, co oznacza, ¿e samochód bêdzie sta³ w miejscu, bez ruchu do przodu lub do ty³u.
            car.horizontalInput = 0f;       // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie sta³ w miejscu, bez skrêtu w lewo lub w prawo.
        }

        // Odbieramy graczowi klawiaturê - AI bêdzie wciskaæ gaz na 30% mocy i jechaæ prosto
        car.verticalInput = 0.3f;  // Ustawiamy wartoœæ wejœcia pionowego na 0.3, co oznacza, ¿e samochód bêdzie jecha³ z 30% mocy silnika. 
        car.horizontalInput = 0f;  // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie jecha³ prosto, bez skrêtu


        // Warunek logiczny - sprawdzamy, czy komponent CarSensors zosta³ poprawnie pobrany (nie jest null). Jeœli tak, to mo¿emy uzyskaæ dostêp do informacji
        // o wykrytych przeszkodach i odleg³oœci do nich, które s¹ przechowywane w zmiennych isObstacleDetected i currentDistanceToObstacle.
        // Na podstawie tych informacji mo¿na podejmowaæ decyzje dotycz¹ce dalszego poruszania siê samochodu, np. zatrzymanie siê przed przeszkod¹,
        // skrêt w innym kierunku itp.
        if (sensors != null)
        {
            // Je¿eli wykryto przeszkodê (isObstacleDetected jest true), to mo¿emy podj¹æ odpowiednie dzia³ania, np. zatrzymaæ siê, skrêciæ w innym
            // kierunku, itp. W tym przypadku, jeœli wykryto przeszkodê, to po prostu kontynuujemy jazdê prosto, poniewa¿ samochód jest w stanie
            // poszukiwania miejsca parkingowego i mo¿e napotkaæ ró¿ne przeszkody na swojej drodze, które musi omijaæ lub pokonywaæ, aby znaleŸæ
            // odpowiednie miejsce do zaparkowania.
            if (sensors.isObstacleDetected)
            {
                // Widzimy œcianê/inne auto 
                if (isMeasuringGap)
                {

                }
            }

            // Obs³uga przypadku, gdy nie wykryto przeszkody (isObstacleDetected jest false). W tym przypadku, jeœli nie wykryto przeszkody, to mo¿emy kontynuowaæ
            // jazdê prosto, poniewa¿ samochód jest w stanie poszukiwania miejsca parkingowego i mo¿e napotkaæ ró¿ne przeszkody na swojej drodze, które
            // musi omijaæ lub pokonywaæ, aby znaleŸæ odpowiednie miejsce do zaparkowania. W tym przypadku, jeœli nie wykryto przeszkody, to po
            // prostu kontynuujemy jazdê prosto, poniewa¿ samochód jest w stanie poszukiwania miejsca parkingowego i mo¿e napotkaæ ró¿ne przeszkody
            // na swojej drodze, które musi omijaæ lub pokonywaæ, aby znaleŸæ odpowiednie miejsce do zaparkowania.
            else
            {
                Debug.Log("FSM: Nie wykryto przeszkody, kontynuujemy jazdê prosto...");
            }
        }    
    }

    // Implementacja metody Exit z interfejsu ICarState. Ta metoda jest wywo³ywana, gdy samochód opuszcza stan poszukiwania miejsca parkingowego.
    public void Exit(CarController car)
    {
        // Ta metoda jest wywo³ywana, gdy samochód opuszcza stan poszukiwania miejsca parkingowego.
        // Tutaj mo¿na dodaæ logikê, która bêdzie wykonywana podczas opuszczania tego stanu, np. resetowanie parametrów, zatrzymanie samochodu itp.
        Debug.Log("FSM: Zakoñczy³em poszukiwanie miejsca.");
    }
}
