using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_System : MonoBehaviour
{
    [Header("Item Global Settings")]
    public string Item_Save_Code;
    public string Item_Amount_Save_Code;
    public bool isItemInBackpack;
    public int Item_Amount;
    public int Item_Sell_Value;
    [Header("UI Settings")]
    public GameObject Remove_Item_Warning_Window;
    public GameObject Sell_Item_Warning_Window;
    public Text Item_Amount_Text;
    [Header("Players Currency")]
    public Currency_System Currency_Script;

    //Check if item is in backpack and how mush is the amount
    public void Check_Item()
    {
        if(PlayerPrefs.HasKey(Item_Save_Code))
        {
            //Item is in backpack
            isItemInBackpack = true;
            //The item UI item schould be active
            this.gameObject.SetActive(true);
            //Check the amount of the item and display them on the amout text
            Item_Amount = PlayerPrefs.GetInt(Item_Amount_Save_Code);
            Item_Amount_Text.text = "" + Item_Amount + "x";
        }
        else
        {
            //Item is not in backpack
            isItemInBackpack = false;
            //The item UI item schould be not active
            this.gameObject.SetActive(false);
        }
    }

    //Remove Item//
    public void Open_Item_Remove_Warning_Window()
    {
        Remove_Item_Warning_Window.SetActive(true);
        Remove_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Remove_Item_Warning_Window()
    {
        Remove_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Call_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Call_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Remove_Item_Warning_Window.SetActive(false);
        Remove_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }
    IEnumerator Disable_Remove_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Remove_Item_Warning_Window.SetActive(false);
        Remove_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }

    public void Remove_Item()
    {
        //Create and play selected sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("FABRIC_Movement_Short_RR4_mono") as AudioClip;
        audioSource.Play();
        //Subtract 1 item count
        Item_Amount -= 1;
        PlayerPrefs.SetInt(Item_Amount_Save_Code, Item_Amount);
        //Check the amount of the item and display them on the amount text
        Item_Amount_Text.text = "" + PlayerPrefs.GetInt(Item_Amount_Save_Code) + "x";
        Remove_Item_Warning_Window.SetActive(false);
        Remove_Item_Warning_Window.GetComponent<Animator>().Rebind();
        if (Item_Amount < 1)
        {
            //if the item amount is lower then 1, delete it
            PlayerPrefs.DeleteKey(Item_Save_Code);
            Item_Amount = 0;
            PlayerPrefs.SetInt(Item_Amount_Save_Code, 0);
            //Item is not in backpack
            isItemInBackpack = false;
            //The item UI item schould be disabled
            Remove_Item_Warning_Window.SetActive(false);
            Remove_Item_Warning_Window.GetComponent<Animator>().Rebind();
            this.gameObject.SetActive(false);
        }
    }

    //Sell Item//
    public void Open_Item_Sell_Warning_Window()
    {
        Sell_Item_Warning_Window.SetActive(true);
        Sell_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Sell_Item_Warning_Window()
    {
        Sell_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Sell_Call_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Sell_Call_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Sell_Item_Warning_Window.SetActive(false);
        Sell_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }
    IEnumerator Disable_Sell_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Sell_Item_Warning_Window.SetActive(false);
        Sell_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }

    public void Sell_Item()
    {
        /*//Create and play selected sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("FABRIC_Movement_Short_RR4_mono") as AudioClip;
        audioSource.Play();*/
        //Subtract 1 item count
        Item_Amount -= 1;
        PlayerPrefs.SetInt(Item_Amount_Save_Code, Item_Amount);
        //Check the amount of the item and display them on the amount text
        Item_Amount_Text.text = "" + PlayerPrefs.GetInt(Item_Amount_Save_Code) + "x";
        Sell_Item_Warning_Window.SetActive(false);
        Sell_Item_Warning_Window.GetComponent<Animator>().Rebind();
        //Get the money
        Currency_Script.Add_Coins(Item_Sell_Value);
        if (Item_Amount < 1)
        {
            //if the item amount is lower then 1, delete it
            PlayerPrefs.DeleteKey(Item_Save_Code);
            Item_Amount = 0;
            PlayerPrefs.SetInt(Item_Amount_Save_Code, 0);
            //Item is not in backpack
            isItemInBackpack = false;
            //The item UI item schould be disabled
            Sell_Item_Warning_Window.SetActive(false);
            Sell_Item_Warning_Window.GetComponent<Animator>().Rebind();
            this.gameObject.SetActive(false);
        }
    }
}
