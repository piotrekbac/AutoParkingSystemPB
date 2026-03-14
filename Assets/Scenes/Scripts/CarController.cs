using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarController
{
    /* 
     
     Tworzê skrypt, który bêdzie odpowiada³ za sterowanie samochodem.
     Bêdzie on wykorzystywa³ komponent Rigidbody do poruszania siê
     samochodu oraz Transform do ustawiania œrodka masy. 

    */

    // Referencja do komponentu Rigidbody, który bêdzie odpowiada³ za fizykê samochodu.
    private Rigidbody rb;

    // Referencja do Transform, który bêdzie odpowiada³ za ustawianie œrodka masy samochodu.
    public Transform centerOfMass;

    // Metoda Start jest wywo³ywana na pocz¹tku gry, przed pierwsz¹ klatk¹. Tutaj ustawiam œrodek masy samochodu, jeœli centerOfMass jest przypisany.
    void Start()
    {

        // Je¿eli centerOfMass jest przypisany, to ustawiam œrodek masy samochodu na podstawie jego pozycji.
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition; // Ustawiam œrodek masy samochodu na podstawie pozycji centerOfMass.
        }
    }
}
