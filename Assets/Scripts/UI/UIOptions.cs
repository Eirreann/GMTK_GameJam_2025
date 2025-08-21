using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIOptions : MonoBehaviour
    {
        [Header("Audio")]
        public AudioMixer Mixer;
        public Slider Master;
        public Slider Music;
        public Slider SFX;

        [Header("Video")] 
        private List<Resolution> _resolutions;
        
        public Slider fpsSlider;
        public TextMeshProUGUI fpsDisplay;
        public Toggle vSyncToggle;
        public Toggle fullscreenToggle;
        public Button applyChangesButton;
        public TMP_Dropdown resolutionDropdown;

        public GameObject confirmSettingsPanel;
        public float Timer;
        public TextMeshProUGUI timerText;
        
        public Button confirmButton;
        public Button revertButton;
        
        [Header("UI Elements")]
        public GameObject resolutionSelectObject;
        public GameObject fullscreenToggleObject;
        public GameObject fpsSliderOption;
        public GameObject vsyncToggleObject;
        
        UISelectionHelper selectionHelper;
        public Button closeButton;

        public void Start()
        {
            Debug.Log(Master);
            
            selectionHelper = FindFirstObjectByType<UISelectionHelper>();
            
            if(Master) Master.onValueChanged.AddListener(_updateMasterAudio);
            if(Music) Music.onValueChanged.AddListener(_updateMusicAudio);
            if(SFX) SFX.onValueChanged.AddListener(_updateSFXAudio);
            
            if(fpsSlider) fpsSlider.onValueChanged.AddListener(_updateFPSSlider);
            if(vSyncToggle) vSyncToggle.onValueChanged.AddListener(_updateVSyncToggle);
            if(fullscreenToggle) fullscreenToggle.onValueChanged.AddListener(_updateFullscreenToggle);
            
            if(resolutionDropdown) resolutionDropdown.onValueChanged.AddListener(_updateResolutionDropdown);

            if(applyChangesButton) applyChangesButton.onClick.AddListener(_applyGraphicsChanges);
            
            if(closeButton) closeButton.onClick.AddListener(SetInitialVideoValues);
            
            if(confirmButton) confirmButton.onClick.AddListener(_confirmGraphicsChanges);
            if(revertButton) revertButton.onClick.AddListener(_revertGraphicsChanges);

            Master.value = PlayerPrefs.GetFloat("Master", 1f);
            Music.value = PlayerPrefs.GetFloat("Music", 1f);
            SFX.value = PlayerPrefs.GetFloat("SFX", 1f);

            SetInitialVideoValues();
            
            #if UNITY_WEBGL
            
                resolutionSelectObject.SetActive(false);
                fullscreenToggleObject.SetActive(false);
            
            #endif
        }

        public void Update()
        {
            if (confirmSettingsPanel && confirmSettingsPanel.activeSelf)
            {
                if (Timer > 0)
                {
                    Timer -= Time.unscaledDeltaTime;
                    timerText.text = $"You have {(int)Timer} seconds to confirm.";
                }

                if (Timer <= 0)
                {
                    SetInitialVideoValues();
                }
            }
        }

        public void SetInitialVideoValues()
        {
            
            fpsSlider.value = PlayerPrefs.GetInt("maxFPS", 30);
            fpsDisplay.text = $"{fpsSlider.value}";
            
            vSyncToggle.isOn = PlayerPrefs.GetInt("vSync", 0) == 1;
            fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            
            applyChangesButton.interactable = false;
            
            GenerateScreenResolutions();
            
            if(confirmSettingsPanel.activeSelf) confirmSettingsPanel.SetActive(false);
        }

        private void GenerateScreenResolutions()
        {
            _resolutions = Screen.resolutions.ToList();
            _resolutions.Reverse();
            
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for(int i = 0; i < _resolutions.Count; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height + $" : { _resolutions[i].refreshRateRatio } Hz";
                
                options.Add(option);
                
                if(_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void _applyGraphicsChanges()
        {
            Application.targetFrameRate = (int)fpsSlider.value;
            QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
            SetResolution(resolutionDropdown.value);
            
            applyChangesButton.interactable = false;
            
            Timer = 15f;
            
            confirmSettingsPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        }

        private void _confirmGraphicsChanges()
        {
            PlayerPrefs.SetInt("maxFPS", (int)fpsSlider.value);
            PlayerPrefs.SetInt("vSync", vSyncToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
            
            confirmSettingsPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }

        private void _revertGraphicsChanges()
        {
            SetInitialVideoValues();
            
            confirmSettingsPanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(closeButton.gameObject);
        }
        
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed, resolution.refreshRateRatio);
        }

        private void _updateMasterAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Master", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Master", value);
        }

        private void _updateMusicAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Music", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Music", value);
        }

        private void _updateSFXAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("SFX", value);
        }

        private void _updateFPSSlider(float value)
        {
            applyChangesButton.interactable = true;
            fpsDisplay.text = $"{value}";
        }

        private void _updateFullscreenToggle(bool value)
        {
            applyChangesButton.interactable = true;
            fullscreenToggle.isOn = value;
        }

        private void _updateVSyncToggle(bool value)
        {
            applyChangesButton.interactable = true;
            vSyncToggle.isOn = value;
        }

        private void _updateResolutionDropdown(int value)
        {
            applyChangesButton.interactable = true;
            Resolution resolution = _resolutions[value];
            
            fpsSlider.maxValue = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);
            if (fpsSlider.value > fpsSlider.maxValue)
            {
                fpsSlider.value = fpsSlider.maxValue;
            }
        }
    }
}