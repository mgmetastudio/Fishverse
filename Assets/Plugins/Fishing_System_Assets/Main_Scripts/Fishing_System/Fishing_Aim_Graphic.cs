using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing_Aim_Graphic : MonoBehaviour
{
    public Fishing_Action_System Fishing_System;

    void OnTriggerStay(Collider Floater)
    {
        if(Floater.tag == "Fishing_Float")
        {
            Fishing_System.isMouseOnAimGraphic = true;
        }
    }

    void OnTriggerExit(Collider Floater)
    {
        Fishing_System.isMouseOnAimGraphic = false;
    }
}
