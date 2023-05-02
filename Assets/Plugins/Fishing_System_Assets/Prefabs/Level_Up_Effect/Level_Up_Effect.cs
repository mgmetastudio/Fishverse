using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Level_Up_Effect : MonoBehaviour
{
    public Text Current_Level_Text;

    public void Start()
    {
        Current_Level_Text.text = "" + PlayerPrefs.GetInt("Current_Saved_Level");
        //Create and play level up sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioSource.clip = Resources.Load("Special & Powerup (21)") as AudioClip;
        audioSource.Play();
    }
}
