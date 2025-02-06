using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject GameOverPanel; // GameOver panel reference
    public GameObject GameOverText; // GameOver text reference
    public GameObject ReplayButton; // GameOver text reference
    public GameObject GameOverCanvas; // GameOver canvas reference
    private bool gameOver = false;
    private GameObject spawnerManager; // Reference to SpawnerManager


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        GameOverCanvas.SetActive(true); // Show the game over UI
        FindUIReferences();
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            GameOverCanvas.SetActive(true); // Show the game over UI
            GameOverPanel.SetActive(true); // Show the game over UI
            GameOverText.SetActive(true); // Show the game over text
            ReplayButton.SetActive(true); // Show the replay button

            Time.timeScale = 0f; // Pause the game

            UpdateBestScoreText(); // Update the best score text
            PlayerCollisions.lives = 3; // Reset lives
            PlayerCollisions.score = 0; // Reset score

        }
    }

    public void UpdateBestScoreText()
    {
        Debug.Log($"GameOver called. Player score: {PlayerCollisions.score}, Best score before update: {TitleScreenManager.bestScore}");
        if (PlayerCollisions.score > TitleScreenManager.bestScore)
        {
            TitleScreenManager.bestScore = PlayerCollisions.score;
            PlayerPrefs.SetInt("BestScore", TitleScreenManager.bestScore);
            PlayerPrefs.Save();
        }
    }

    private void FindUIReferences()
    {
        GameOverCanvas = GameObject.Find("GameOverCanvas");
        GameOverPanel = GameOverCanvas?.transform.Find("GameOverPanel")?.gameObject;
        GameOverText = GameOverPanel?.transform.Find("GameOverText")?.gameObject;
        ReplayButton = GameOverPanel?.transform.Find("ReplayButton")?.gameObject;

        if (GameOverPanel == null || GameOverText == null || ReplayButton == null)
        {
            Debug.LogError("One or more UI elements could not be found! Check scene hierarchy.");
        }
    }
    private void FindSpawnerManager()
    {
        spawnerManager = GameObject.Find("SpawnerManager");

        if (spawnerManager != null)
        {
            DontDestroyOnLoad(spawnerManager); // Keep SpawnerManager alive
        }
        else
        {
            Debug.LogError("SpawnerManager not found in the scene!");
        }
    }

    public void ResetGame()
    {
        gameOver = false;
        GameOverPanel.SetActive(false); // Hide the GameOver panel
        GameOverText.SetActive(false); // Hide the GameOver text
        ReplayButton.SetActive(false); // Hide the replay button


        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
