using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Shop_System : MonoBehaviour
{
    [Header("Player Objects")]
    public Transform Player;
    public GameObject Backpack_System;
    public GameObject Player_Menu;
    public Transform Main_Camera;
    [Header("UI")]
    public GameObject Need_A_Backpack_Warning;
    public GameObject Sell_Items_Button;
    public GameObject Buy_Items_Button;
    public GameObject Need_A_Backpack_Warning_Buy;
    public GameObject Buy_Items_Canvas_Prefab;
    public GameObject Buy_Items_Canvas_Spawned_Prefab;
    public GameObject Cancel_Selling_Items_Canvas;
    public GameObject Cancel_Buy_Items_Canvas;
    [Header("Enter Shop")]
    public GameObject In_Near_Graphic;
    public BoxCollider Trigger;
    [Header("Menu")]
    public GameObject Menu;
    [Header("Shop Camera")]
    public GameObject Shop_Camera;

    void OnTriggerStay(Collider Player)
    {
        if(Player.tag == "Player")
        {
            In_Near_Graphic.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E) & Player.GetComponent<PauseMenuToggleCP>().isInteracting == false)
            {
                Player.GetComponent<PauseMenuToggleCP>().isInteracting = true;
                Player_Menu.SetActive(false);
                Player.GetComponent<PauseMenuToggleCP>().enabled = false;
                Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
                //Disable player control
                Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;

                Check_If_Has_Items();
                Trigger.enabled = false;
                In_Near_Graphic.SetActive(false);
                Menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None; //unlock cursor
                Cursor.visible = true; //make mouse visible
                Shop_Camera.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider Player)
    {
        In_Near_Graphic.SetActive(false);
    }

    public void Leave_Menu()
    {
        Player_Menu.SetActive(true);
        Player.GetComponent<PauseMenuToggleCP>().enabled = true;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Disable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        Player.GetComponent<PauseMenuToggleCP>().CancelPause();

        Check_If_Has_Items();
        Menu.SetActive(false);
        Trigger.enabled = true;
        //In_Near_Graphic.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse
        Shop_Camera.SetActive(false);
        Player.GetComponent<PauseMenuToggleCP>().isInteracting = false;
    }

    public void Start()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    IEnumerator Check_If_Player_Has_Backpack()
    {
        yield return new WaitForSeconds(1);

        Check_If_Has_Items();
    }

    public void Check_If_Has_Items()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;

        if (Backpack_System == null)
            Backpack_System = GameObject.FindGameObjectWithTag("Backpack_System");

        if (Player.GetComponent<Character_Attachments>().HasBackpack == true)
        {
            Sell_Items_Button.GetComponent<Button>().interactable = true;
            Need_A_Backpack_Warning.SetActive(false);
            Buy_Items_Button.GetComponent<Button>().interactable = true;
            Need_A_Backpack_Warning_Buy.SetActive(false);
        }
        else
        {
            Sell_Items_Button.GetComponent<Button>().interactable = false;
            Need_A_Backpack_Warning.SetActive(true);
            Buy_Items_Button.GetComponent<Button>().interactable = false;
            Need_A_Backpack_Warning_Buy.SetActive(true);
        }
    }

    public void Enter_Backpack_System()
    {
        //Turn the sell mode from the backpack_system on to enable the sell buttons
        Shop_Camera.SetActive(false);
        Menu.SetActive(false);
        Cancel_Selling_Items_Canvas.SetActive(true);
        Backpack_System.GetComponent<Backpack_System>().isSellingItems = true;
        Backpack_System.GetComponent<Backpack_System>().Open_Backpack_Menu();
    }

    public void Close_Backpack_System()
    {
        //Turn the sell mode from the backpack_system on, to enable the sell buttons
        Shop_Camera.SetActive(true);
        Cancel_Selling_Items_Canvas.SetActive(false);
        Backpack_System.GetComponent<Backpack_System>().isSellingItems = false;
        Backpack_System.GetComponent<Backpack_System>().Exit_Backpack_Menu_From_Shop();
        Player_Menu.SetActive(false);
        Check_If_Has_Items();
        Menu.SetActive(true);
    }

    public void Open_Buy_Items()
    {
        Menu.SetActive(false);
        Instantiate(Buy_Items_Canvas_Prefab);
        Buy_Items_Canvas_Spawned_Prefab = GameObject.FindGameObjectWithTag("Buy_Items_Canvas");
        Cancel_Buy_Items_Canvas.SetActive(true);
    }

    public void Close_Buy_Items()
    {
        Cancel_Buy_Items_Canvas.SetActive(false);
        Destroy(Buy_Items_Canvas_Spawned_Prefab);
        Buy_Items_Canvas_Spawned_Prefab = null;
        Check_If_Has_Items();
        Menu.SetActive(true);
    }
}
