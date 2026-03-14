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

    [Header("Referencje - Modele 3D kó³ (Wizualizacja kó³)")]


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