
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;  // Reference to the boxPrefab prefab
    public float spawnInterval = 8.5f;  // Time between spawns
    public float xMin = 2f;  // Minimum x-coordinate for spawning
    public float xMax = 6f;  // Maximum x-coordinate for spawning
    public float spawnHeight = 10f; // Height where boxes will spawn from
    public int maxBoxesPerSpawn = 2; // Maximum number of boxes to spawn at once
    private PlayerBorder playerBorder;  // Reference to PlayerBorder to get screen edges
    public static float timer;
    public Material[] customBoxMaterials;

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
            // Clamp the number of boxes to ensure it doesn't exceed the maximum green: money, yellw
            int boxesToSpawn = Mathf.Clamp(Random.Range(1, maxBoxesPerSpawn + 1), 1, maxBoxesPerSpawn);
            for (int i = 0; i < boxesToSpawn; i++)
            {
                // Generate a random x-position within the defined range
                float spawnX = Random.Range(xMin, xMax);
                // Instantiate the new boxPrefab at the random position
                GameObject newBox = Instantiate(boxPrefab, new Vector2(spawnX, spawnHeight), Quaternion.identity);

                // Assign random floatSpeed and acceleration values
                float randomFloatSpeed = Random.Range(2f, 3f); // Adjust range as needed
                float randomAcceleration = Random.Range(2f, 3f); // Adjust range as needed

                BoxMovement boxMovement = newBox.GetComponent<BoxMovement>();
                if (boxMovement != null)
                {
                    boxMovement.Initialize(randomFloatSpeed, randomAcceleration, this);
                }

                // Randomly select a box type
                newBox.tag = "boxPrefab"; // Add tag to the new boxPrefab
                BoxType type = (BoxType)Random.Range(0, System.Enum.GetValues(typeof(BoxType)).Length);


                MeshRenderer meshRenderer = newBox.GetComponent<MeshRenderer>();
                // Dynamically determine and assign the material based on BoxType
                Material newMaterial = GetMaterialForBoxType(type);

                // Apply the material
                Debug.Log("Material works" + newMaterial.mainTexture);
                meshRenderer.material = newMaterial;

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

    Material GetMaterialForBoxType(BoxType type)
    {
         Material loadedMaterial = null;

        switch (type)
        {
            case BoxType.AddPoints:
                loadedMaterial = Resources.Load<Material>("AddPointsTexture"); // Load the material
                break;
            case BoxType.AddMoney:
                loadedMaterial = Resources.Load<Material>("AddMoneyTexture");
                break;

            case BoxType.MultiplyPoints:
                loadedMaterial = Resources.Load<Material>("MultiplyPointsTexture");
                break;

            case BoxType.RemoveLife:
                loadedMaterial = Resources.Load<Material>("RemoveLifeTexture");
                break;

            case BoxType.AddPowerup:
                loadedMaterial = Resources.Load<Material>("AddPowerupTexture");
                break;
        }

        return loadedMaterial;
    }

    public void BoxDestroyed()
    {
        if (gameObject != null)
        {
            gameObject.tag = "Untagged"; // Remove from "boxPrefab" tag to exclude from future searches
            Destroy(gameObject); // Destroy the boxPrefab
        }

        // Dynamically count active boxes
        int boxCount = GameObject.FindGameObjectsWithTag("boxPrefab").Length;

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
