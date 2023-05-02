using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Backpack_System : MonoBehaviour
{
    public GameObject Backpack_Design;
    public GameObject[] Item_UI_Buttons;
    [Header("Open Backpack")]
    public Transform Player;
    public GameObject Player_Ui_Canvas;
    public GameObject Progress_Menu;
    public GameObject Coins_System;
    public Transform Main_Camera;
    public GameObject Backpack_Camera;
    public GameObject Backpack_Variants;
    [Header("Messages")]
    public GameObject Messages_Canvas;
    public GameObject Dont_Have_A_Backpack_Message;
    public GameObject Item_Taken_Message;
    [Header("UI")]
    public GameObject Start_Warning_Window;
    public Toggle Do_Not_Show_Again_Toogle;
    //Disable/Enable backpack button on pause menu
    public GameObject Backpack_Pause_Menu_Button;
    public GameObject Warning_Backpack_Pause_Menu_Button;
    public GameObject Backpack_Pause_Menu_Canvas;
    public GameObject Backpack_Pause_Menu_Info;
    [Header("Selling")]
    public bool isSellingItems;
    public GameObject[] Items_Sell_Buttons;
    public GameObject Back_Button;
    [Header("Toggle Backpack Visibility")]
    public bool shouldBackpackBeVisible = true;
    public Toggle Backpack_Visible_Toggler;
    public bool canToggle = false;
    public GameObject Backpack_Visibility_Disabled_Info;
    [Header("Backpack Mesh")]
    public MeshRenderer[] Backpack_Meshes;
    [Header("Baits")]
    public bool isUsingFloater = true;//If = false, player is using spinner.
    public bool NoBite = true;
    public bool hasPredatoryFishBite = false;
    [Header("Floater")]
    public Text Floater_Selection_Text;
    public Button Floater_Selection_Button;
    public GameObject Floater_Checkbox;
    public string Floater_Save_Code = "Floater_With_Hook_Saved";
    public string Floater_Using_Save_Code = "Floater_Used";
    public GameObject Floater;
    [Header("Spinner")]
    public Text Spinner_Selection_Text;
    public Button Spinner_Selection_Button;
    public GameObject Spinner_Checkbox;
    public string Spinner_Save_Code = "Spinner_Saved";
    public string Spinner_Using_Save_Code = "Spinner_Used";
    public GameObject Spinner;
    [Header("Floater Additional Baits")]
    public bool isUsingAdditionalBait = false;
    [Header("Cheese")]
    public Text Cheese_Selection_Text;
    public Button Cheese_Selection_Button;
    public GameObject Cheese_Checkbox;
    public string Cheese_Save_Code = "Cheese_Saved";
    public string Cheese_Using_Save_Code = "Cheese_Used";
    public string Cheese_Amount_Save_Code = "Cheese_Amount_Saved";
    public GameObject Cheese;
    public bool isUsingCheese = false;
    [Header("Worm")]
    public Text Worm_Selection_Text;
    public Button Worm_Selection_Button;
    public GameObject Worm_Checkbox;
    public string Worm_Save_Code = "Worm_Saved";
    public string Worm_Using_Save_Code = "Worm_Used";
    public string Worm_Amount_Save_Code = "Worm_Amount_Saved";
    public GameObject Worm;
    public bool isUsingWorm = false;

    public void Open_Backpack_Menu()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.None; //unlock cursor
        Cursor.visible = true; //make mouse visible
        Player.GetComponent<Animator>().enabled = true;
        Player.GetComponent<PauseMenuToggleCP>().DisablePauseMenuUI();
        Player.GetComponent<PauseMenuToggleCP>().enabled = false;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
        //Disable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;
        Backpack_Variants.SetActive(true);
        Player_Ui_Canvas.SetActive(false);
        Backpack_Camera.SetActive(true);
        Backpack_Design.SetActive(true);
        Check_All();
        if (PlayerPrefs.HasKey("Backpack_Viewer_Start_Message_Saved"))
        {
            //The player do not wan't to see the start info again, so don't open it.
        }
        else
        {
            Open_Start_Warning_Window();
        }

        if(isSellingItems == true)
        {
            foreach (GameObject Sell_Buttons in Items_Sell_Buttons)
            {
                Sell_Buttons.SetActive(true);
            }
            Back_Button.SetActive(false);
        }
        else
        {
            foreach (GameObject Sell_Buttons in Items_Sell_Buttons)
            {
                Sell_Buttons.SetActive(false);
            }
            Back_Button.SetActive(true);
        }

        //Let every item check if it is in the backpack
        foreach (GameObject items in Item_UI_Buttons)
        {
            items.GetComponent<Item_System>().Check_Item();
        }

        foreach (MeshRenderer Backpack_Mesh in Backpack_Meshes)
        {
            Backpack_Mesh.enabled = true;
        }

        Check_Baits();
    }

    public void Exit_Backpack_Menu()
    {
        if (isSellingItems == true)
        {
            isSellingItems = false;

            foreach (GameObject Sell_Buttons in Items_Sell_Buttons)
            {
                Sell_Buttons.SetActive(false);
            }

            Back_Button.SetActive(true);
        }

        Backpack_Camera.SetActive(false);
        /*if (Player.GetComponent<Character_Attachments>().shouldBackpackBeVisible == true)
        {
            Backpack_Variants.SetActive(true);
        }
        else
        {
            Backpack_Variants.SetActive(false);
        }*/
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse

        Player.GetComponent<Animator>().enabled = true;
        Player.GetComponent<PauseMenuToggleCP>().enabled = true;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Enable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        Player_Ui_Canvas.SetActive(true);
        Backpack_Design.SetActive(false);
        //Toggle_Backpack_Visibility();
        Check_Baits();
    }

    public void Exit_Backpack_Menu_From_Shop()
    {
        if (isSellingItems == true)
        {
            isSellingItems = false;

            foreach (GameObject Sell_Buttons in Items_Sell_Buttons)
            {
                Sell_Buttons.SetActive(false);
            }

            Back_Button.SetActive(true);
        }

        Backpack_Camera.SetActive(false);
        /*if (Player.GetComponent<Character_Attachments>().shouldBackpackBeVisible == true)
        {
            Backpack_Variants.SetActive(true);
        }
        else
        {
            Backpack_Variants.SetActive(false);
        }*/
        Backpack_Design.SetActive(false);
        //Toggle_Backpack_Visibility();
        Check_Baits();
    }

    public void Start()
    {
        if(Player.GetComponent<Character_Attachments>().HasBackpack == true)
        {
            Backpack_Pause_Menu_Button.SetActive(true);
            Warning_Backpack_Pause_Menu_Button.SetActive(false);
        }
        else
        {
            Backpack_Pause_Menu_Button.SetActive(false);
            Warning_Backpack_Pause_Menu_Button.SetActive(true);
        }

        Backpack_Design.SetActive(true);
        //Let every item check if it is in the backpack
        foreach (GameObject items in Item_UI_Buttons)
        {
            items.GetComponent<Item_System>().Check_Item();
        }
        Backpack_Design.SetActive(false);
        //StartCoroutine(Disable_Design_After_Seconds());

        /*if (PlayerPrefs.GetInt("Backpack_Visibility") == 1)
        {
            shouldBackpackBeVisible = false;
            Backpack_Visible_Toggler.isOn = false;
        }

        if (PlayerPrefs.GetInt("Backpack_Visibility") == 2)
        {
            shouldBackpackBeVisible = true;
            Backpack_Visible_Toggler.isOn = true;
        }

        StartCoroutine(Check_Backpack_Visibility());*/

        //Checks which bait the player is unsing.
        Check_Baits();
    }

    /*IEnumerator Check_Backpack_Visibility()
    {
        yield return new WaitForSeconds(1);

        Toggle_Backpack_Visibility();
    }*/

    /*IEnumerator Disable_Design_After_Seconds()
    {
        yield return new WaitForSeconds(0.1f);
        Backpack_Design.SetActive(false);
    }*/

    public void Check_All()
    {
        //Let every item check if it is in the backpack
        foreach (GameObject items in Item_UI_Buttons)
        {
            items.GetComponent<Item_System>().Check_Item();
        }

        Check_Baits();
    }

    public void Disable_All_Messages()
    {
        Dont_Have_A_Backpack_Message.SetActive(false);
        Item_Taken_Message.SetActive(false);
    }

    public void Cant_Take_Item_Message()
    {
        Messages_Canvas.SetActive(true);
        Disable_All_Messages();
        Dont_Have_A_Backpack_Message.SetActive(true);
        Messages_Canvas.GetComponent<Animator>().Play("World_Coin_UI_Right_Fade_In");
        StartCoroutine(Deactivate_Message_Canvas());
    }

    public void Take_Item_Message()
    {
        Messages_Canvas.SetActive(true);
        Disable_All_Messages();
        Item_Taken_Message.SetActive(true);
        Messages_Canvas.GetComponent<Animator>().Play("World_Coin_UI_Right_Fade_In");
        StartCoroutine(Deactivate_Message_Canvas());
    }

    private IEnumerator Deactivate_Message_Canvas()
    {
        yield return new WaitForSeconds(7f);
        Disable_All_Messages();
        Messages_Canvas.SetActive(false);
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
            PlayerPrefs.SetInt("Backpack_Viewer_Start_Message_Saved", 1);
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

    //Info backpack pause menu
    public void Open_Backpack_Pause_Menu_Warning_Window()
    {
        Backpack_Pause_Menu_Canvas.SetActive(true);
        Backpack_Pause_Menu_Info.SetActive(true);
        Backpack_Pause_Menu_Info.GetComponent<Animator>().Play("Spawn_Frien_Window_In");
    }
    public void Close_Backpack_Pause_Menu_Warning_Window()
    {
        Backpack_Pause_Menu_Info.GetComponent<Animator>().Play("Spawn_Frien_Window_Out");
        StartCoroutine(Disable_Backpack_Pause_Menu_Warning_Menu_After_Seconds());
    }
    IEnumerator Disable_Backpack_Pause_Menu_Warning_Menu_After_Seconds()
    {
        yield return new WaitForSeconds(1);
        Backpack_Pause_Menu_Canvas.SetActive(false);
        Backpack_Pause_Menu_Info.SetActive(false);
        Backpack_Pause_Menu_Info.GetComponent<Animator>().Rebind();
    }
    /*//2 = true, 1 = false
    public void Toggle_Backpack_Visibility_Boolean()
    {
        if(canToggle == true)
        {
            if (shouldBackpackBeVisible == true)
            {
                PlayerPrefs.SetInt("Backpack_Visibility", 1);
                shouldBackpackBeVisible = false;
                Backpack_Visible_Toggler.isOn = false;
            }
            else
            {
                PlayerPrefs.SetInt("Backpack_Visibility", 2);
                shouldBackpackBeVisible = true;
                Backpack_Visible_Toggler.isOn = true;
            }
        }
    }

    public void Toggle_Backpack_Visibility()
    {
        if(Player.GetComponent<Character_Attachments>().HasBackpack == true)
        {
            if (shouldBackpackBeVisible == true)
            {
                foreach (MeshRenderer Backpack_Mesh in Backpack_Meshes)
                {
                    Backpack_Mesh.enabled = true;
                }
            }
            else
            {
                foreach (MeshRenderer Backpack_Mesh in Backpack_Meshes)
                {
                    Backpack_Mesh.enabled = false;

                    Instantiate(Backpack_Visibility_Disabled_Info);
                }
            }
        }

        canToggle = true;
    }*/

    //Select Floater or Spinner
    public void Select_Floater()
    {
        PlayerPrefs.DeleteKey(Spinner_Using_Save_Code);
        PlayerPrefs.SetInt(Floater_Using_Save_Code, 1);
        Floater_Selection_Button.interactable = false;
        Spinner_Selection_Button.interactable = true;
        Floater_Selection_Text.text = "Selected";
        Spinner_Selection_Text.text = "Select";
        Floater_Checkbox.SetActive(true);
        Spinner_Checkbox.SetActive(false);
        Floater.SetActive(true);
        Spinner.SetActive(false);
        NoBite = false;
        hasPredatoryFishBite = false;
        isUsingFloater = true;
        //Check if Floater Additional Baits are used
        //Cheese
        if (PlayerPrefs.HasKey(Cheese_Save_Code) & PlayerPrefs.HasKey(Cheese_Using_Save_Code))
        {
            Select_Cheese();
        }
        if (PlayerPrefs.HasKey(Cheese_Save_Code) & !PlayerPrefs.HasKey(Worm_Using_Save_Code))
            Cheese_Selection_Button.interactable = true;
        //Worm
        if (PlayerPrefs.HasKey(Worm_Save_Code) & PlayerPrefs.HasKey(Worm_Using_Save_Code))
        {
            Select_Worm();
        }
        if (PlayerPrefs.HasKey(Worm_Save_Code) & !PlayerPrefs.HasKey(Cheese_Using_Save_Code))
            Worm_Selection_Button.interactable = true;
    }

    public void Select_Spinner()
    {
        PlayerPrefs.DeleteKey(Floater_Using_Save_Code);
        PlayerPrefs.SetInt(Spinner_Using_Save_Code, 1);
        Spinner_Selection_Button.interactable = false;
        Floater_Selection_Button.interactable = true;
        Spinner_Selection_Text.text = "Selected";
        Floater_Selection_Text.text = "Select";
        Spinner_Checkbox.SetActive(true);
        Floater_Checkbox.SetActive(false);
        Spinner.SetActive(true);
        Floater.SetActive(false);
        NoBite = false;
        hasPredatoryFishBite = true;
        isUsingFloater = false;
        Worm_Selection_Button.interactable = false;
        Cheese_Selection_Button.interactable = false;
        Deselect_Cheese();
        Deselect_Worm();
    }

    //Floater Additional Baits//
    //Cheese
    public void Select_Cheese()
    {
        PlayerPrefs.DeleteKey(Worm_Using_Save_Code);
        PlayerPrefs.SetInt(Cheese_Using_Save_Code, 1);
        Cheese_Selection_Button.interactable = false;
        Worm_Selection_Button.interactable = true;
        Cheese_Selection_Text.text = "Selected";
        Worm_Selection_Text.text = "Select";
        Cheese_Checkbox.SetActive(true);
        Worm_Checkbox.SetActive(false);
        Cheese.SetActive(true);
        Worm.SetActive(false);
        isUsingCheese = true;
        isUsingAdditionalBait = true;
    }
    public void Deselect_Cheese()
    {
        PlayerPrefs.DeleteKey(Cheese_Using_Save_Code);
        Cheese_Selection_Button.interactable = false;
        Cheese_Selection_Text.text = "Select";
        Cheese_Checkbox.SetActive(false);
        Cheese.SetActive(false);
        isUsingCheese = false;
        isUsingAdditionalBait = false;
    }

    //Worm
    public void Select_Worm()
    {
        PlayerPrefs.DeleteKey(Cheese_Using_Save_Code);
        PlayerPrefs.SetInt(Worm_Using_Save_Code, 1);
        Worm_Selection_Button.interactable = false;
        Cheese_Selection_Button.interactable = true;
        Worm_Selection_Text.text = "Selected";
        Cheese_Selection_Text.text = "Select";
        Worm_Checkbox.SetActive(true);
        Cheese_Checkbox.SetActive(false);
        Worm.SetActive(true);
        Cheese.SetActive(false);
        isUsingWorm = true;
        isUsingAdditionalBait = true;
    }
    public void Deselect_Worm()
    {
        PlayerPrefs.DeleteKey(Worm_Using_Save_Code);
        Worm_Selection_Button.interactable = false;
        Worm_Selection_Text.text = "Select";
        Worm_Checkbox.SetActive(false);
        Worm.SetActive(false);
        isUsingWorm = false;
        isUsingAdditionalBait = false;
    }

    //Checks which bait the player is using.
    public void Check_Baits()
    {
        if(PlayerPrefs.HasKey(Floater_Save_Code) & PlayerPrefs.HasKey(Floater_Using_Save_Code))
        {
            Select_Floater();
            //Check if Floater Additional Baits are used
            //Cheese
            if(PlayerPrefs.HasKey(Cheese_Save_Code) & PlayerPrefs.HasKey(Cheese_Using_Save_Code))
            {
                Select_Cheese();
            }
            //Worm
            if (PlayerPrefs.HasKey(Worm_Save_Code) & PlayerPrefs.HasKey(Worm_Using_Save_Code))
            {
                Select_Worm();
            }
        }

        if(!PlayerPrefs.HasKey(Floater_Using_Save_Code))
        {
            Worm_Selection_Button.interactable = false;
            Cheese_Selection_Button.interactable = false;
        }

        if (PlayerPrefs.HasKey(Spinner_Save_Code) & PlayerPrefs.HasKey(Spinner_Using_Save_Code))
        {
            Select_Spinner();
        }
        //Player dosn't have a bait.
        if (!PlayerPrefs.HasKey(Floater_Save_Code) & !PlayerPrefs.HasKey(Spinner_Save_Code))
        {
            NoBite = true;
        }
    }
}
