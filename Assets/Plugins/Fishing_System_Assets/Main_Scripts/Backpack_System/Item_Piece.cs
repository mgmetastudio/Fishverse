using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Piece : MonoBehaviour
{
    public Transform Player;
    [Header("Item Global Settings")]
    public bool isUnique;
    public bool isSpawned;
    public GameObject Item_Piece_Object;
    public string Item_Save_Code;
    public string Item_Amount_Save_Code;
    public int Item_Amount;
    [Header("Item UI Circle Settings")]
    public Collider triggerCollider;
    public GameObject Enter_Icon;
    public GameObject Outline_Image;
    public bool pointerDown;
    private float pointerDownTimer;
    public float requiredHoldTime;
    [Header("Backpack")]
    public GameObject Backpack_System;
    public bool Backpack_System_Found;
    public bool isBackpack = false;

    [SerializeField]
    private Image fillImage;

    public void Start()
    {
        //Find player
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        //Destroy the current item, if the player has the current item already.
        if(isUnique == true & PlayerPrefs.HasKey(Item_Save_Code))
            Destroy(Item_Piece_Object);

        if (isBackpack == true)
        {
            if(PlayerPrefs.HasKey(Item_Save_Code))
                Destroy(Item_Piece_Object);
        }

        Item_Amount = PlayerPrefs.GetInt(Item_Amount_Save_Code);

        if(isSpawned == true)
        {
            Backpack_System_Found = true;
            if (Backpack_System == null)
                Backpack_System = GameObject.FindGameObjectWithTag("Backpack_System");

            Take_Item();

            Destroy(Item_Piece_Object);
        }
    }

    private void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

    void OnTriggerStay(Collider x)
    {
        if (x.tag == "Player")
        {
            if(Backpack_System_Found == false)
            {
                Backpack_System_Found = true;
                if (Backpack_System == null)
                    Backpack_System = GameObject.FindGameObjectWithTag("Backpack_System");
            }

            Outline_Image.SetActive(true);
            if (Input.GetButton("Action"))
            {
                pointerDown = true;

                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer > requiredHoldTime)
                {
                    if(isBackpack == false)
                    {
                        //Isn't a backpack, so take the item.
                        Take_Item();
                    }
                    else
                    {
                        //It's a backpack, unlock the backpack.
                        Player.GetComponent<Character_Attachments>().Unlock_Backpack();
                        Destroy(Item_Piece_Object);
                    }

                    Reset();
                }
                fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
            }
            else
            {
                Reset();
            }
        }
    }

    void OnTriggerExit(Collider x)
    {
        Outline_Image.SetActive(false);
    }

    public void Take_Item()
    {
        Item_Amount = PlayerPrefs.GetInt(Item_Amount_Save_Code);
        //Find player
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        //Check if player has a backpack
        if (Player.GetComponent<Character_Attachments>().HasBackpack == true)
        {
            //Create and play selected sound
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = Resources.Load("FABRIC_Movement_Short_RR4_mono") as AudioClip;
            audioSource.Play();
            //Take it
            Item_Amount += 1;
            PlayerPrefs.SetInt(Item_Amount_Save_Code, Item_Amount);
            PlayerPrefs.SetInt(Item_Save_Code, 1);
            Backpack_System.GetComponent<Backpack_System>().Check_All();
            Backpack_System.GetComponent<Backpack_System>().Take_Item_Message();
            Destroy(Item_Piece_Object);
        }
        else
        {
            Backpack_System.GetComponent<Backpack_System>().Cant_Take_Item_Message();
        }
    }
}
