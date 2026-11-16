using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    float MasterVolume = 0.4f;
    float MusicVolume = 0.6f;
    float SFXVolume = 0.7f;

    string masterExposed = "MasterVol";
    string musicExposed = "MusicVol";
    string sfxExposed = "SFXVol";
    private void Start()
    {
        if ((PlayerPrefs.HasKey(musicExposed) && PlayerPrefs.HasKey(sfxExposed) && PlayerPrefs.HasKey(masterExposed)))
        {
            LoadVolume();
        }
        else
        {
            masterSlider.value = MasterVolume;
            musicSlider.value = MusicVolume;
            SFXSlider.value = SFXVolume;
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    } 
    public void SetMasterVolume()
    {
       MasterVolume = masterSlider.value;
       mixer.SetFloat(masterExposed, Mathf.Log10(MasterVolume) * 20);
       PlayerPrefs.SetFloat(masterExposed, MasterVolume);
    }
    public void SetMusicVolume()
    {
       MusicVolume = musicSlider.value;
       mixer.SetFloat(musicExposed, Mathf.Log10(MusicVolume) * 20);
       PlayerPrefs.SetFloat(musicExposed, MusicVolume);
    }

    public void SetSFXVolume()
    {
        SFXVolume = SFXSlider.value;
        mixer.SetFloat(sfxExposed, Mathf.Log10(MusicVolume) * 20);
        PlayerPrefs.SetFloat(sfxExposed, SFXVolume);
    }
    public void LoadVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat(masterExposed);
        masterSlider.value = MasterVolume;
        MusicVolume = PlayerPrefs.GetFloat(musicExposed);
        musicSlider.value = MusicVolume;
        SFXVolume = PlayerPrefs.GetFloat(sfxExposed);
        SFXSlider.value = SFXVolume;

        SaveVolumeSettings();
    }
    public void SaveVolumeSettings()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}
