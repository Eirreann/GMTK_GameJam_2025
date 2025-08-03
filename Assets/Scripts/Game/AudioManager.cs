using System.Collections;
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

    private void Start()
    {
        Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
        Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
        Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
    }

    private IEnumerator _transitionMusic()
    {
        yield return null;
        // TODO: Fade out LoopSource over 3 secs
        // TODO: Fade in StartSource after 2 secs
    }

    public void OnTakeDamage(AudioSource source)
    {
        SFXSource.PlayOneShot(TakeDamage);
    }
}
