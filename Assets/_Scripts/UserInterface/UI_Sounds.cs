using UnityEngine;

public class UI_Sounds : MonoBehaviour
{
    public AudioSource[] all_sounds;

    public void PlaySound(int sound_index)
    {
        if (all_sounds[sound_index])
        {
            all_sounds[sound_index].pitch = Random.Range(0.95f, 1.04f);
            all_sounds[sound_index].Play();
        }
    }
}
