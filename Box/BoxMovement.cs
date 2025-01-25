using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    public float floatSpeed; // Speed for floating up
    public float maxFallSpeed = 5f; // Maximum speed while falling
    public float acceleration; // Acceleration as the box gets close to the bottom
    private bool movingUp = true; // Is the box floating up?
    private float screenBottom;
    private float screenTop;
    private BoxCollisions boxCollisions;
    private BoxSpawner boxSpawner;

    void Start()
    {
        // Get screen bounds
        Camera cam = Camera.main;
        screenBottom = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        screenTop = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        // Start the box below the screen
        transform.position = new Vector3(
            Random.Range(-1f, 6f), // Random horizontal position (adjust as needed)
            screenBottom - 1f,    // Below the bottom of the screen
            0
        );
        boxSpawner = GetComponent<BoxSpawner>();
        boxCollisions = GetComponent<BoxCollisions>();
        Debug.Log("Box spawned at position: " + transform.position);
    }

    void Update()
    {
        if (movingUp)
        {
            // Move up
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            // Check if the box has reached the top
            if (transform.position.y >= screenTop)
            {
                movingUp = false; // Start falling
            }
        }
        else
        {
            // Accelerate as the box falls down
            float fallSpeed = Mathf.Clamp(
                maxFallSpeed * ((transform.position.y - screenBottom) / (screenTop - screenBottom)),
                0,
                maxFallSpeed
            );
            // Move down
            transform.position -= Vector3.up * (fallSpeed + acceleration) * Time.deltaTime;

            // Destroy the box if it goes off the screen
            if (transform.position.y <= screenBottom)
            {
                boxCollisions.handleCollisionOutsideScreen();

                boxSpawner.BoxDestroyed();

            }

        }
    }

        public void Initialize(float floatSpeed, float acceleration, BoxSpawner spawner)
    {
        this.floatSpeed = floatSpeed;
        this.acceleration = acceleration;

        this.boxSpawner = spawner;
    }
}
