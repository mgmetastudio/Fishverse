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
        var inv = playerObj.GetComponent<Inventory>();
        foreach (var item in inv.Fishes)
        {
            inv.money += System.Int32.Parse(item.GetComponent<FishAIController>()._scriptable.FishRetailValue);
        }
        inv.Fishes = new List<GameObject>();
    }
}
