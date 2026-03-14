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

    public WheelCollider frontLeftWheel;    // Ko³o przednie lewe
    public WheelCollider frontRightWheel;   // Ko³o przednie prawe
    public WheelCollider backLeftWheel;     // Ko³o tylne lewe
    public WheelCollider backRightWheel;    // Ko³o tylne prawe


    // Metoda Start jest wywo³ywana na pocz¹tku gry, przed pierwsz¹ klatk¹. 
    void Start()
    {
        // Pobieram komponent Rigidbody, który jest przypisany do tego obiektu (samochodu).
        rb = GetComponent<Rigidbody>();

        // Je¿eli centerOfMass jest przypisany, to ustawiam œrodek masy samochodu na podstawie jego pozycji.
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition; // Ustawiam œrodek masy
        }
    }
}