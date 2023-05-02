using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Currency_System : MonoBehaviour
{
    public int Current_Coins;
    public Text current_Currency_Text;
    public GameObject currency_Plus_Logo;
    public GameObject currency_Minus_Logo;

    void Start()
    {
        //Loads the coins
        Current_Coins = PlayerPrefs.GetInt("Coins", Current_Coins);
        current_Currency_Text.text = "" + Current_Coins;
        Save_Coins();
    }

    public void Add_Coins(int addAmount)
    {
        //Create and play selected sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("Special & Powerup (38)") as AudioClip;
        audioSource.Play();
        currency_Plus_Logo.SetActive(true);
        currency_Plus_Logo.GetComponent<Animator>().Rebind();
        StartCoroutine(Disable_Currency_Plus());
        Current_Coins += addAmount;
        Save_Coins();
    }
    IEnumerator Disable_Currency_Plus()
    {
        yield return new WaitForSeconds(4);
        currency_Plus_Logo.SetActive(false);
    }

    public void Remove_Coins(int addAmount)
    {
        //Create and play selected sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioSource.clip = Resources.Load("Item Purchase (2)") as AudioClip;
        audioSource.Play();
        currency_Minus_Logo.SetActive(true);
        currency_Minus_Logo.GetComponent<Animator>().Rebind();
        StartCoroutine(Disable_Currency_Minus());
        Current_Coins -= addAmount;
        Save_Coins();
    }
    IEnumerator Disable_Currency_Minus()
    {
        yield return new WaitForSeconds(4);
        currency_Minus_Logo.SetActive(false);
    }

    public void Load_Coins()
    {
        //Loads the coins
        Current_Coins = PlayerPrefs.GetInt("Coins", Current_Coins);
        current_Currency_Text.text = "" + Current_Coins;
    }

    public void Save_Coins()
    {
        //Save the coins
        PlayerPrefs.SetInt("Coins", Current_Coins);
        PlayerPrefs.Save();
        Load_Coins();
    }
}
