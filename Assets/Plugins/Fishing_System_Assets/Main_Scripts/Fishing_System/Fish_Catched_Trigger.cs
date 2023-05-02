using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Catched_Trigger : MonoBehaviour
{
    public Fishing_Action_System Fishing_System;
    public Transform Floater;
    public Transform Player;
    public float minFishDistance = 3;

    void Update()
    {
        float Current_Fish_Distance = Vector3.Distance(Floater.transform.position, Player.transform.position);

        if (Current_Fish_Distance < minFishDistance)
        {
            Fishing_System.isFishInTrigger = true;
        }
    }

    /*void OnTriggerEnter(Collider Fish)
    {
        if(Fish.tag == "Fish")
        {
            if(Fish.GetComponent<AIFishControl>().Bite == true)
                Fishing_System.isFishInTrigger = true;
        }
    }

    void OnTriggerExit(Collider Fish)
    {
        Fishing_System.isFishInTrigger = false;
    }*/
}
