using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Windows")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [Header("MainMenu")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private TextMeshProUGUI highestStreakText;
    [SerializeField] private TextMeshProUGUI mainMenuText;
    [Header("Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button settingsBackButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        settingsBackButton.onClick.AddListener(OnBackButtonClick);
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderValueChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderValueChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeSliderValueChanged);

        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);

        if (SerializeManager.Instance.GetBool(BoolType.FirstPlay))
            SerializeManager.Instance.SetFloat(FloatType.HighestStreak, 0f);
    }

    private void Update()
    {
        mainMenuText.rectTransform.localScale = Vector3.one + (new Vector3(Mathf.Sin(Time.time * 2f), Mathf.Sin(Time.time * 2f), Mathf.Sin(Time.time * 2f)) * 0.02f);

        highestStreakText.text = "highest streak: " + SerializeManager.Instance.GetFloat(FloatType.HighestStreak);
    }

    private void OnPlayButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    private void OnSettingsButtonClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);

        masterVolumeSlider.value = SerializeManager.Instance.GetFloat(FloatType.MasterVolume);
        musicVolumeSlider.value = SerializeManager.Instance.GetFloat(FloatType.MusicVolume);
        sfxVolumeSlider.value = SerializeManager.Instance.GetFloat(FloatType.SfxVolume);
    }

    private void OnBackButtonClick()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    private void OnMasterVolumeSliderValueChanged(float value)
    {
        SerializeManager.Instance.SetFloat(FloatType.MasterVolume, value);
    }

    private void OnMusicVolumeSliderValueChanged(float value)
    {
        SerializeManager.Instance.SetFloat(FloatType.MusicVolume, value);
    }

    private void OnSfxVolumeSliderValueChanged(float value)
    {
        SerializeManager.Instance.SetFloat(FloatType.SfxVolume, value);
    }
}
