using UnityEngine;
using UnityEngine.UI; // For accessing UI elements
using TMPro;
using UnityEngine.SceneManagement;

public enum BoxType
{
    AddPoints,
    MultiplyPoints,
    RemoveLife
}

public class PlayerLauncher : MonoBehaviour
{
    public float maxPower = 20f; // Maximum launch power
    private Vector2 initialPosition = new Vector2(-3f, 0f); // Always start at x = -3
    private Rigidbody2D rb;

    public BoxType boxType;
    public float rightEdge = 8.39f; // Right edge position where the sphere resets
    public float leftEdge = -8.39f;
    public float topEdge = 4.39f; // Top edge position where the sphere bounces back
    public float bottomEdge = -4.39f; // Bottom edge position where the sphere bounces

    public TextMeshProUGUI scoreText; // Reference to the UI Text element
    public TextMeshProUGUI livesText; // Reference to the UI Text element

    public static int score = 0; // Variable to keep track of the score
    public static int lives = 3; // Variable to keep track of the lives



    private SpriteRenderer spriteRenderer;  // Reference to the SpriteRenderer component

    private void Start()
    {
        transform.position = initialPosition; // Ensure the sphere starts at the left side
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D

        UpdateScoreText(); // Initialize the score display

        Invoke("GameOver", 3f); // Simulate game over after 3 seconds



        Debug.Log("SpriteRenderer: " + spriteRenderer);

        // Modify color based on boxType

    }

    private void Update()
    {
        // Detect mouse click to launch the sphere
        if (Input.GetMouseButtonDown(0))
        {
            LaunchToMousePosition();
        }

        if (transform.position.x <= leftEdge && rb.linearVelocity.x < 0)
        {
            ReverseHorizontalDirection();
        }

        if (transform.position.x >= rightEdge && rb.linearVelocity.x > 0)
        {
            ResetSpherePosition();
        }

        if (transform.position.y <= bottomEdge && rb.linearVelocity.y < 0)
        {
            ReverseVerticalDirection();
        }

        if (transform.position.y >= topEdge && rb.linearVelocity.y > 0)
        {
            ReverseVerticalDirection();
        }
    }


    private void LaunchToMousePosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 launchDirection = (mousePosition - (Vector2)transform.position).normalized;

        float distance = Vector2.Distance(transform.position, mousePosition);
        float launchPower = Mathf.Clamp(distance, 1f, maxPower);

        rb.linearVelocity = launchDirection * launchPower;
    }

    private void ReverseHorizontalDirection()
    {
        rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
    }

    private void ReverseVerticalDirection()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);
    }

    private void ResetSpherePosition()
    {
        transform.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
    }

    // Handle box collision logic
    public void OnBoxCollision(BoxType type)
    {
        switch (type)
        {
            case BoxType.AddPoints:
                score += 10;
                break;
            case BoxType.MultiplyPoints:
                score += 100;
                break;
            case BoxType.RemoveLife:
                lives -= 1;
                break;
        }
        UpdateScoreText();  // Update UI
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }

    }

    private void UpdateScoreText()
    {
        // Update the score display in the UI
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;
    }
}
