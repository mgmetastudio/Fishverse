using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace NullSave.TOCK.Inventory
{
    public class TooltipBait : MonoBehaviour
    {
        [Header("Tooltip Bait ")]
        public GameObject TooltipBait_;
        [Header("Panel Instantiate ")]
        public Transform Parent;
        private GameObject tooltipInstance;// Instance of the TooltipBait
        public string AlertText;
        private bool Isshowing;
        // Start is called before the first frame update
        public void ShowTooltipBait()
        {
            Isshowing = true;
            if (tooltipInstance == null && TooltipBait_ != null)
            {
                tooltipInstance = Instantiate(TooltipBait_, Parent.transform); 
                TooltipBait_.GetComponent<Animator>().Play("OpenTooltip");

            }
            else if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
                tooltipInstance = Instantiate(TooltipBait_, Parent.transform);
            }
        }
        public void Update()
        {
            if (Isshowing)
            {
                tooltipInstance.GetComponentInChildren<TMP_Text>().text = AlertText;
                Isshowing = false;
            }
            
        }
        public void HideTooltipBait()
        {
            TooltipBait_.SetActive(false);
        }

    }
}
