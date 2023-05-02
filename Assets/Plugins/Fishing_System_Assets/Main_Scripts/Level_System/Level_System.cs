using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level_System : MonoBehaviour
{
    public bool isActiv;
    [Header("Level System")]
    public Text Current_Level_Text;
    public GameObject Level_Up_Effect_Prefab;
    public int Current_Level;
    public Image XP_Fill_Amount;
    public int Current_Xp;
    public GameObject Level_Plus_Logo;
    public GameObject Level_Minus_Logo;

    public void Start()
    {
        Load_Current_Level_Stats();
    }

    public void Update()
    {
        #region Level System
        if (isActiv == true)
        {
            //If xp amount have reached "1.0", then level up.
            if (XP_Fill_Amount.fillAmount == 1f)
            {
                XP_Fill_Amount.fillAmount = 0;
                PlayerPrefs.SetFloat("Current_Saved_Fill_Amount", XP_Fill_Amount.fillAmount);
                Level_Up();
            }
        }
        #endregion
    }

    //Level System

    public void Load_Current_Level_Stats()
    {
        if (isActiv == true)
        {
            Current_Level = PlayerPrefs.GetInt("Current_Saved_Level");
            XP_Fill_Amount.fillAmount = PlayerPrefs.GetFloat("Current_Saved_Fill_Amount");
            Current_Level_Text.text = "" + Current_Level;
        }
    }

    public void Level_Up()
    {
        Current_Level += 1;
        PlayerPrefs.SetInt("Current_Saved_Level", Current_Level);
        Level_Plus_Logo.SetActive(true);
        Level_Plus_Logo.GetComponent<Animator>().Rebind();
        Instantiate(Level_Up_Effect_Prefab);
        Load_Current_Level_Stats();
        StartCoroutine(Disable_Level_Plus());
    }

    public void Add_XP_Points(float XP_Amount)
    {
        XP_Fill_Amount.fillAmount += XP_Amount;
        PlayerPrefs.SetFloat("Current_Saved_Fill_Amount", XP_Fill_Amount.fillAmount);
        Level_Plus_Logo.SetActive(true);
        Level_Plus_Logo.GetComponent<Animator>().Rebind();
        Load_Current_Level_Stats();
        StartCoroutine(Disable_Level_Plus());
    }

    IEnumerator Disable_Level_Plus()
    {
        yield return new WaitForSeconds(4);
        Level_Plus_Logo.SetActive(false);
    }

    IEnumerator Disable_Level_Minus()
    {
        yield return new WaitForSeconds(4);
        Level_Minus_Logo.SetActive(false);
    }
}
