using UnityEngine;
using UnityEngine.UI; // For accessing UI elements
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerLauncher : MonoBehaviour
{
    public float maxPower = 20f; // Maximum launch power
    public Image powerBar; // Drag a UI Image here to show the progress bar
    private Rigidbody2D rb;
    private bool isDragging = false; // To track mouse dragging
    public LineRenderer lineRenderer;
    public float dotSpacing = 1f; // Distance between dots
    public float lineWidth = 0.05f; // Width of each dot
    private Vector2 initialMousePosition;
    private Vector2 currentMousePosition;
    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer component

    private void Start()
    {
        Debug.Log("PlayerLauncher script is attached to: " + gameObject.name);
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D

        if (lineRenderer == null)
        {
            // Add a LineRenderer component dynamically if not set in the inspector
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 0; // Start with no points
            lineRenderer.useWorldSpace = true;
        }

        // Apply the dotted line texture
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.mainTexture = Resources.Load<Texture2D>("DottedLineTexture");
        lineRenderer.material.mainTextureScale = new Vector2(1f / dotSpacing, 1); // Adjust spacing
        lineRenderer.textureMode = LineTextureMode.Tile; // Ensure it tiles properly

        if (powerBar != null)
        {
            powerBar.fillAmount = 0f; // Ensure the bar starts empty
            powerBar.gameObject.SetActive(false); // Hide the progress bar initially
        }

        // Invoke("GameOver", 3f); 
        Debug.Log("SpriteRenderer: " + spriteRenderer);
    }
    void OnMouseDown()
    {
        isDragging = true;
        if (powerBar != null)
        {
            powerBar.gameObject.SetActive(true);
        }

        initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("Initial Mouse Position: " + initialMousePosition);
        // Initialize the LineRenderer with 2 points (start and end)
        lineRenderer.positionCount = 0;
        // lineRenderer.SetPosition(0, transform.position); 
    }
    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Get the current mouse position
            Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate direction and distance from initial position
            float distance = Vector2.Distance(initialMousePosition, currentMousePosition);
            Vector2 direction = (currentMousePosition - initialMousePosition).normalized;

            // Calculate the number of dots
            int numberOfDots = Mathf.CeilToInt(distance / dotSpacing);

            // Set the LineRenderer to the correct number of points
            lineRenderer.positionCount = numberOfDots;

            // Set the positions of each dot
            for (int i = 0; i < numberOfDots; i++)
            {
                Vector2 dotPosition = initialMousePosition + direction * dotSpacing * i;
                lineRenderer.SetPosition(i, dotPosition);
            }

            // float distance = Vector2.Distance(transform.position, currentMousePosition);
            float launchPower = Mathf.Clamp(distance, 1f, maxPower); // Calculate launch power
            UpdatePowerBar(launchPower);
        }
    }
    void OnMouseUp()
    {
        // Stop dragging, launch the object, and hide the progress bar
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDirection = (mousePosition - (Vector2)transform.position).normalized;

            float distance = Vector2.Distance(transform.position, mousePosition);
            float launchPower = Mathf.Clamp(distance, 1f, maxPower);
           
            LaunchToMousePosition(launchPower);  // Launch the object

            rb.linearVelocity = launchDirection * launchPower;

            isDragging = false;
            if (powerBar != null)
            {
                powerBar.fillAmount = 0f;
                powerBar.gameObject.SetActive(false);
            }
            // Clear the projected path (line)
            lineRenderer.positionCount = 0;
        }
    }
    void UpdatePowerBar(float power)
    {
        if (powerBar != null)
        {
            float fillAmount = power / maxPower; // Normalize the power value and update the fill amount
            powerBar.fillAmount = fillAmount;

            Color fillColor = Color.Lerp(Color.white, Color.green, fillAmount);
            powerBar.color = fillColor;
        }
    }
    private void LaunchToMousePosition(float launchPower)
    {
        if (!isDragging) return; // Ensure the method only executes during a drag operation

        // Get the mouse position in world space
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction and distance to determine the launch vector
        Vector2 launchDirection = (mousePosition - (Vector2)transform.position).normalized;
        Debug.Log("Launch Power: " + launchPower);

        // Set the velocity of the Rigidbody2D
        rb.linearVelocity = launchDirection * launchPower;
        Debug.Log("Linear Velocity: " + rb.linearVelocity);

        // Reset the dragging state and hide the power bar
        isDragging = false;
        if (powerBar != null)
        {
            powerBar.fillAmount = 0f;
            powerBar.gameObject.SetActive(false);
        }
    }
}
