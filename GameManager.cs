using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject GameOverPanel; // GameOver panel reference
    public GameObject GameOverText; // GameOver text reference

    public GameObject ResetButton; // GameOver text reference

    public GameObject ReplayButton; // GameOver text reference
    private bool gameOver = false;

    private void Awake()
    {
        // Ensure GameManager is activated on scene load
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent multiple instances
        }

        // Ensure GameManager is active even if the canvas starts inactive
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }



    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            GameOverPanel.SetActive(true); // Show the game over UI
            GameOverText.SetActive(true); // Show the game over text
            ResetButton.SetActive(true); // Show the reset button
            ReplayButton.SetActive(true); // Show the replay button
            Time.timeScale = 0f; // Pause the game
            Debug.Log($"GameOver called. Player score: {PlayerLauncher.score}, Best score before update: {TitleScreenManager.bestScore}");


            if (PlayerLauncher.score > TitleScreenManager.bestScore)
            {
                TitleScreenManager.bestScore = PlayerLauncher.score;
                PlayerPrefs.SetInt("BestScore", TitleScreenManager.bestScore);
                PlayerPrefs.Save();
            }
        }
    }

    public void ResetGame()
    {
        gameOver = false;
        GameOverPanel.SetActive(false); // Hide the GameOver panel
        GameOverText.SetActive(false); // Hide the GameOver text
        ResetButton.SetActive(false); // Hide the reset button
        ReplayButton.SetActive(false); // Hide the replay button
        // Reset game state here
        Time.timeScale = 1f; // Resume the game
        PlayerLauncher.lives = 3; // Reset lives
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
