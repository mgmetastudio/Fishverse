using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using UnityEngine;
using NullSave.TOCK.Inventory;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PhotonView photonView;
    [SerializeField] PlayerFishing PlayerFishing;
    [SerializeField] InventoryCog InventoryCog;
    [SerializeField] Category fishCategory;
    [Space]
    [Header("Timer and Fish Text")]
    [SerializeField] TMP_Text TimerText;
    [SerializeField] TMP_Text FishText;
    [SerializeField] public bool Isfishingfull=false;
    private bool hasStartedTimer = false;
    private OpenWorld_Manager OpenWorld_Manager;


    void Start()
    {
        OpenWorld_Manager = FindObjectOfType<OpenWorld_Manager>();
    }
    // Update is called once per frame
    void Update()
    {

        UpdateTimerText();
        FishText.text = InventoryCog.GetItems(fishCategory).Count + "/20";
        if (InventoryCog.GetItems(fishCategory).Count >= 20)
        {
            Isfishingfull = true;
        }
        else
        {
            Isfishingfull = false;
            PlayerFishing._Linebroke.SetBool("PackBack_Full", false);
        }

    }

    private void UpdateTimerText()
    {
        TimerText.text = OpenWorld_Manager.timer;
    }
}
