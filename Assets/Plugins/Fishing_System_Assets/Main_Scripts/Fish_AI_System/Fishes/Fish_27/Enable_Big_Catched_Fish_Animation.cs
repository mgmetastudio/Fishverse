using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable_Big_Catched_Fish_Animation : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Catched_Fish_Object;

    public void Enbale_Big_Catched_Fish_Anim()
    {
        Camera.GetComponent<Animator>().Rebind();
        Catched_Fish_Object.SetActive(true);
    }

    public void Disable_Big_Catched_Fish_Anim()
    {
        Catched_Fish_Object.SetActive(false);
    }
}
