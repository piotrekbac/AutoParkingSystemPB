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

    // Flaga, która wskazuje, czy samochód min¹³ ju¿ pierwsz¹ przeszkodê (np. pierwszy samochód na parkingu) podczas poszukiwania miejsca parkingowego.
    // Ta flaga mo¿e byæ u¿ywana do okreœlenia, kiedy zacz¹æ mierzyæ lukê miêdzy samochodami, poniewa¿ chcemy zacz¹æ mierzyæ lukê dopiero po miniêciu
    // pierwszej przeszkody, aby unikn¹æ b³êdów w pomiarze luki spowodowanych przez pierwsz¹ przeszkodê.
    private bool hasPassedFirstObstacle = false; 

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
            // Obliczamy odleg³oœæ, jak¹ samochód przejecha³ od pozycji pocz¹tkowej (gapStartPosition) do aktualnej pozycji samochodu (car.transform.position) za pomoc¹ funkcji Vector3.Distance i przypisujemy j¹ do zmiennej distanceDrivenPastSpot.
            float distanceDrivenPastSpot = Vector3.Distance(gapStartPosition, car.transform.position) - requiredGapWidth;  

            // Podje¿d¿amy dodatkowe 2.5 metra do przodu, ¿eby zrównaæ siê z pojazdem, z przodu 
            if (distanceDrivenPastSpot < 2.5f)
            {
                car.verticalInput = 0.2f;       // Ustawiamy wartoœæ wejœcia pionowego na 0.2, co oznacza, ¿e samochód bêdzie jecha³ z 30% mocy silnika, aby podjechaæ do przodu i wyrównaæ siê z pojazdem z przodu.
                car.horizontalInput = 0f;       // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie jecha³ prosto, bez skrêtu, aby podjechaæ do przodu i wyrównaæ siê z pojazdem z przodu.
                car.breakInput = 0f;            // Ustawiamy wartoœæ wejœcia hamulca na 0, co oznacza, ¿e samochód nie bêdzie hamowa³, co pozwoli mu swobodnie poruszaæ siê do przodu i wyrównaæ siê z pojazdem z przodu.
            }

            // je¿eli przejechaliœmy ju¿ 2.5 metra, to zatrzymujemy samochód i przechodzimy do stanu parkowania (ParkState), poniewa¿ jesteœmy ju¿ wyrównani z pojazdem z przodu i mo¿emy rozpocz¹æ manewr parkowania.
            else
            {
                // Jesteœmy w idealnej pozycji startowej do "koperty" (parkowanie na kopertê)
                car.breakInput = 1f;                // Ustawiamy wartoœæ wejœcia hamulca na 1, co oznacza, ¿e samochód bêdzie hamowa³ z pe³n¹ si³¹, co pozwoli mu zatrzymaæ siê w miejscu, gotowy do rozpoczêcia manewru parkowania.
                car.verticalInput = 0f;             // Ustawiamy wartoœæ wejœcia pionowego na 0, co oznacza, ¿e samochód bêdzie sta³ w miejscu, bez ruchu do przodu lub do ty³u, gotowy do rozpoczêcia manewru parkowania.
                car.ChangeState(new ParkState());   // Zmieniamy stan samochodu na nowy obiekt ParkState, co oznacza, ¿e przechodzimy do stanu parkowania i rozpoczynamy manewr parkowania.
            }

            return;     // Je¿eli ju¿ znaleŸliœmy miejsce, to po wykonaniu powy¿szych dzia³añ (podjechanie do przodu i zatrzymanie siê) koñczymy aktualizacjê stanu i nie wykonujemy dalszej logiki poszukiwania miejsca parkingowego, poniewa¿ jesteœmy ju¿ gotowi do rozpoczêcia manewru parkowania.

        }

        // Odbieramy graczowi klawiaturê - AI bêdzie wciskaæ gaz na 30% mocy i jechaæ prosto
        car.verticalInput = 0.3f;       // Ustawiamy wartoœæ wejœcia pionowego na 0.3, co oznacza, ¿e samochód bêdzie jecha³ z 30% mocy silnika. 
        car.horizontalInput = 0f;       // Ustawiamy wartoœæ wejœcia poziomego na 0, co oznacza, ¿e samochód bêdzie jecha³ prosto, bez skrêtu
        car.breakInput = 0f;            // Ustawiamy wartoœæ wejœcia hamulca na 0, co oznacza, ¿e samochód nie bêdzie hamowa³, co pozwoli mu swobodnie poruszaæ siê podczas poszukiwania miejsca parkingowego.

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
                // Ustawiamy flagê hasPassedFirstObstacle na true, co oznacza, ¿e samochód min¹³ ju¿ pierwsz¹ przeszkodê
                // (np. pierwszy samochód na parkingu) podczas poszukiwania miejsca parkingowego.
                hasPassedFirstObstacle = true;    

                // Widzimy œcianê/inne auto 
                if (isMeasuringGap)
                {
                    // Wyœwietlam komunikat o zmierzeniu luki i jej dalszym poszukiwaniu
                    Debug.Log("FSM: Luka by³a za ma³a! Ignoruje i szukam dalej...");
                    isMeasuringGap = false;     // Reset flagi isMeasuringGap - nie mierzymy ju¿ luki bo jest za ma³a, wiêc mo¿emy szukaæ dalej.
                }
            }

            // Obs³uga przypadku, gdy nie wykryto przeszkody (isObstacleDetected jest false). W tym przypadku, jeœli nie wykryto przeszkody, to mo¿emy kontynuowaæ
            // jazdê prosto, poniewa¿ samochód jest w stanie poszukiwania miejsca parkingowego i mo¿e napotkaæ ró¿ne przeszkody na swojej drodze, które
            // musi omijaæ lub pokonywaæ, aby znaleŸæ odpowiednie miejsce do zaparkowania. W tym przypadku, jeœli nie wykryto przeszkody, to po
            // prostu kontynuujemy jazdê prosto, poniewa¿ samochód jest w stanie poszukiwania miejsca parkingowego i mo¿e napotkaæ ró¿ne przeszkody
            // na swojej drodze, które musi omijaæ lub pokonywaæ, aby znaleŸæ odpowiednie miejsce do zaparkowania.
            else
            {
                // Widzimy pust¹ przestrzeñ (laser jest zielony)
                if (!isMeasuringGap)
                {
                    isMeasuringGap = true;                                                  // Ustawiamy flagê isMeasuringGap na true, co oznacza, ¿e zaczynamy mierzyæ lukê miêdzy samochodami, poniewa¿ wykryliœmy pust¹ przestrzeñ (laser jest zielony).
                    gapStartPosition = car.transform.position;                              // Ustawiamy gapStartPosition na aktualn¹ pozycjê samochodu, co oznacza, ¿e zaczynamy mierzyæ lukê od tej pozycji, poniewa¿ wykryliœmy pust¹ przestrzeñ (laser jest zielony).
                    Debug.Log("FSM: Zauwa¿ono pocz¹tek luki! Rozpoczynam pomiar...");       // Wyœwietlam komunikat o zauwa¿eniu pocz¹tku luki i rozpoczêciu pomiaru, co oznacza, ¿e zaczynamy mierzyæ lukê miêdzy samochodami, poniewa¿ wykryliœmy pust¹ przestrzeñ (laser jest zielony).
                }

                // Obs³ugujemy przypadek, gdy mierzymy lukê isMeasuringGap jest true, co oznacza, ¿e zaczêliœmy mierzyæ lukê miêdzy samochodami
                else
                {
                    // Samochód jedzie zd³u¿ luki, a my mierzymy odleg³oœæ od jej pocz¹tku

                    // Obliczamy aktualn¹ szerokoœæ luki miêdzy samochodami, mierz¹c odleg³oœæ miêdzy pozycj¹ pocz¹tkow¹ (gapStartPosition) a aktualn¹ pozycj¹ samochodu (car.transform.position) za pomoc¹ funkcji Vector3.Distance.
                    float currentGapWidth = Vector3.Distance(gapStartPosition, car.transform.position);

                    // Obs³uga przypadku, gdy aktualna szerokoœæ luki (currentGapWidth) jest wiêksza lub równa wymaganej szerokoœci luki (requiredGapWidth). Jeœli aktualna szerokoœæ luki jest wystarczaj¹ca, to mo¿emy uznaæ, ¿e znaleŸliœmy odpowiednie miejsce parkingowe i podj¹æ odpowiednie dzia³ania, np. zatrzymaæ siê, zmieniæ stan na parkowanie itp.
                    if (currentGapWidth >= requiredGapWidth)
                    {
                        // Wyœwietlam komunikat o sukcesie znalezienia idealnego miejsca parkingowego, wraz z aktualn¹ szerokoœci¹ luki (currentGapWidth) sformatowan¹ do dwóch miejsc po przecinku, co oznacza, ¿e znaleŸliœmy odpowiednie miejsce parkingowe i mo¿emy zatrzymaæ samochód.
                        Debug.Log($"FSM: SUKCES! Znalaz³em idealne miejsce (Szerokoœæ: {currentGapWidth:F2}m). Zatrzymujê auto!");

                        spotFound = true;    // Ustawiamy flagê spotFound na true, co oznacza, ¿e znaleŸliœmy odpowiednie miejsce parkingowe (lukê miêdzy samochodami) podczas poszukiwania.

                    }
                }
            }
        }

        // Obs³uga przypadku, gdy komponent CarSensors jest null. W tym przypadku, jeœli nie uda³o siê pobraæ komponentu CarSensors, to mo¿emy wyœwietliæ komunikat o b³êdzie lub podj¹æ inne odpowiednie dzia³ania, np. zatrzymaæ samochód, itp. W tym przypadku, jeœli nie uda³o siê pobraæ komponentu CarSensors, to po prostu nic nie robimy, poniewa¿ nie mamy informacji o wykrytych przeszkodach i odleg³oœci do nich, które s¹ potrzebne do podejmowania decyzji dotycz¹cych poruszania siê samochodu podczas poszukiwania miejsca parkingowego.
        else
        {

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
