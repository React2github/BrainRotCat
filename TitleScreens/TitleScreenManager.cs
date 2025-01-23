using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class TitleScreenManager : MonoBehaviour
{
    public GameObject TitleCanvas;  // Reference to the title canvas
    public GameObject TitleButton;  // Reference to the title button
    public Button playButton;       // Reference to the play button
    public GameObject TitlePanel;   // Reference to the title panel
    public GameObject ScoreText;    // Reference to the score text
    public GameObject LivesText;    // Reference to the lives text
    public GameObject boxPrefab;     // Reference to the box prefab
    // public GameObject PowerBar;     // Reference to the power bar
    public GameObject BestScoreText;
    // public GameObject HUDObject;
    // public Button HUDButton; // Reference to the HUD button
    public static int bestScore = 10;
    public BoxSpawner boxSpawner; 

    private void Start()
    {
        Debug.Log("TitleScreenManager started.");
        TitleCanvas.SetActive(true); // Ensure the title canvas is active at the start of the game
        TitlePanel.SetActive(true);

        // PowerBar.SetActive(false);

        boxSpawner = FindObjectOfType<BoxSpawner>();
        Time.timeScale = 0f; // Pause the game
        playButton.onClick.AddListener(StartGame);  // HUDButton.onClick.AddListener(ResetGame);
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Debug.Log($"Best score loaded: {bestScore}");
        UpdateBestScoreText(); // Update the best score text at the start
    }

    public void ShowTitleScreen()
    {
        TitleCanvas.SetActive(true); // Display the title screen
        Time.timeScale = 0f;  // Pause the game while on the title screen (if you want)
        UpdateBestScoreText(); // Update the best score text
    }

    private void UpdateBestScoreText()
    {
        if (BestScoreText != null)
        {
            TMP_Text bestScoreTextComponent = BestScoreText.GetComponent<TMP_Text>();
            if (bestScoreTextComponent != null)
            {
                bestScoreTextComponent.text = $"Best Score: {bestScore}";
                Debug.Log($"Best score displayed: {bestScore}");
            }
            else
            {
                Debug.LogWarning("TMP_Text component not found on BestScoreText GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("BestScoreText reference is null.");
        }
    }

    public void StartGame()
    {
        TitleCanvas.SetActive(true);  // Hide the title screen and unpause the game
        TitleButton.SetActive(false);
        TitlePanel.SetActive(false);
        // PowerBar.SetActive(false);

        ScoreText.SetActive(true);
        LivesText.SetActive(true);
        Time.timeScale = 1f;
        boxSpawner.SpawnBoxes();
    }

    public void ResetGame()
    {
        PlayerCollisions.score = 0;
        PlayerCollisions.lives = 3;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
