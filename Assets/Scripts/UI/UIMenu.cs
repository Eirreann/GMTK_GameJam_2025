using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer Mixer;

    [Header("Panels")]
    public GameObject StartMenu;
    public GameObject MainMenuContainer;
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject IntroPanel;
    
    [Header("Buttons")]
    public Button StartBtn;
    public Button ExitBtn;
    
    public Button SettingsBtn;
    public Button CreditsBtn;
    public Button ContinueBtn;

    [Header("Back Buttons")]
    public Button SettingsBackBtn;
    public Button CreditsBackBtn;
    
    [Header("UI Helper")]
    public UISelectionHelper _uiSelectionHelper;
    
    [Header("Build Version")]
    public TextMeshProUGUI buildVersion;

    private void Start()
    {
        Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
        
        Application.targetFrameRate = PlayerPrefs.GetInt("maxFPS", 30);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync", 0);
        
        _uiSelectionHelper = GetComponent<UISelectionHelper>();
        _uiSelectionHelper.AddActivePanel(MainMenuContainer);

        StartBtn.onClick.AddListener(_start);
        SettingsBtn.onClick.AddListener(_settings);
        CreditsBtn.onClick.AddListener(_credits);
        ContinueBtn.onClick.AddListener(_continue);
        ExitBtn.onClick.AddListener(_exit);
        
        buildVersion.text = Debug.isDebugBuild ? "Development build" : "Production build";
        buildVersion.text += "\nApp version " + Application.version + ", Unity version " + Application.unityVersion;
    }

    

    private void Update()
    {

        if (_uiSelectionHelper._inputSystem.UI.Submit.WasPressedThisFrame() && StartMenu.activeSelf)
        {
            StartMenu.SetActive(false);
            MainMenuContainer.SetActive(true);
            
            _uiSelectionHelper.AddActivePanel(MainMenuContainer);
            
            _uiSelectionHelper.SetLastSelected(ContinueBtn.gameObject);
            _uiSelectionHelper.GrabLastSelectedButton();
        }
    }
    
    public void SetMenuButtons(bool _state)
    {
        _uiSelectionHelper.tint.enabled = !_state;
        MainMenuContainer.SetActive(_state);
    }

    private void _start()
    {
        IntroPanel.SetActive(true);
        
        _uiSelectionHelper.AddActivePanel(IntroPanel);
        _uiSelectionHelper.SetLastSelected(StartBtn.gameObject);

        SetMenuButtons(false);
        
        EventSystem.current.SetSelectedGameObject(ContinueBtn.gameObject);
    }

    private void _settings()
    {
        SettingsPanel.SetActive(true);
        
        _uiSelectionHelper.AddActivePanel(SettingsPanel);
        _uiSelectionHelper.SetLastSelected(SettingsBtn.gameObject);
        
        SetMenuButtons(false);
        
        EventSystem.current.SetSelectedGameObject(SettingsBackBtn.gameObject);
    }

    private void _credits()
    {
        CreditsPanel.SetActive(true);
        
        _uiSelectionHelper.AddActivePanel(CreditsPanel);
        _uiSelectionHelper.SetLastSelected(CreditsBtn.gameObject);
        
        SetMenuButtons(false);
        
        EventSystem.current.SetSelectedGameObject(CreditsBackBtn.gameObject);
    }

    private void _continue()
    {
        _uiSelectionHelper._inputSystem.UI.Disable();
        SceneManager.LoadScene(1);
    }

    private void _exit()
    {
        Application.Quit();
    }
}
}
