using UnityEngine;

public class BoxCollisions : MonoBehaviour
{
    private BoxType boxType;
    private PlayerCollisions playerCollisions;
    private BoxSpawner boxSpawner;
    private GameManager gameManager;
    public bool isActive = true;
    public void SetBoxSpawnerReference(BoxSpawner spawner)
    {
        boxSpawner = spawner;
    }
    public void SetBoxType(BoxType type)
    {
        boxType = type;
    }
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerCollisions = FindObjectOfType<PlayerCollisions>();
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
            playerCollisions.OnBoxCollision(boxType); // Call method to update score/lives
            if (boxSpawner != null)
            {
                boxSpawner.BoxDestroyed();
            }
        }
    }

    public void handleCollisionOutsideScreen()
    {
        {
            if (boxType == BoxType.AddPoints || boxType == BoxType.MultiplyPoints)
            {
                if (PlayerCollisions.lives > 0)
                    PlayerCollisions.lives--;
                    playerCollisions.UpdateLivesUI();
            }

            if (boxSpawner != null)
            {
                boxSpawner.BoxDestroyed();
            }

            if (PlayerCollisions.lives <= 0)
            {
                PlayerCollisions.lives = 0;
                // gameManager.GameOver();
            }

        }
    }

}
