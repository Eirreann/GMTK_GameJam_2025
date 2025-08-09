using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public AudioMixer Mixer;
    public PlayerInput playerInput;

    public GameObject scrollingGround;
    public GameObject threeDimensionalBackground;
    
    public InputSystem_Actions _inputSystem;
    public InputSystem_Actions.UIActions _uiActions;

    [Header("Panels")] 
    public GameObject _activePanel;

    public GameObject MainMenuContainer;
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject ControlsPanel;
    public GameObject IntroPanel;
    
    [Header("Buttons")]
    public Button StartBtn;
    public Button ExitBtn;

    public Button ControlsBtn;
    public Button SettingsBtn;
    public Button CreditsBtn;
    public Button ContinueBtn;

    [Header("Back Buttons")]
    public Button SettingsBackBtn;
    public Button ControlsBackBtn;
    public Button CreditsBackBtn;

    [SerializeField] private GameObject _lastVisitedButton;
    [SerializeField] private GameObject _lastSelectedButton;
    [SerializeField] private String _lastControlScheme;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        _inputSystem = new InputSystem_Actions();

        _inputSystem.UI.Enable();
        
        EventSystem.current.SetSelectedGameObject(StartBtn.gameObject);
        
        _lastVisitedButton = StartBtn.gameObject;
        _lastSelectedButton = StartBtn.gameObject;
        
        StartBtn.onClick.AddListener(_start);
        SettingsBtn.onClick.AddListener(_settings);
        ControlsBtn.onClick.AddListener(_controls);
        CreditsBtn.onClick.AddListener(_credits);
        ContinueBtn.onClick.AddListener(_continue);
        ExitBtn.onClick.AddListener(_exit);
        
        Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
    }

    public void SetLastVisited(GameObject lastVisitedButton)
    {
        _lastVisitedButton = lastVisitedButton;
    }

    private void Update()
    {
        //Move the 3D backdrop
        scrollingGround.transform.Translate(new Vector3(-0.10f * Time.deltaTime, 0, 0));
        threeDimensionalBackground.transform.Translate(new Vector3(-0.05f * Time.deltaTime, 0, 0));
        
        if (scrollingGround.transform.position.x < -48) scrollingGround.transform.Translate(96f, 0, 0);
        if (threeDimensionalBackground.transform.position.x < -11.66f) threeDimensionalBackground.transform.Translate(11.66f, 0, 0);
        
        Debug.Log(threeDimensionalBackground.transform.position.x);
        
        if (!StartBtn.enabled && _inputSystem.UI.Cancel.WasPressedThisFrame())
        {
            _activePanel.SetActive(false);
            SetMenuButtons(true);
            ReturnToLastButton();
        }
        
        if (playerInput.currentControlScheme != _lastControlScheme)
        {
            EventSystem.current.SetSelectedGameObject(_lastVisitedButton);
            _lastControlScheme = playerInput.currentControlScheme;
        }
    }
    
    public void SetMenuButtons(bool _state)
    {
        //StartBtn.enabled = _state;
        //ExitBtn.enabled = _state;
        
        //ControlsBtn.enabled = _state;
        //SettingsBtn.enabled = _state;
        //CreditsBtn.enabled = _state;
        
        MainMenuContainer.SetActive(_state);
    }

    public void ReturnToLastButton()
    {
        EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
    }

    private void _start()
    {
        IntroPanel.SetActive(true);
        _activePanel = IntroPanel;
        
        _lastSelectedButton = StartBtn.gameObject;
        EventSystem.current.SetSelectedGameObject(ContinueBtn.gameObject);
    }

    private void _controls()
    {
        ControlsPanel.SetActive(true);
        _activePanel = ControlsPanel;
        
        _lastSelectedButton = ControlsBtn.gameObject;
        EventSystem.current.SetSelectedGameObject(ControlsBackBtn.gameObject);
    }

    private void _settings()
    {
        SettingsPanel.SetActive(true);
        _activePanel = SettingsPanel;
        
        _lastSelectedButton = SettingsBtn.gameObject;
        EventSystem.current.SetSelectedGameObject(SettingsBackBtn.gameObject);
    }

    private void _credits()
    {
        CreditsPanel.SetActive(true);
        _activePanel = CreditsPanel;
        
        _lastSelectedButton = CreditsBtn.gameObject;
        EventSystem.current.SetSelectedGameObject(CreditsBackBtn.gameObject);
    }

    private void _continue()
    {
        _inputSystem.UI.Disable();
        SceneManager.LoadScene(1);
    }

    private void _exit()
    {
        Application.Quit();
    }
}
