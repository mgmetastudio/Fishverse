using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoatInputProxy : MonoBehaviour
{
    [Header("PhotonView")]
    [SerializeField] public PhotonView photon_view;

    [Header("Joystick [Android/IOS]")]
    public Joystick joystick;
    [Header("Buttons [Android/IOS]")]
    [SerializeField] List<GameObject> Buttons;

    public bool mobileInput;
    public float horizontalInput, verticalInput;


    // Start is called before the first frame update
    void Start()
    {
        if (photon_view.IsMine )
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            mobileInput = false;
#else
            mobileInput = true;
#endif
            if(joystick == null)
            {
               joystick = FindObjectOfType<Joystick>();
                if (!mobileInput)
                {
                    joystick.SetInactive();
                }
            }
           
        }
        if (!mobileInput)
        {
            HideButtons();
        }
    }


        // Update is called once per frame
        void Update()
    {
        if (!mobileInput)
        {
            HideButtons();
        }

    }
    public float MoveBoatHorizontal()
    {
        if (mobileInput)
        {
            horizontalInput = joystick.Horizontal;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
        return horizontalInput;
    }
    public float MoveBoatVertical()
    {
        if (mobileInput)
        {
            verticalInput = joystick.Vertical;
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical");
        }
        return verticalInput;
    }
    void HideButtons()
    {
        foreach (GameObject obj in Buttons)
        {
            if (obj != null)
            {
                // Disable the game object
                obj.SetInactive();
            }
        }
    }
}
