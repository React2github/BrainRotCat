using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject GameOverPanel; // GameOver panel reference
    public GameObject GameOverText; // GameOver text reference
    public GameObject ResetButton; // GameOver text reference
    public GameObject ReplayButton; // GameOver text reference
    public GameObject GameOverCanvas; // GameOver canvas reference
    private bool gameOver = false;

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            GameOverCanvas.SetActive(true); // Show the game over UI
            GameOverPanel.SetActive(true); // Show the game over UI
            GameOverText.SetActive(true); // Show the game over text
            ResetButton.SetActive(true); // Show the reset button
            ReplayButton.SetActive(true); // Show the replay button

            Time.timeScale = 0f; // Pause the game

            UpdateBestScoreText(); // Update the best score text
            PlayerCollisions.lives = 3; // Reset lives
            PlayerCollisions.score = 0; // Reset score
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
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

    public void ResetGame()
    {
        gameOver = false;
        GameOverPanel.SetActive(false); // Hide the GameOver panel
        GameOverText.SetActive(false); // Hide the GameOver text
        ResetButton.SetActive(false); // Hide the reset button
        ReplayButton.SetActive(false); // Hide the replay button


        Time.timeScale = 1f; // Resume the game
        PlayerCollisions.lives = 3; // Reset lives
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
