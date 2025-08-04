using System.Collections;
using DG.Tweening;
using Game;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioMixer Mixer;

    [Header("Sources")]
    public AudioSource MusicStartSource;
    public AudioSource MusicLoopSource;
    public AudioSource SFXSource;

    [Header("Sound Effects")]
    public AudioClip TakeDamage;
    public AudioClip Pickup;
    public AudioClip CarryLoop;
    public AudioClip Deposit;
    
    [Header("Music")]
    public AudioClip Part1Intro;
    public AudioClip Part1Loop;
    
    public AudioClip Part2Intro;
    public AudioClip Part2Loop;

    private float _musicVolume;

    private void Start()
    {
        Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
        
        _musicVolume = MusicStartSource.volume;
        
        StartCoroutine(_startMusic(Part1Intro, Part1Loop));
    }

    private void Update()
    {
        
    }

    

    public void OnTakeDamage(AudioSource source)
    {
        SFXSource.PlayOneShot(TakeDamage);
    }

    public void OnPickupRope()
    {
        SFXSource.PlayOneShot(Pickup);
        SFXSource.clip = CarryLoop;
        SFXSource.loop = true;
        SFXSource.Play();
    }

    public void OnDropRope()
    {
        SFXSource.loop = false;
        SFXSource.Stop();
    }

    public void OnDepositRope()
    {
        SFXSource.loop = false;
        SFXSource.Stop();
        SFXSource.PlayOneShot(Deposit);
    }
    
    public void ProgressMusic()
    {
        StartCoroutine(_transitionMusic());
    }

    private IEnumerator _startMusic(AudioClip intro, AudioClip loop)
    {
        MusicStartSource.clip = intro;
        MusicLoopSource.clip = loop;
        
        MusicStartSource.Play();
        
        yield return new WaitForSecondsRealtime(intro.length - .5f);
        
        Debug.Log("Switching to loop track.");
        
        MusicStartSource.DOFade(0, 1.5f);
        MusicLoopSource.DOFade(_musicVolume, 1.5f);
        
        MusicLoopSource.Play();
    }

    private IEnumerator _transitionMusic()
    {
        MusicLoopSource.DOFade(0, 2f);
        yield return new WaitForSecondsRealtime(2f);
        
        MusicStartSource.DOFade(_musicVolume, 2f);
        
        StartCoroutine(_startMusic(Part2Intro, Part2Loop));
    }
}
