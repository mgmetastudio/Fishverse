using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuToggleCP : MonoBehaviour
{
    public bool isPauseMenuStopped;
    public bool isPauseMenuActive;
    public bool isOpenPauseMenuENDED;
    public Transform Player;
    public GameObject PauseMenuUI;
    public Transform Main_Camera;
    public GameObject Info;
    public GameObject Close_Info;
    public GameObject Level_Panel_Window;
    [Header("Fishing System")]
    public Fishing_System Fishing_Start_System;
    [Header("Interaction")]
    public bool isInteracting = false;
    [Header("Cursor")]
    public bool isCursorEnabled = false;

    void Start()
    {
        if (Main_Camera == null)
            Main_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPauseMenuActive == false & isOpenPauseMenuENDED == true)
            {
                isOpenPauseMenuENDED = false;
                isPauseMenuActive = false;
                Pause();
            }
            else if (isPauseMenuActive == true & isOpenPauseMenuENDED == true)
            {
                isOpenPauseMenuENDED = false;
                isPauseMenuActive = false;
                CancelPause();
            }
        }

        if(isPauseMenuStopped == true)
        {
            PauseMenuUI.SetActive(false);
            isPauseMenuActive = false;
            isOpenPauseMenuENDED = true;
        }

        //Toggle Cursor
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isCursorEnabled == false)
            {
                Cursor.lockState = CursorLockMode.None; //unlock cursor
                Cursor.visible = true; //make mouse visible
                isCursorEnabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; //lock cursor
                Cursor.visible = false; //disable visible mouse
                isCursorEnabled = false;
            }
        }
    }

    void Pause()
    {
        if(isPauseMenuActive == false)
        {
            isInteracting = true;
            Fishing_Start_System.Check_Catched_Fishes();
            PauseMenuUI.SetActive(true);
            Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = false;
            //Disable player control
            Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = false;
            if (Player == null)
                Player = GameObject.FindGameObjectWithTag("Player").transform;
            PauseMenuUI.GetComponent<Animator>().Play("Pause_Menu_Fade_in");
            Cursor.lockState = CursorLockMode.None; //unlock cursor
            Cursor.visible = true; //make mouse visible
            Player.GetComponent<Animator>().enabled = true;
            Info.SetActive(false);
            Level_Panel_Window.SetActive(true);
            Close_Info.SetActive(true);
            StartCoroutine(Wait_If_Open_Pause_Menu_Is_Done());
        }
    }
    IEnumerator Wait_If_Open_Pause_Menu_Is_Done()
    {
        yield return new WaitForSeconds(1);
        if(isPauseMenuStopped == true)
        {
            PauseMenuUI.SetActive(false);
            isPauseMenuActive = false;
            isOpenPauseMenuENDED = true;
        }
        else
        {
            isPauseMenuActive = true;
            isOpenPauseMenuENDED = true;
        }
    }

    public void CancelPause()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        Main_Camera.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>().enabled = true;
        //Enable player control
        Player.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().canControl = true;
        PauseMenuUI.GetComponent<Animator>().Play("Pause_Menu_Fade_out");
        Cursor.lockState = CursorLockMode.Locked; //lock cursor
        Cursor.visible = false; //disable visible mouse
        Info.SetActive(true);
        Level_Panel_Window.SetActive(false);
        Close_Info.SetActive(false);
        StartCoroutine(Disable_Pause_Menu());
    }
    IEnumerator Disable_Pause_Menu()
    {
        yield return new WaitForSeconds(1);
        PauseMenuUI.SetActive(false);
        isPauseMenuActive = false;
        isOpenPauseMenuENDED = true;
        isInteracting = false;
    }

    public void DisablePauseMenuUI()
    {
        if (Player.GetComponent<PauseMenuToggleCP>().isPauseMenuActive == true)
            PauseMenuUI.SetActive(false);
    }

    public void EnablePauseMenuUI()
    {
        if (Player.GetComponent<PauseMenuToggleCP>().isPauseMenuActive == true)
            PauseMenuUI.SetActive(true);
    }
}
