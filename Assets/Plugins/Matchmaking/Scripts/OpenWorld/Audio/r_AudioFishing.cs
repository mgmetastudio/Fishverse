using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class r_AudioFishing : MonoBehaviour
{
    #region Variables
    [Header("Audio Source")]
    public AudioSource r_AudioSource_Reeling;
    public AudioSource r_AudioSource_Drag;
    public AudioSource r_AudioSource_Cast;

    [Header("Drag fish Clips")]
    public List <AudioClip> DragFishClips;

    [Header("Rod Cast Clips")]
    public List<AudioClip> RodCastClips;

    [Header("Rod Reeling Clips")]
    public List<AudioClip> RodReelingClips;
    // Dictionary to track playing state for each AudioSource
    private Dictionary<AudioSource, bool> isPlayingDict = new Dictionary<AudioSource, bool>();

    #endregion
    public void Start()
    {
        isPlayingDict[r_AudioSource_Reeling] = false;
        isPlayingDict[r_AudioSource_Drag] = false;
        isPlayingDict[r_AudioSource_Cast] = false;
    }
    public void PlaySound(List<AudioClip> listclip,AudioSource r_AudioSource, float volume , float Time)
    {
        if (listclip.Count > 0 && !isPlayingDict[r_AudioSource])
        {
            // Generate a random index to select a random clip from the list
            int randomIndex = Random.Range(0, listclip.Count);

            // Get the randomly selected AudioClip
            AudioClip randomClip = listclip[randomIndex];

            // Set the AudioSource's clip to the randomly selected clip
            r_AudioSource.clip = randomClip;

            // Set the volume
            r_AudioSource.volume = volume;

            // Play the audio
            r_AudioSource.PlayOneShot(randomClip);

            isPlayingDict[r_AudioSource] = true;

            // Start a coroutine to reset the flag when the audio finishes
            StartCoroutine(ResetIsPlayingFlag(randomClip.length- Time, r_AudioSource));
        }
    }
    public void StopSound(AudioSource r_AudioSource)
    {
        r_AudioSource.Stop();
        isPlayingDict[r_AudioSource] = false;
    }

    private IEnumerator ResetIsPlayingFlag(float duration, AudioSource audioSource)
    {
        yield return new WaitForSeconds(duration);
        // Reset the flag after the delay
        isPlayingDict[audioSource] = false;
    }

    public void PlayReelingSound()
    {
       PlaySound(RodReelingClips, r_AudioSource_Reeling, 0.7f, 1);
    }

    // Call the PlaySound method with r_AudioSource_Drag.
    public void PlayDragSound()
    {
        PlaySound(DragFishClips, r_AudioSource_Drag, 0.7f, 0f);
    }

    public void PlayCastSound()
    {
        PlaySound(RodCastClips, r_AudioSource_Cast, 0.5f, 0f);
    }


}
