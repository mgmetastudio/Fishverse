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
        var inv = playerObj.GetComponent<Opsive.UltimateInventorySystem.Core.InventoryCollections.Inventory>();
        var shopInv = playerObj.GetComponentInChildren<Opsive.UltimateInventorySystem.UI.Menus.ShopMenuOpener>();
        // print("INV: " + inv);
        // print("Shop: " + shop);
        
        shopInv.Open(inv);

        // int amount = 0;
        // foreach (var item in inv.fishInv)
        // {
        //     string resultString = System.Text.RegularExpressions.Regex.Match(item.FishRetailValue.text, @"\d+").Value;
        //     amount += System.Int32.Parse(resultString);
        // }
        // inv.SellAllFish(amount);
    }
}
