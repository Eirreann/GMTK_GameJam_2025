using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public AudioMixer Mixer;
    
    [Header("Panels")]
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject IntroPanel;
    
    [Header("Buttons")]
    public Button StartBtn;
    public Button SettingsBtn;
    public Button CreditsBtn;
    public Button ContinueBtn;
    public Button ExitBtn;

    private void Start()
    {
        StartBtn.onClick.AddListener(_start);
        SettingsBtn.onClick.AddListener(_settings);
        CreditsBtn.onClick.AddListener(_credits);
        ContinueBtn.onClick.AddListener(_continue);
        ExitBtn.onClick.AddListener(_exit);
        
        Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
    }

    private void _start()
    {
        IntroPanel.SetActive(true);
    }

    private void _settings()
    {
        SettingsPanel.SetActive(true);
    }

    private void _credits()
    {
        CreditsPanel.SetActive(true);
    }

    private void _continue()
    {
        SceneManager.LoadScene(1);
    }

    private void _exit()
    {
        Application.Quit();
    }
}
