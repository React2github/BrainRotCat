
using UnityEngine;
using TMPro;
using System.Collections;
public class BoxSpawner : MonoBehaviour
{
    public static BoxSpawner Instance { get; private set; }
    public GameObject boxPrefab;  // Reference to the boxPrefab prefab
    public float spawnInterval = 8.5f;  // Time between spawns
    public float xMin = 2f;  // Minimum x-coordinate for spawning
    public float xMax = 6f;  // Maximum x-coordinate for spawning
    public float spawnHeight = 10f; // Height where boxes will spawn from
    public int maxBoxesPerSpawn = 3; // Maximum number of boxes to spawn at once
    private PlayerBorder playerBorder;  // Reference to PlayerBorder to get screen edges
    public static float timer;
    public Material[] customBoxMaterials;
    private GameObject[] allBoxes;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set this instance as the Singleton
        }
        else
        {
            // Deactivate the extra instance rather than destroying it
            gameObject.SetActive(false); // Disable the extra BoxSpawner instance
            return; // Exit the Awake method to prevent further execution
        }

        // Optional: If you want the BoxSpawner to persist across scenes, you can add this
        DontDestroyOnLoad(gameObject); // Keep the instance alive across scenes if needed
    }
    void Start()
    {
        playerBorder = FindObjectOfType<PlayerBorder>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        IsAnyBoxOnScreen();

        allBoxes = GameObject.FindGameObjectsWithTag("boxPrefab");
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

                MeshRenderer meshRenderer = newBox.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    meshRenderer = newBox.AddComponent<MeshRenderer>();
                }
                MeshFilter meshFilter = newBox.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    meshFilter = newBox.AddComponent<MeshFilter>();
                }

                // Assign the Flash mesh to the MeshFilter
                Mesh flashMesh = Resources.Load<Mesh>("Flask");
                if (flashMesh != null)
                {
                    meshFilter.mesh = flashMesh;
                }


                // Assign the  material to the MeshRenderer
                Material Transparent = Resources.Load<Material>("Transparent");
                if (Transparent != null)
                {
                    meshRenderer.material = Transparent;
                }
                // Set the scale of the new boxPrefab
                newBox.transform.localScale = new Vector3(1.33f, newBox.transform.localScale.y, newBox.transform.localScale.z);

                /////////////////////////// Child componenet 
                // Create the Capsule child object
                GameObject capsule = new GameObject("Capsule");

                // Set the Capsule as a child of newBox
                capsule.transform.parent = newBox.transform;

                // Reset the local position to ensure it's properly placed inside newBox
                capsule.transform.localPosition = new Vector3(0f, 0.06f, 0f);

                // Set the scale of capsule
                capsule.transform.localScale = new Vector3(0.87f, 0.87f, 0.87f);

                // Add and assign the MeshFilter
                MeshFilter capsuleMeshFilter = capsule.AddComponent<MeshFilter>();
                Mesh flaskMesh = Resources.Load<Mesh>("Flask");
                if (flaskMesh != null)
                {
                    capsuleMeshFilter.mesh = flaskMesh;
                }

                // Add and assign the MeshRenderer
                MeshRenderer capsuleMeshRenderer = capsule.AddComponent<MeshRenderer>();
                Material insideLiquidMaterial = Resources.Load<Material>("InsideLquid");
                if (insideLiquidMaterial != null)
                {
                    capsuleMeshRenderer.material = insideLiquidMaterial;
                }

                // Assign random floatSpeed and acceleration values
                float randomFloatSpeed = Random.Range(2f, 3f); // Adjust range as needed
                float randomAcceleration = Random.Range(2f, 3f); // Adjust range as needed

                BoxMovement boxMovement = newBox.GetComponent<BoxMovement>();
                if (boxMovement != null)
                {
                    boxMovement.Initialize(randomFloatSpeed, randomAcceleration, this);
                }

                BoxCollider2D collider = newBox.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = newBox.AddComponent<BoxCollider2D>(); // Add BoxCollider2D if it doesn't exist
                }
                collider.isTrigger = true; // Set isTrigger to true


                // Randomly select a box type
                newBox.tag = "boxPrefab"; // Add tag to the new boxPrefab
                BoxType type = (BoxType)Random.Range(0, System.Enum.GetValues(typeof(BoxType)).Length);
                SetCapsuleColor(type, newBox);


                // MeshRenderer meshRenderer = newBox.GetComponent<MeshRenderer>();
                // Dynamically determine and assign the material based on BoxType
                // Material newMaterial = GetMaterialForBoxType(type);

                // Apply the material
                // meshRenderer.material = newMaterial;

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

    void SetCapsuleColor(BoxType type, GameObject boxPrefab)
    {
        // Find the child object named "Capsule" inside boxPrefab
        Transform capsuleTransform = boxPrefab.transform.Find("Capsule");

        if (capsuleTransform != null)
        {
            Renderer capsuleRenderer = capsuleTransform.GetComponent<Renderer>();
            // Transform labelTextTransform = boxPrefab.transform.Find("LabelText");

            if (capsuleRenderer != null)
            {
                capsuleRenderer.material = new Material(capsuleRenderer.material); // Create a new material instance
                Color baseColor = Color.white;

                // Set the material color dynamically based on the BoxType
                switch (type)
                {
                    case BoxType.AddPoints:
                        baseColor = Color.white;
                        // SetLabelText(labelTextTransform, "");
                        break;
                    case BoxType.AddMoney:
                        baseColor = Color.yellow;
                        // SetLabelText(labelTextTransform, "150");
                        break;
                    case BoxType.MultiplyPoints:
                        baseColor = Color.green;
                        // SetLabelText(labelTextTransform, "100");
                        break;
                    case BoxType.RemoveLife:
                        baseColor = Color.black;
                        // SetLabelText(labelTextTransform, "");
                        break;
                    case BoxType.AddPowerup:
                        // SetLabelText(labelTextTransform, "");
                        baseColor = Color.red;
                        break;
                }

                capsuleRenderer.material.color = baseColor;  // Set the base color

                MonoBehaviour boxMonoBehaviour = boxPrefab.GetComponent<MonoBehaviour>();
                if (boxMonoBehaviour != null)
                {
                    boxMonoBehaviour.StartCoroutine(ColorPopEffect(capsuleRenderer, baseColor));
                }
            }
        }
    }

    IEnumerator ColorPopEffect(Renderer renderer, Color baseColor)
    {
        float duration = 8f;
        float elapsedTime = 0f;

        Color popColor = Color.Lerp(baseColor, Color.white, 0.5f); // Brighter pop color

        while (elapsedTime < duration)
        {
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            Color poppedColor = Color.Lerp(baseColor, popColor, t);

            renderer.material.SetColor("_Color", poppedColor); // Ensure proper material update
            Debug.Log(renderer.material.color + "popped color" + poppedColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        renderer.material.SetColor("_Color", baseColor); // Reset at the end
        Debug.Log(renderer.material.color + "reset to base color" + baseColor);
    }


    // Method to set the label text of the TMP text object
    void SetLabelText(Transform labelTextTransform, string labelText)
    {
        // Get the TextMeshPro component from the label text object
        TextMeshPro textMeshPro = labelTextTransform.GetComponent<TextMeshPro>();

        if (textMeshPro != null)
        {
            textMeshPro.text = labelText;  // Set the text dynamically
        }
    }

    public void BoxDestroyed(GameObject boxInstance)
    {
        if (boxInstance != null)
        {
            Destroy(boxInstance);
            Debug.Log("Box destroyed!");
        }

        // Dynamically count active boxes
        // Start a coroutine to wait until the end of the frame
        StartCoroutine(CheckAndSpawnBoxes());
    }

    private IEnumerator CheckAndSpawnBoxes()
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        // Dynamically count active boxes
        allBoxes = GameObject.FindGameObjectsWithTag("boxPrefab");
        Debug.Log("Active box count: " + allBoxes.Length);

        if (allBoxes.Length == 0)
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
