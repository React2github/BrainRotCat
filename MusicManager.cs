using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource; // Drag Audio Source here
    public Button playMusicButton;  // Drag UI Button here

    void Start()
    {
        playMusicButton.onClick.AddListener(PlayMusic);
    }

    void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
