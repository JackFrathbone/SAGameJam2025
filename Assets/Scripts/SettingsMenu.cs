using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{    
    public Slider musicSlider, sfxSlider, mouseSensitivitySlider;
    public AudioMixer audioMixer;

    private PlayerController playerController;

    private void OnEnable()
    {
        ShowPanel();
    }
    public void ShowPanel()
    {
        
        audioMixer.GetFloat("musicVol", out float musVol);
        musicSlider.value = MixerToSlider(musVol);

        audioMixer.GetFloat("sfxVol", out float sfxVol);
        sfxSlider.value = MixerToSlider(sfxVol);

        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 4);

        playerController = FindFirstObjectByType<PlayerController>();
    }

    public float SliderToMixer(float sliderValue)
    {
        return Mathf.Log10(sliderValue) * 20;
    }

    public float MixerToSlider(float mixerValue)
    {
        return Mathf.Pow(10, (mixerValue / 20f));
    }

    public void ShowPanelClick()
    {
        ShowPanel();
    }

    public void HidePanel()
    {
        //Close menu
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("musicVol", SliderToMixer(musicSlider.value));
    }

    public void ChangeSFXVolume()
    {
        audioMixer.SetFloat("sfxVol", SliderToMixer(sfxSlider.value));
    }

    public void ChangeMouseSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
        if (playerController != null)
        {
            playerController.RefreshMouseSensitivity();
        }
    }

    public void ExitToMenu()
    {
        
        //SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (openButton.gameObject.activeInHierarchy)
        //        ShowPanel();
        //    else
        //        HidePanel();
        //}
    }
}
