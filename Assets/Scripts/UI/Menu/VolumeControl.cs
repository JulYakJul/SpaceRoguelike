using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1.0f);
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        }
    }

    private void OnVolumeChange(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVolume(volume);
        }
    }
}
