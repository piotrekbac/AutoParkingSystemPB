using System;
using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarController : MonoBehaviour
{
    /* 
     Tworzê skrypt, który bêdzie odpowiada³ za sterowanie samochodem.
     Bêdzie on wykorzystywa³ komponent Rigidbody do poruszania siê
     samochodu oraz Transform do ustawiania œrodka masy. 
    */

    // Deklarujê nag³ówek dla sekcji referencji do komponentów, które bêd¹ u¿ywane w skrypcie.
    [Header("Referencje - Wheel Colidery (Fizyka kó³)")]

    public WheelCollider frontLeftCollider;     // Referencja do Wheel Collidera przedniego lewego ko³a.
    public WheelCollider frontRightCollider;    // Referencja do Wheel Collidera przedniego prawego ko³a.
    public WheelCollider rearLeftCollider;      // Referencja do Wheel Collidera tylnego lewego ko³a.
    public WheelCollider rearRightCollider;     // Referencja do Wheel Collidera tylnego prawego ko³a.


    // Deklarujê nag³ówek dla sekcji referencji do modeli 3D kó³, które bêd¹ u¿ywane do wizualizacji kó³ samochodu.
    [Header("Referencje - Modele 3D kó³ (Wizualizacja kó³)")]

    public Transform frontLeftVisual;     // Referencja do Transformu modelu 3D przedniego lewego ko³a (do wizualizacji).
    public Transform frontRightVisual;    // Referencja do Transformu modelu 3D przedniego prawego ko³a (do wizualizacji).
    public Transform rearLeftVisual;      // Referencja do Transformu modelu 3D tylnego lewego ko³a (do wizualizacji).
    public Transform rearRightVisual;     // Referencja do Transformu modelu 3D tylnego prawego ko³a (do wizualizacji).

    // Deklarujê nag³ówek dla sekcji ustawieñ pojazdu, które bêd¹ u¿ywane do konfiguracji fizyki samochodu.
    [Header("Ustawienia pojazdu")]

    public Transform centerOfMass;      // Referencja do Transformu, który bêdzie okreœla³ œrodek masy samochodu.
    public float motorForce = 1500f;    // Si³a silnika, która bêdzie u¿ywana do napêdzania samochodu.
    public float maxSteerAngle = 35f;   // Maksymalny k¹t skrêtu kó³, który bêdzie u¿ywany do sterowania samochodem.
    public float wheelbase = 2.7f;      // Rozstaw osi samochodu, który bêdzie u¿ywany do obliczania k¹tów skrêtu kó³.
    public float trackWidth = 1.8f;     // Szerokoœæ toru jazdy samochodu, która bêdzie u¿ywana do obliczania k¹tów skrêtu kó³.


    // Prywatna zmienna do przechowywania referencji do komponentu Rigidbody, który bêdzie u¿ywany do poruszania siê samochodu.
    private Rigidbody rb;

    // Zmienna do przechowywania aktualnego stanu samochodu w ramach FSM (Finite State Machine).
    private ICarState currentState;    


    public float verticalInput;    // Zmienna do przechowywania wartoœci wejœcia pionowego (przyspieszenie/hamowanie).
    public float horizontalInput;  // Zmienna do przechowywania wartoœci wejœcia poziomego (skrêt kó³).


    // Metoda Start jest wywo³ywana na pocz¹tku gry, przed pierwsz¹ klatk¹. 
    void Start()
    {
        // Pobieram komponent Rigidbody, który jest przypisany do tego obiektu (samochodu).
        rb = GetComponent<Rigidbody>();

        // Je¿eli centerOfMass jest przypisany, to ustawiam œrodek masy samochodu na podstawie jego pozycji.
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;   // Ustawiam œrodek masy
        }

        // Ustawiam pocz¹tkowy stan samochodu na SearchState, co oznacza, ¿e samochód rozpocznie poszukiwanie miejsca parkingowego na pocz¹tku gry.
        ChangeState(new SearchState());   
    }

    // Metoda Update - jest wywo³ywana raz na klatkê i jest odpowiedzialna za pobieranie wejœcia
    // od gracza oraz aktualizacjê pozycji i rotacji modeli 3D kó³, aby odpowiada³y aktualnemu stanowi fizyki kó³.
    void Update()
    {
        // Warunek logiczny - sprawdzam, czy currentState nie jest null, co oznacza, ¿e samochód znajduje siê w jakimœ stanie. 
        if (currentState != null)
        {
            // Je¿eli currentState nie jest null, to wywo³ujê metodê UpdateState na aktualnym stanie, przekazuj¹c jako argument ten obiekt (samochód), aby umo¿liwiæ aktualizacjê stanu samochodu na podstawie logiki specyficznej dla tego stanu.
            currentState.UpdateState(this);   
        }

        UpdateWheelPoses();    // Aktualizujê pozycje i rotacje modeli 3D kó³, aby odpowiada³y aktualnemu stanowi fizyki kó³.
    }

    // Metoda FixedUpdate - jest wywo³ywana w sta³ych odstêpach czasu i jest odpowiedzialna za aktualizacjê fizyki
    // samochodu, w tym obliczanie k¹tów skrêtu kó³ na podstawie zasady Ackermana oraz stosowanie si³y silnika do kó³
    // tylnych, aby poruszaæ samochód do przodu lub do ty³u.
    void FixedUpdate()
    {
        HandlerMotor();                 // Stosujê si³ê silnika do kó³ tylnych na podstawie wejœcia pionowego, aby poruszaæ samochód do przodu lub do ty³u.
        HandlerSteeringAckerman();      // Obliczam k¹ty skrêtu kó³ przednich na podstawie zasady Ackermana, aby zapewniæ poprawn¹ stabilnoœæ pojazdu podczas zakrêtów.
    }

    // Metoda HandlerMotor - metoda odpowiedzialna za stosowanie si³y silnika do kó³ tylnych na podstawie wejœcia pionowego, aby poruszaæ samochód do przodu lub do ty³u.
    private void HandlerMotor()
    {
        rearLeftCollider.motorTorque = verticalInput * motorForce;     // Stosujê si³ê silnika do tylnego lewego ko³a na podstawie wejœcia pionowego i ustawionej si³y silnika.
        rearRightCollider.motorTorque = verticalInput * motorForce;    // Stosujê si³ê silnika do tylnego prawego ko³a na podstawie wejœcia pionowego i ustawionej si³y silnika.
    }

    // Metoda HandlerSteeringAckerman - metoda odpowiedzialna za obliczanie k¹tów skrêtu kó³ przednich na podstawie zasady
    // Ackermana, która zapewnia, ¿e ko³a skrêcaj¹ w odpowiedni sposób podczas zakrêtów, aby unikn¹æ poœlizgu i poprawiæ stabilnoœæ pojazdu.
    private void HandlerSteeringAckerman()
    {
        float steerAngle = horizontalInput * maxSteerAngle;   // Obliczam k¹t skrêtu na podstawie wejœcia poziomego i maksymalnego k¹ta skrêtu.

        // Je¿eli k¹t skrêtu jest wiêkszy ni¿ 0.01f (aby unikn¹æ niepotrzebnych obliczeñ przy bardzo ma³ych wartoœciach), to wykonujê dalsze obliczenia.
        if (Math.Abs(steerAngle) > 0.01f)
        {
            // Obliczam promieñ skrêtu ze œrodka osi: L / ten(delta)
            float turnRadius = wheelbase / Mathf.Tan(Mathf.Deg2Rad * steerAngle);   

            // Wzory z pdf - k¹ty wewnêtrzne i zewnêtrzne - zmienione z radianów na stopnie
            float angleInner = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - (trackWidth / 2)));   // K¹t skrêtu wewnêtrznego ko³a.
            float angleOuter = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + (trackWidth / 2)));   // K¹t skrêtu zewnêtrznego ko³a.


            // Warunek logiczny - je¿eli k¹t skrêtu jest wiêkszy ni¿ 0, to oznacza, ¿e samochód skrêca w prawo, wiêc ustawiam
            // k¹t skrêtu wewnêtrznego ko³a na przednim prawym kole, a k¹t skrêtu zewnêtrznego ko³a na przednim lewym kole.
            // Skrêt w prawo
            if (steerAngle > 0)
            {
                frontRightCollider.steerAngle = angleInner;   // Ustawiam k¹t skrêtu wewnêtrznego ko³a na przednim prawym kole.
                frontLeftCollider.steerAngle = angleOuter;    // Ustawiam k¹t skrêtu zewnêtrznego ko³a na przednim lewym kole.
            }


            // W przeciwnym przypadku, je¿eli k¹t skrêtu jest mniejszy ni¿ 0, to oznacza, ¿e samochód skrêca w lewo, wiêc ustawiam
            // Skrêt w lewo
            else
            {
                frontLeftCollider.steerAngle = angleInner;   // Ustawiam k¹t skrêtu wewnêtrznego ko³a na przednim lewym kole.
                frontRightCollider.steerAngle = angleOuter;  // Ustawiam k¹t skrêtu zewnêtrznego ko³a na przednim prawym kole.
            }
        }

        // W przeciwnym przypadku, je¿eli k¹t skrêtu jest mniejszy lub równy 0.01f, to oznacza, ¿e samochód jedzie prosto, wiêc ustawiam k¹t skrêtu na 0 dla obu przednich kó³.
        else
        {
            frontLeftCollider.steerAngle = 0;    // Ustawiam k¹t skrêtu na 0 dla przedniego lewego ko³a.
            frontRightCollider.steerAngle = 0;   // Ustawiam k¹t skrêtu na 0 dla przedniego prawego ko³a.
        }
    }


    // Metoda UpdateWheelPoses - metoda odpowiedzialna za aktualizacjê pozycji i rotacji modeli 3D kó³ na podstawie pozycji i rotacji Wheel Colliderów, aby zapewniæ poprawn¹ wizualizacjê kó³ podczas jazdy samochodu.
    private void UpdateWheelPoses()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftVisual);     // Aktualizujê pozycjê i rotacjê modelu 3D przedniego lewego ko³a.
        UpdateSingleWheel(frontRightCollider, frontRightVisual);   // Aktualizujê pozycjê i rotacjê modelu 3D przedniego prawego ko³a.
        UpdateSingleWheel(rearLeftCollider, rearLeftVisual);       // Aktualizujê pozycjê i rotacjê modelu 3D tylnego lewego ko³a.
        UpdateSingleWheel(rearRightCollider, rearRightVisual);     // Aktualizujê pozycjê i rotacjê modelu 3D tylnego prawego ko³a.
    }


    // Metoda UpdateSingleWheel - metoda pomocnicza, która aktualizuje pozycjê i rotacjê pojedynczego modelu 3D ko³a na podstawie pozycji i rotacji
    // odpowiadaj¹cego mu Wheel Collidera, aby zapewniæ poprawn¹ wizualizacjê tego ko³a podczas jazdy samochodu.
    private void UpdateSingleWheel(WheelCollider collider, Transform visual)
    {
        Vector3 pos;                                    // Zmienna do przechowywania pozycji ko³a.
        Quaternion rot;                                 // Zmienna do przechowywania rotacji ko³a.
        collider.GetWorldPose(out pos, out rot);        // Pobieram pozycjê i rotacjê Wheel Collidera.
        visual.position = pos;                          // Ustawiam pozycjê modelu 3D ko³a na podstawie pozycji Wheel Collidera.
        visual.rotation = rot;                          // Ustawiam rotacjê modelu 3D ko³a na podstawie rotacji Wheel Collidera.
    }
}