using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
// using Mirror;
using UnityEngine;

public class Image_Rotate_To_camera : MonoBehaviour
{
    public Transform Cam;

    public void Start()
    {
        if (GetComponentInParent<PhotonView>().IsMine)
        {
            gameObject.SetInactive();
            return;
        }

        Cam = Camera.main.transform;
    }

    void Update()
    {
        if (Cam == null)
        {
            transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(-Cam.GetComponent<Camera>().transform.forward, Cam.GetComponent<Camera>().transform.up);
        }
    }
}
