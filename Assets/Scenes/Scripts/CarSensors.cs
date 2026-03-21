using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class CarSensors : MonoBehaviour
{
    [Header("Ustawienia LIDARu")]
    public float sensorLength = 10f;
    public Transform rightSensorPosition;

    public float currentDistanceToObstacle = 0f;
    public bool isObstacleDetected = false;

    void Update()
    {
        ScanEnvironment();
    }

    private void ScanEnvironment()
    {
        Vector3 origin = rightSensorPosition != null ? rightSensorPosition.position : transform.position;
        Vector3 direction = transform.right;
        direction.y = 0;
        direction.Normalize();

        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, sensorLength))
        {
            // MEGA WA¯NE: Zmieniono z 1.0f na 0.1f! Laser ju¿ nie zgubi kostki, jeœli podjedziesz za blisko!
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
    }
}