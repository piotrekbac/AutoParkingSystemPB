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


    private float verticalInput;    // Zmienna do przechowywania wartoœci wejœcia pionowego (przyspieszenie/hamowanie).
    private float horizontalInput;  // Zmienna do przechowywania wartoœci wejœcia poziomego (skrêt kó³).


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
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");      // Pobieram wartoœæ wejœcia poziomego (skrêt kó³) z klawiatury.
        verticalInput = Input.GetAxis("Vertical");          // Pobieram wartoœæ wejœcia pionowego (przyspieszenie/hamowanie) z klawiatury.

        
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
        }
    }
}