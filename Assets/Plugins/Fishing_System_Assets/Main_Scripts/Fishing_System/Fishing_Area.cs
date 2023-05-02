using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_Area : MonoBehaviour
{
    public GameObject Fishing_Action_System;

    public void Find_Fishing_Action_System()
    {
        //Searching the Find_Fishing_Action_System...
        if (GameObject.FindGameObjectWithTag("Fishing_Action_System") != null)
        {
            //We have found the Find_Fishing_Action_System assign him to the Find_Fishing_Action_System variable
            if (Fishing_Action_System == null)
                Fishing_Action_System = GameObject.FindGameObjectWithTag("Fishing_Action_System");
        }
        else
        {
            //The Find_Fishing_Action_System is not there
        }
    }

    void OnTriggerStay(Collider Player)
    {
        if(Fishing_Action_System != null)
        {
            if (Player.tag == "Player" & Fishing_Action_System.GetComponent<Fishing_Action_System>().isInFishSwimArea == false)
            {
                Fishing_Action_System.GetComponent<Fishing_Action_System>().canFishing = true;
            }
        }
    }

    void OnTriggerExit(Collider Player)
    {
        if(Fishing_Action_System != null)
        {
            Fishing_Action_System.GetComponent<Fishing_Action_System>().canFishing = false;
        }
    }
}
