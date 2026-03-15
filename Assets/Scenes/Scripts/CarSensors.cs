using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarSensors : MonoBehaviour
{
    // Nag³ówek dla sekcji ustawieñ LIDARu, które bêd¹ u¿ywane do konfiguracji sensorów LIDARu samochodu.
    [Header("Ustawienia LIDARu")]

    public float sensorLength = 10f;        // D³ugoœæ promienia LIDARu, który bêdzie u¿ywany do wykrywania przeszkód.
    public Transform rightSensorPosition;   // Referencja do Transformu, który bêdzie okreœla³ pozycjê prawego sensora LIDARu.

    public float currentDistanceToObstacle;   // Zmienna do przechowywania aktualnej odleg³oœci do przeszkody, która bêdzie aktualizowana na podstawie wyników wykrywania LIDARu.
    public bool isObstacleDetected;           // Zmienna do przechowywania informacji o tym, czy przeszkoda zosta³a wykryta, która bêdzie aktualizowana na podstawie wyników wykrywania LIDARu.

    // Metoda ScanEnvironment jest odpowiedzialna za skanowanie otoczenia za pomoc¹ LIDARu i aktualizowanie informacji o wykrytych przeszkodach.
    private void ScanEnvironment()
    {
        Vector3 origin = rightSensorPosition != null ? rightSensorPosition.position : transform.position;  // Ustalam punkt pocz¹tkowy promienia LIDARu (pozycja prawego sensora lub pozycja samochodu).

        Vector3 direction = transform.right;   // Ustalam kierunek promienia LIDARu (kierunek prawy samochodu).

        RaycastHit hit;                        // Tworzê zmienn¹ typu RaycastHit, która bêdzie przechowywaæ informacje o trafieniu promienia LIDARu w przeszkodê.

        // Warunek logiczny - fizyka Raycast - sprawdzam, czy promieñ LIDARu trafia w przeszkodê, u¿ywaj¹c funkcji Physics.Raycast, która zwraca true, jeœli promieñ trafi w coœ, i false, jeœli nie trafi. Jeœli promieñ trafi w przeszkodê, informacje o trafieniu zostan¹ zapisane w zmiennej hit.
        if (Physics.Raycast(origin, direction, out hit, sensorLength))
        {
            isObstacleDetected = true;                 // Ustawiam isObstacleDetected na true, poniewa¿ przeszkoda zosta³a wykryta.
            currentDistanceToObstacle = hit.distance;  // Aktualizujê currentDistanceToObstacle na odleg³oœæ do wykrytej przeszkody, która jest dostêpna w zmiennej hit.distance.

            // Rysowanie CZERWONEGO lasera w edytorze - uderzenie w przeszkodê 
            Debug.DrawLine(origin, direction * hit.distance, Color.red);  // Rysujê liniê od punktu pocz¹tkowego do punktu trafienia, u¿ywaj¹c koloru czerwonego, aby wizualizowaæ wykrycie przeszkody.
        }
    }
}
