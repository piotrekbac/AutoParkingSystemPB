using UnityEngine;

// Piotr Bacior 15 722 - WSEI Kraków - Informatyka stosowana

public class MovingObstacle : MonoBehaviour
{
    public float speed = 2.0f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Ruch ping-pong w osi X (przechodzenie tam i z powrotem przez jezdniê)
        float offset = Mathf.PingPong(Time.time * speed, 12f) - 6f;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}