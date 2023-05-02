using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Attachments : MonoBehaviour
{
    [Header("Attachments")]
    [Header("Backpack")]
    public GameObject Backpack;
    public bool HasBackpack;
    public bool shouldBackpackBeVisible;

    public void Start()
    {
        Load_Backpack();
    }

    public void Unlock_Backpack()
    {
         PlayerPrefs.SetInt("Backpack_Unlocked", 1);

        Load_Backpack();
    }

    public void Load_Backpack()
    {
        if (PlayerPrefs.HasKey("Backpack_Unlocked"))
        {
            Backpack.SetActive(true);
            HasBackpack = true;
        }
        else
        {
            Backpack.SetActive(false);
            HasBackpack = false;
        }
    }
}
