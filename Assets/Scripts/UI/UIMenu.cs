using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject IntroPanel;
    
    [Header("Buttons")]
    public Button StartBtn;
    public Button SettingsBtn;
    public Button CreditsBtn;
    public Button ContinueBtn;

    private void Start()
    {
        StartBtn.onClick.AddListener(_start);
        SettingsBtn.onClick.AddListener(_settings);
        CreditsBtn.onClick.AddListener(_credits);
        ContinueBtn.onClick.AddListener(_continue);
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
}
