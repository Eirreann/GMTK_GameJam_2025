using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    
    [Header("Buttons")]
    public Button StartBtn;
    public Button SettingsBtn;
    public Button CreditsBtn;

    private void Start()
    {
        StartBtn.onClick.AddListener(_start);
        SettingsBtn.onClick.AddListener(_settings);
        CreditsBtn.onClick.AddListener(_credits);
    }

    private void _start()
    {
        SceneManager.LoadScene(1);
    }

    private void _settings()
    {
        SettingsPanel.SetActive(true);
    }

    private void _credits()
    {
        CreditsPanel.SetActive(true);
    }

}
