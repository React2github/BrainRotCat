
using UnityEngine;
public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;  // Reference to the boxPrefab prefab
    public float spawnInterval = 8.5f;  // Time between spawns
    public float xMin = -1f;  // Minimum x-coordinate for spawning
    public float xMax = 6f;  // Maximum x-coordinate for spawning
    public float spawnHeight = 10f; // Height where boxes will spawn from
    public int maxBoxesPerSpawn = 2; // Maximum number of boxes to spawn at once
    private PlayerBorder playerBorder;  // Reference to PlayerBorder to get screen edges
    public static float timer;

    void Start()
    {
        playerBorder = FindObjectOfType<PlayerBorder>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        IsAnyBoxOnScreen();

        GameObject[] allBoxes = GameObject.FindGameObjectsWithTag("boxPrefab");
        Debug.Log("Active box count: " + allBoxes.Length);


        // Debug.Log("Timer: " + timer);

        // Check if any boxPrefab is on screen and only spawn if none are present
        if (timer >= spawnInterval && !IsAnyBoxOnScreen())
        {
            Debug.Log("Spawning new boxes");
            // SpawnBoxes();
            timer = 0f;  // Reset timer after spawning
        }
        else
        {
            Debug.Log("Boxes are still on screen");
        }
    }

    private bool IsAnyBoxOnScreen()
    {
        // Get all boxes in the scene
        GameObject[] allBoxes = GameObject.FindGameObjectsWithTag("boxPrefab");
        // Loop through all the boxes and check if any are within screen boundaries
        foreach (GameObject boxPrefab in allBoxes)
        {
            BoxCollisions boxCollisions = boxPrefab.GetComponent<BoxCollisions>();
            if (boxCollisions != null && boxCollisions.isActive)
            {
                // Get the boxPrefab's position
                float boxYPosition = boxPrefab.transform.position.y;
                // Check if the boxPrefab is between the top and bottom edges of the screen
                if (boxYPosition <= playerBorder.topEdge && boxYPosition >= playerBorder.bottomEdge)
                {
                    return true;
                }
            }
        }
        return false; // No boxPrefab on screen
    }

    public void SpawnBoxes()
    {
        if (boxPrefab == null)
        {
            Debug.LogError("BoxPrefab is not assigned!");
            return;
        }

        if (!IsAnyBoxOnScreen())
        {
            // Clamp the number of boxes to ensure it doesn't exceed the maximum
            int boxesToSpawn = Mathf.Clamp(Random.Range(1, maxBoxesPerSpawn + 1), 1, maxBoxesPerSpawn);
            for (int i = 0; i < boxesToSpawn; i++)
            {
                // Generate a random x-position within the defined range
                float spawnX = Random.Range(xMin, xMax);
                // Instantiate the new boxPrefab at the random position
                GameObject newBox = Instantiate(boxPrefab, new Vector2(spawnX, spawnHeight), Quaternion.identity);
                // Randomly select a box type
                newBox.tag = "boxPrefab"; // Add tag to the new boxPrefab
                BoxType type = (BoxType)Random.Range(0, System.Enum.GetValues(typeof(BoxType)).Length);

                SpriteRenderer spriteRenderer = newBox.GetComponent<SpriteRenderer>();
                switch (type)
                {
                    case BoxType.AddPoints:
                        spriteRenderer.color = Color.green; // Green for adding points
                        break;
                    case BoxType.MultiplyPoints:
                        spriteRenderer.color = Color.yellow;   // Red for deducting points
                        break;
                    case BoxType.RemoveLife:
                        spriteRenderer.color = Color.black; // Yellow for removing life
                        break;
                }

                BoxCollisions boxCollisions = newBox.GetComponent<BoxCollisions>();
                if (boxCollisions != null)
                {
                    boxCollisions.SetBoxType(type); // Set the box type
                    boxCollisions.SetBoxSpawnerReference(this); // Pass reference to BoxSpawner
                }

                EnableBoxComponents(newBox);  // Ensure components are properly enabled

                Debug.Log("New box spawned at position: " + new Vector2(spawnX, spawnHeight)); // Log spawn for debugging
            }
        }
    }

    public void BoxDestroyed()
    {
        if (gameObject != null)
        {
            gameObject.tag = "Untagged"; // Remove from "boxPrefab" tag to exclude from future searches
            Destroy(gameObject); // Destroy the boxPrefab
            Debug.Log("Box destroyed.");
        }
        else
        {
            Debug.LogError("BoxPrefab is null!");
        }

        // Dynamically count active boxes
        int boxCount = GameObject.FindGameObjectsWithTag("boxPrefab").Length;
        Debug.Log("Updated active box count: " + boxCount);

        if (boxCount == 0)
        {
            Debug.Log("No active boxes, spawning new boxes...");
            SpawnBoxes(); // Spawn new boxes
        }
    }


    private void EnableBoxComponents(GameObject boxPrefab)
    {
        // Re-enable components to ensure they are not disabled
        MonoBehaviour[] monoBehaviours = boxPrefab.GetComponentsInChildren<MonoBehaviour>();
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            monoBehaviour.enabled = true;  // Enable each script
        }

        Rigidbody2D rb2d = boxPrefab.GetComponent<Rigidbody2D>();
        if (rb2d != null && !rb2d.isKinematic)
        {
            rb2d.simulated = true; // Enable Rigidbody2D physics if it's not kinematic
        }

        BoxCollider2D boxCollider = boxPrefab.GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }
}
