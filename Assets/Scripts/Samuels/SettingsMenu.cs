using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    //Checks if any settings are saved and if so loads them on startup
    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume") || PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    //Makes Master Volume audiomixer work with Master Volume slider
    //and saves the volume value
    public void SetMasterVolume()
    {
        float masterVolume = masterSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume)*20);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
    }

    //Makes Music Volume audiomixer work with Music Volume slider
    //and saves the volume value

    public void SetMusicVolume() 
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume)*20);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

    //Makes SFX Volume audiomixer work with SFX Volume slider
    //and saves the volume value

    public void SetSFXVolume() 
    {
        float sfxVolume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf .Log10(sfxVolume)*20);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
    

    //Loads the volume settings after closing and reopening the game
    //if any settings are saved
    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetMasterVolume();
    }
}
