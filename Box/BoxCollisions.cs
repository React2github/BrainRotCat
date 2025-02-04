using UnityEngine;
using System.Collections;
public class BoxCollisions : MonoBehaviour
{
    private BoxType boxType;
    private PlayerCollisions playerCollisions;
    private BoxSpawner boxSpawner;
    private GameManager gameManager;
    public bool isActive = true;
    private AudioSource audioSource;
    // public GameObject expulsionEffectPrefab; 
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
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = true;
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
            Vector2 contactPoint = collision.contacts[0].point;

            playerCollisions.OnBoxCollision(boxType); // Call method to update score/lives
            if (boxSpawner != null)
            {
                audioSource.Play();
                StartCoroutine(DelayedBoxDestruction(contactPoint));
            }
        }
    }

    private IEnumerator DelayedBoxDestruction(Vector2 contactPoint)
    {
        // Wait for the duration of the audio clip
        yield return new WaitForSeconds(audioSource.clip.length);

        // if (expulsionEffectPrefab != null)
        // {
        //     Instantiate(expulsionEffectPrefab, contactPoint, Quaternion.identity);
        // }

        // ParticleSystem particleSystem = expulsionEffectPrefab.GetComponent<ParticleSystem>();
        // particleSystem.Play();
        // Destroy the box after the audio has finished playing
        if (boxSpawner != null)
        {
            boxSpawner.BoxDestroyed();
        }
    }

    public void handleCollisionOutsideScreen()
    {
        {
            if (boxType == BoxType.RemoveLife)
            {
                return;
            }
            else
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
                gameManager.GameOver();
            }

        }
    }

}
