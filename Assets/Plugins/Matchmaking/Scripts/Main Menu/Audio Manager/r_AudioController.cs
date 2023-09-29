using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class r_AudioController : MonoBehaviour
{
    public static r_AudioController instance;

    /// <summary>
    /// This script is used for the button click sounds.
    /// </summary>

    #region Variables
    [Header("Audio Source Button")]
    public AudioSource m_AudioSource;

    [Header("Audio Clip Button")]
    public AudioClip m_ClickSound;

    [Header("Background")]
    [SerializeField] private AudioClip BackgroundClip;
    public float MaxBackgroundVolume = 0.7f;
    public AudioSource backgroundSource;

    #endregion

    #region Unity Calls
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // Add this line
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Destroy any duplicates of this object
            return;
        }

        instance = this;
    }
    #endregion

    #region Play Sound
    public void PlayClickSound()
    {
        if (m_AudioSource != null && m_ClickSound != null)
            m_AudioSource.PlayOneShot(m_ClickSound);
    }
    public void PlayClip(AudioClip AudioClip)
    {
        m_AudioSource.PlayOneShot(AudioClip);
    }
    #endregion
    public void PlayBackground()
    {
        if (BackgroundClip == null) return;
        if (backgroundSource == null) { backgroundSource = gameObject.AddComponent<AudioSource>(); }

        backgroundSource.clip = BackgroundClip;
        backgroundSource.volume = 0;
        backgroundSource.playOnAwake = false;
        backgroundSource.loop = true;
        StartCoroutine(FadeAudio(backgroundSource, true, MaxBackgroundVolume));
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopBackground()
    {
        if (backgroundSource == null) return;

        FadeAudio(backgroundSource, false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ForceStopAllFades()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator FadeAudio(AudioSource source, bool up, float volume = 1)
    {
        if (up)
        {
            source.Play();
            while (source.volume < volume)
            {
                source.volume += Time.deltaTime * 0.01f;
                yield return null;
            }
        }
        else
        {
            while (source.volume > 0)
            {
                source.volume -= Time.deltaTime * 0.5f;
                yield return null;
            }
        }
    }

    public float BackgroundVolume
    {
        set
        {
            if (backgroundSource != null) { backgroundSource.volume = value; }
        }
    }


    private static r_AudioController _instance;
    public static r_AudioController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<r_AudioController>();
            }
            return _instance;
        }
    }
}
