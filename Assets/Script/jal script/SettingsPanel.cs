using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle fullscreenToggle;

    public void ApplySaved()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVol", 0.75f);
        fullscreenToggle.isOn = Screen.fullScreen;
        SetMusic(musicSlider.value);
        SetSfx(sfxSlider.value);
    }

    public void SetMusic(float v) => SetVolume("MusicVol", v);
    public void SetSfx(float v)   => SetVolume("SfxVol", v);
    public void SetFullscreen(bool on) => Screen.fullScreen = on;

    void SetVolume(string param, float v)
    {
        mixer.SetFloat(param, Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f); // slider 0-1 -> decibels
        PlayerPrefs.SetFloat(param, v);
    }
}