using UnityEngine;
using UnityEngine.UI;

public class MainSettings : MonoBehaviour
{
    public Slider volumeSlider;
    public Button toggleButton;
    private bool isMuted = false;

    void Start()
    {
        // Set the slider value to the current volume
        volumeSlider.value = AudioListener.volume;

        // Add a listener to call the OnVolumeChange method whenever the slider value changes
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);

        // Add a listener to call the ToggleMute method when the button is clicked
        toggleButton.onClick.AddListener(ToggleMute);
    }

    void OnVolumeChange(float value)
    {
        // Change the global volume
        AudioListener.volume = value;
    }

    void ToggleMute()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0 : volumeSlider.value;
    }
}
