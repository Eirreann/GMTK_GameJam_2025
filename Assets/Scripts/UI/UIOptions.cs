using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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
        public Slider fpsSlider;
        public TextMeshProUGUI fpsDisplay;

        public Toggle vSyncToggle;
        

        public void Start()
        {
            Master.onValueChanged.AddListener(_updateMasterAudio);
            Music.onValueChanged.AddListener(_updateMusicAudio);
            SFX.onValueChanged.AddListener(_updateSFXAudio);
            fpsSlider.onValueChanged.AddListener(_updateFPSSlider);
            vSyncToggle.onValueChanged.AddListener(_updateVSyncToggle);

            Master.value = PlayerPrefs.GetFloat("Master", 1f);
            Music.value = PlayerPrefs.GetFloat("Music", 1f);
            SFX.value = PlayerPrefs.GetFloat("SFX", 1f);
            
            Application.targetFrameRate = PlayerPrefs.GetInt("maxFPS", 2) * 30;
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("vSync", 0);
        }

        private void _updateMasterAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Master", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Master", value);
            //Debug.Log(value);
        }

        private void _updateMusicAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Music", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Music", value);
            //Debug.Log(value);
        }

        private void _updateSFXAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("SFX", value);
            //Debug.Log(value);
        }

        private void _updateFPSSlider(float value)
        {
            float baseValue = 30f;
            
            Application.targetFrameRate = (int)(baseValue * value);
            PlayerPrefs.SetInt("maxFPS", (int)value);

            fpsDisplay.text = $"{baseValue * value}";
            //Debug.Log(value);
        }

        private void _updateVSyncToggle(bool value)
        {
            vSyncToggle.isOn = value;
            
            QualitySettings.vSyncCount = value ? 1 : 0;
            PlayerPrefs.SetInt("vSync", value ? 1 : 0);
            
            Debug.Log(value);
        }
    }
}