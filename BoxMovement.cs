using UnityEngine;

public class BoxMovement : MonoBehaviour
{
    public float floatSpeed = 6f; // Speed for floating up
    public float maxFallSpeed = 5f; // Maximum speed while falling
    public float acceleration = 5f; // Acceleration as the box gets close to the bottom
    private bool movingUp = true; // Is the box floating up?
    private BoxSpawner boxSpawner; // Reference to the BoxSpawner script

    private BoxType boxType;

    public void SetBoxType(BoxType type)
    {
        boxType = type;
    }

    public bool isActive = true;

    public void SetBoxSpawnerReference(BoxSpawner spawner)
    {
        boxSpawner = spawner;
    }

    private float screenBottom;
    private float screenTop;

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

        Debug.Log("Box spawned at position: " + transform.position);

        boxSpawner = FindObjectOfType<BoxSpawner>();
        if (boxSpawner == null)
        {
            Debug.LogError("BoxSpawner component not found in the scene!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ensure the box only destroys itself when colliding with the PlayerLauncher
        if (collision.gameObject.CompareTag("PlayerSphere"))
        {
            isActive = false;
            if (boxSpawner != null)
            {
                boxSpawner.BoxDestroyed();
            }

            PlayerLauncher playerLauncher = collision.gameObject.GetComponent<PlayerLauncher>();
            if (playerLauncher != null)
            {
                playerLauncher.OnBoxCollision(boxType); // Call method in PlayerLauncher to update score/lives
            }
            Destroy(gameObject); // Destroy the box

        }
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
            if (transform.position.y <= screenBottom - 1f)
            {
                if (boxSpawner != null)
                {
                    boxSpawner.BoxDestroyed(); // Call BoxDestroyed in BoxSpawner
                }
                Debug.Log("Box destroyed for going off-screen.");
                if (boxType == BoxType.AddPoints || boxType == BoxType.MultiplyPoints)
                {
                    PlayerLauncher.lives--;
                }


                PlayerLauncher playerLauncher = FindObjectOfType<PlayerLauncher>();
                if (playerLauncher != null)
                {
                    playerLauncher.UpdateLivesUI();
                }

                Destroy(gameObject);

                if (PlayerLauncher.lives <= 0)
                {
                    GameManager.Instance.GameOver();  
                }

            }
        }
    }
}
