using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing_System : MonoBehaviour
{
    public GameObject Fishing_Rod;
    public bool HasFishingRod;
    [Header("Open Fishing System")]
    public Transform Player;
    public GameObject Player_Ui_Canvas;
    public GameObject Progress_Menu;
    public GameObject Coins_System;
    public Transform Main_Camera;
    public GameObject Fishing_Camera;
    [Header("UI Settings")]
    public GameObject Fishing_Design;
    public GameObject Start_Warning_Window;
    public Toggle Do_Not_Show_Again_Toogle;
    [Header("Messages")]
    public GameObject Messages_Canvas;
    public GameObject Entered_Fishing_Message;
    public GameObject Cant_Fishing_Here_Message;
    //Disable/Enable fishing button on pause menu
    public GameObject Fishing_Pause_Menu_Button;
    public GameObject Warning_Fishing_Pause_Menu_Button;
    public GameObject Fishing_Pause_Menu_Canvas;
    public GameObject Fishing_Pause_Menu_Info;
    [Header("Fishing Action System")]
    public GameObject Fishing_Action_System;
    [Header("Catched Fishes")]
    public int Catched_Fishes;
    public Text Catched_Fishes_Text;
    public string Biggest_Fish_Size;
    public Text Biggest_Fish_Text;
    public string Biggest_Fish_Name_Text;
    public Text Biggest_Fish_Name;

    public void Start()
    {
        Check_If_Have_Fishing_Rod();

        Check_Catched_Fishes();

        Set_Up_Stats();
    }

    public void Set_Up_Stats()
    {
        if(PlayerPrefs.HasKey("Stats_have_been_set"))
        {

        }
        else
        {//Set start fish size.
            PlayerPrefs.SetString("Best_Fish_Size", "00 cm");
            //Add money.
            PlayerPrefs.SetInt("Coins", 45);
            PlayerPrefs.SetInt("Stats_have_been_set", 1);
        }
    }

    public void Check_If_Have_Fishing_Rod()
    {
        if(PlayerPrefs.HasKey("Fishing_Rod_01_Saved") & Player.GetComponent<Character_Attachments>().HasBackpack == true)
        {
            HasFishingRod = true;
            Fishing_Pause_Menu_Button.SetActive(true);
            Warning_Fishing_Pause_Menu_Button.SetActive(false);
        }
        else
        {
            HasFishingRod = false;
            Warning_Fishing_Pause_Menu_Button.SetActive(true);
            Fishing_Pause_Menu_Button.SetActive(false);
        }
    }

    public void Open_Fishing_Menu()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if(!PlayerPrefs.HasKey("Fishing_System_Opened"))
        {
            PlayerPrefs.SetInt("Fishing_System_Opened", 1);
        }
        Player_Ui_Canvas.SetActive(false);
        Player.GetComponent<PauseMenuToggleCP>().enabled = false;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
        //Disable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;
        Fishing_Camera.SetActive(true);
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
        Fishing_Design.SetActive(true);
        if (PlayerPrefs.HasKey("Fishing_Viewer_Start_Message_Saved"))
        {
            //The player do not wan't to see the start info again, so don't open it.
        }
        else
        {
            Open_Start_Warning_Window();
        }
        //Enable fishing rod and play hold fishing rod animation
        Fishing_Rod.SetActive(true);
        Player.GetComponent<Animator>().Play("Walk_W_Fishing_Rod");
        Check_Catched_Fishes();
    }

    public void Exit_Fishing_Menu()
    {
        Fishing_Camera.SetActive(false);
        Player.GetComponent<PauseMenuToggleCP>().enabled = true;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Enable player control
        Player.GetComponent<Animator>().Rebind();
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        Player_Ui_Canvas.SetActive(true);
        Player.GetComponent<PauseMenuToggleCP>().CancelPause();
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
        Fishing_Design.SetActive(false);
        Fishing_Rod.SetActive(false);
    }

    //Info message on start open
    public void Open_Start_Warning_Window()
    {
        Start_Warning_Window.SetActive(true);
        Start_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Start_Warning_Window()
    {
        if (Do_Not_Show_Again_Toogle.isOn == true)
        {
            PlayerPrefs.SetInt("Fishing_Viewer_Start_Message_Saved", 1);
        }
        Start_Warning_Window.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Start_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Start_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Start_Warning_Window.SetActive(false);
        Start_Warning_Window.GetComponent<Animator>().Rebind();
    }

    //Messages
    public void Disable_All_Messages()
    {
        Cant_Fishing_Here_Message.SetActive(false);
        Entered_Fishing_Message.SetActive(false);
    }

    public void Can_not_Fishing_Here_Message()
    {
        Messages_Canvas.SetActive(true);
        Disable_All_Messages();
        Cant_Fishing_Here_Message.SetActive(true);
        Messages_Canvas.GetComponent<Animator>().Play("World_Coin_UI_Right_Fade_In");
        StartCoroutine(Deactivate_Message_Canvas());
    }

    public void Entered_Fishing_System_Message()
    {
        Messages_Canvas.SetActive(true);
        Disable_All_Messages();
        Entered_Fishing_Message.SetActive(true);
        Messages_Canvas.GetComponent<Animator>().Play("World_Coin_UI_Right_Fade_In");
        StartCoroutine(Deactivate_Message_Canvas());
    }

    private IEnumerator Deactivate_Message_Canvas()
    {
        yield return new WaitForSeconds(7f);
        Disable_All_Messages();
        Messages_Canvas.SetActive(false);
    }

    //Info backpack pause menu
    public void Open_Fishing_Pause_Menu_Warning_Window()
    {
        Fishing_Pause_Menu_Canvas.SetActive(true);
        Fishing_Pause_Menu_Info.SetActive(true);
        Fishing_Pause_Menu_Info.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Fishing_Pause_Menu_Warning_Window()
    {
        Fishing_Pause_Menu_Info.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Fishing_Pause_Menu_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Fishing_Pause_Menu_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Fishing_Pause_Menu_Canvas.SetActive(false);
        Fishing_Pause_Menu_Info.SetActive(false);
        Fishing_Pause_Menu_Info.GetComponent<Animator>().Rebind();
    }

    public void Start_Fishing()
    {
        Player.GetComponent<PauseMenuToggleCP>().enabled = true;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Enable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        Player.GetComponent<PauseMenuToggleCP>().CancelPause();
        Fishing_Design.SetActive(false);
        Fishing_Action_System.SetActive(true);
        Fishing_Action_System.GetComponent<Fishing_Action_System>().Start_Fishing();
    }

    public void Add_Catched_Fishes_Point()
    {
        Catched_Fishes += 1;
        PlayerPrefs.SetInt("Catched_Fishes_Saved", Catched_Fishes);
    }

    public void Check_Catched_Fishes()
    {
        Catched_Fishes = PlayerPrefs.GetInt("Catched_Fishes_Saved", Catched_Fishes);
        Catched_Fishes_Text.text = "" + Catched_Fishes;

        Biggest_Fish_Size = PlayerPrefs.GetString("Best_Fish_Size");
        Biggest_Fish_Text.text = Biggest_Fish_Size;

        Biggest_Fish_Name_Text = PlayerPrefs.GetString("Biggest_Fish_Name");
        Biggest_Fish_Name.text = Biggest_Fish_Name_Text;
    }
}
