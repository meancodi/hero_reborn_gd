using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1f);
        }

        Load();
    }

    public void ChangeVolume(float value)
    {
        Debug.Log("Changed to " + value);
        AudioListener.volume = value;
    }

    private void Load()
    {
        float v = PlayerPrefs.GetFloat("musicVolume");
        volumeSlider.value = v;
        AudioListener.volume = v;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}