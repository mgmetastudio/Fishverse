using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
public class MixerManager : MonoBehaviour
{
    private static MixerManager instance;

    public AudioMixer mixer;
    public AudioSource AudioWaitingroom;
    public AudioSource AudioBackground;

    [SerializeField] private AudioMixerSnapshot whenInWaiting, WhenInmenu, WhenInEmpty;

    // Public property to access the singleton instance

    public static MixerManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        // Check if an instance already exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Set the instance and mark it to not be destroyed on load
        instance = this;
    }

    public void Start()
    {
        EnableMainMenuSound();
    }

    public void UpdateAudioSource(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.None:
                // Stop All audio source
                WhenInEmpty.TransitionTo(3);
                StartCoroutine(UpdateAudioSourceState(3, AudioWaitingroom, false));
                StartCoroutine(UpdateAudioSourceState(3, AudioBackground, false));
                break;
            case SoundType.MainMenu:
                // Enable MainMenu Audio Source
                WhenInmenu.TransitionTo(3);
                StartCoroutine(UpdateAudioSourceState(3, AudioWaitingroom, false));
                StartCoroutine(UpdateAudioSourceState(0, AudioBackground, true));
                break;
            case SoundType.WaitingMenu:
                // Enable Waiting Menu Audio Source
                whenInWaiting.TransitionTo(3);
                StartCoroutine(UpdateAudioSourceState(0, AudioWaitingroom, true));
                StartCoroutine(UpdateAudioSourceState(3, AudioBackground, false));
                break;
            case SoundType.Ingame:
                // Enable Waiting Menu Audio Source
                WhenInEmpty.TransitionTo(3);
                StartCoroutine(UpdateAudioSourceState(3, AudioWaitingroom, false));
                StartCoroutine(UpdateAudioSourceState(3, AudioBackground, false));
                break;
            default:
                break;
        }
    }

    private IEnumerator UpdateAudioSourceState(float time, AudioSource audioSource, bool play)
    {
        yield return new WaitForSeconds(time);
        if (play)
        {
            audioSource.Play();
        }
        else
        {
           // audioSource.Stop();
        }
    }

    public void EnableMainMenuSound()
    {
        AudioBackground.volume = 0.2f;
        WhenInmenu.TransitionTo(3);
        StartCoroutine(UpdateAudioSourceState(0, AudioWaitingroom, false));
    }
}
public enum SoundType { None, MainMenu, WaitingMenu, Ingame }
