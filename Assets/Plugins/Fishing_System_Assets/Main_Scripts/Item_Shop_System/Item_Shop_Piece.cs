using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Shop_Piece : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject Buy_Item_Warning_Window;
    [Header("Players Currency")]
    public GameObject Currency_Script;
    public int Item_Price;
    public GameObject Item_Prefab;
    public GameObject Item_Prefab_Spawned;
    [Header("Not Enought Money")]
    public GameObject Not_Enought_Money_Warning;

    public void Start()
    {
        if (Currency_Script == null)
        {
            Currency_Script = GameObject.FindGameObjectWithTag("Currency_System");
        }
    }

    //Buy Item//
    public void Open_Item_Buy_Warning_Window()
    {
        if (Item_Price <= PlayerPrefs.GetInt("Coins"))
        {//Have enought money
            Buy_Item_Warning_Window.SetActive(true);
            Buy_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
        }
        else
        {//Not enought money
            Not_Enought_Money_Warning.SetActive(true);
            StartCoroutine(TimeToDisableWarning());
        }
    }

    IEnumerator TimeToDisableWarning()
    {
        yield return new WaitForSeconds(3);

        Not_Enought_Money_Warning.SetActive(false);
    }

    public void Close_Buy_Item_Warning_Window()
    {
        Buy_Item_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Buy_Call_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Buy_Call_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(0);
        Buy_Item_Warning_Window.SetActive(false);
        Buy_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }
    IEnumerator Disable_Buy_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Buy_Item_Warning_Window.SetActive(false);
        Buy_Item_Warning_Window.GetComponent<Animator>().Rebind();
    }

    public void Buy_Item()
    {
        //Subtract the money
        Currency_Script.GetComponent<Currency_System>().Remove_Coins(Item_Price);
        Instantiate(Item_Prefab);
        StartCoroutine(Disable_Buy_Call_Warning_Menu_After_Seconds());
    }
}
