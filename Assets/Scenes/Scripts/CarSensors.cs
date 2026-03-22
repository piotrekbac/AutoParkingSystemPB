using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia LIDARu")]
    public float sensorLength = 10f;
    public Transform rightSensorPosition;

    public float currentDistanceToObstacle = 0f;
    public bool isObstacleDetected = false;

    // NOWOŒÆ: Przedni radar do wykrywania pieszych!
    public bool isFrontObstacleDetected = false;

    void Update()
    {
        ScanEnvironment();
    }

    private void ScanEnvironment()
    {
        // 1. SKANOWANIE BOCZNE (Szukanie Luki)
        Vector3 origin = rightSensorPosition != null ? rightSensorPosition.position : transform.position;
        Vector3 direction = transform.right;
        direction.y = 0;
        direction.Normalize();

        if (Physics.Raycast(origin, direction, out RaycastHit hit, sensorLength))
        {
            if (hit.distance > 0.1f)
            {
                isObstacleDetected = true;
                currentDistanceToObstacle = hit.distance;
                Debug.DrawRay(origin, direction * hit.distance, Color.red);
            }
            else
            {
                isObstacleDetected = false;
                currentDistanceToObstacle = sensorLength;
                Debug.DrawRay(origin, direction * sensorLength, Color.green);
            }
        }
        else
        {
            isObstacleDetected = false;
            currentDistanceToObstacle = sensorLength;
            Debug.DrawRay(origin, direction * sensorLength, Color.green);
        }

        // 2. SKANOWANIE PRZEDNIE (Awaryjne - HFSM)
        // Rzucamy krótki promieñ (3 metry) prosto przed maskê samochodu
        Vector3 frontOrigin = transform.position + transform.forward * 2.3f;
        frontOrigin.y = 0.5f;

        if (Physics.Raycast(frontOrigin, transform.forward, out RaycastHit frontHit, 3.0f))
        {
            isFrontObstacleDetected = true;
            Debug.DrawRay(frontOrigin, transform.forward * frontHit.distance, Color.magenta);
        }
        else
        {
            isFrontObstacleDetected = false;
            Debug.DrawRay(frontOrigin, transform.forward * 3.0f, Color.blue);
        }
    }
}