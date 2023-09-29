using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BackgroundSoundManager : MonoBehaviour
{
    [Header("Background Snapshots")]
    public AudioMixerSnapshot background1Snapshot; // Reference to the first snapshot
    public AudioMixerSnapshot background2Snapshot; // Reference to the second snapshot
    public AudioMixerSnapshot silenceSnapshot; // Reference to the snapshot for silence
    [Header("Background Clips")]
    public AudioClip[] backgroundSounds; // Array of background sounds
    public float transitionTime = 2.0f; // Duration of the transition in seconds
    public float silenceProbability = 0.1f; // Probability of silence
    [Header("Background Audio Source")]
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    private bool isUsingAudioSource1 = true;
    private bool isSilenceAtStart = true;

    private void Start()
    {
        background1Snapshot.TransitionTo(1);
        // Start playing the first background sound in source 1
        PlayNextSound(audioSource1);
        StartCoroutine(PlayRandomBackgroundSound());
    }

    private IEnumerator PlayRandomBackgroundSound()
    {
        while (true)
        {
            // Determine which audio source is currently active
            AudioSource currentSource = isUsingAudioSource1 ? audioSource1 : audioSource2;
            // Play the next sound on the current audio source
            PlayNextSound(currentSource);
            // Determine if silence should be played
            if (Random.value < silenceProbability && !isSilenceAtStart)
            {
                TransitionToSilence();
            }
            float soundDuration = currentSource.clip != null ? currentSource.clip.length : 0f;
            yield return new WaitForSeconds(soundDuration - (transitionTime-5));
            AudioMixerSnapshot currentSnapshot = isUsingAudioSource1 ? background1Snapshot : background2Snapshot;
            AudioMixerSnapshot nextSnapshot = isUsingAudioSource1 ? background2Snapshot : background1Snapshot;

            // Transition to the next snapshot
            nextSnapshot.TransitionTo(transitionTime);

            // Wait for the specified transition time
            yield return new WaitForSeconds(transitionTime);

            // Toggle the active audio source
            isUsingAudioSource1 = !isUsingAudioSource1;
            isSilenceAtStart = false;
        }
    }
    private void PlayNextSound(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = backgroundSounds[Random.Range(0, backgroundSounds.Length)];
        audioSource.Play();
    }
    private void TransitionToSilence()
    {
        silenceSnapshot.TransitionTo(transitionTime);
    }
}
