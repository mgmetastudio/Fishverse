using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWorldShop : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;

    void OnTriggerEnter(Collider other)
    {
        if (playerMask.Includes(other.gameObject.layer))
            OnPlayerEnter(other.gameObject);
    }

    void OnPlayerEnter(GameObject playerObj)
    {
        // var inv = playerObj.GetComponent<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>();
        // var shopInv = playerObj.GetComponentInChildren<Opsive.UltimateInventorySystem.UI.Menus.ShopMenuOpener>();
        
        // shopInv.Open(inv);
    }
}
