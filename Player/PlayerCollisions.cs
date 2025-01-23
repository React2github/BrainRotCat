using UnityEngine;
using TMPro;
public enum BoxType
{
    AddPoints,
    MultiplyPoints,
    RemoveLife
}
public class PlayerCollisions : MonoBehaviour
{
    public BoxType boxType;
    public static int score = 0; // Variable to keep track of the score
    public static int lives = 3; // Variable to keep track of the lives
    public TextMeshProUGUI scoreText; // Reference to the UI Text element
    public TextMeshProUGUI livesText; // Reference to the UI Text element
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        UpdateScoreText(); // Initialize the score display
    }

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
        UpdateScoreText();

        if (lives <= 0)
        {
            gameManager.GameOver();
        }
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
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;
    }
}
