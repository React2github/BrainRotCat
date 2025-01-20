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

    public GameObject BestScoreText;

    public static int bestScore = 10;



    // public GameObject PlayerSphere; 

    private void Start()
    {
        // Ensure the title canvas is active at the start of the game
        TitleCanvas.SetActive(true);
        TitlePanel.SetActive(true);
        ScoreText.SetActive(false);
        LivesText.SetActive(false);
        // PlayerSphere.SetActive(false);

        Time.timeScale = 0f; // Pause the game

        // Set up the button click listener
        playButton.onClick.AddListener(StartGame);

        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        Debug.Log($"Best score loaded: {bestScore}");
        // Update the best score text at the start
        UpdateBestScoreText();
    }

    public void ShowTitleScreen()
    {
        // Display the title screen
        TitleCanvas.SetActive(true);

        // Pause the game while on the title screen (if you want)
        Time.timeScale = 0f;

        // Update the best score text
        UpdateBestScoreText();

    }

    private void UpdateBestScoreText()
    {
        if (BestScoreText != null)
        {
            // Get the TMP_Text component from the GameObject
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
    // Method to start the game

    public void StartGame()
    {
        // Hide the title screen and unpause the game
        TitleCanvas.SetActive(false);
        TitleButton.SetActive(false);
        TitlePanel.SetActive(false);

        ScoreText.SetActive(true);
        LivesText.SetActive(true);
        // PlayerSphere.SetActive(true);
        Time.timeScale = 1f;



    }
}
