using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NullSave.TOCK.Inventory
{
    public class TooltipBait : MonoBehaviour
    {
        [Header("Tooltip Bait ")]
        public GameObject TooltipBait_;
        [Header("Panel Instantiate ")]
        public Transform Parent;
        private GameObject tooltipInstance; // Instance of the TooltipBait

        // Start is called before the first frame update
        public void ShowTooltipBait()
        {
            if (tooltipInstance == null && TooltipBait_ != null)
            {
                tooltipInstance = Instantiate(TooltipBait_, Parent.transform);
            }
            else if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
                tooltipInstance = Instantiate(TooltipBait_, Parent.transform);
            }
        }
        public void HideTooltipBait()
        {
            TooltipBait_.SetActive(false);
        }

    }
}
