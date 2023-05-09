using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishUtil : MonoBehaviour
{
    [ContextMenu("FIX")]
    public void Fix()
    {
        DestroyImmediate(GetComponentInChildren<Animator>());
        foreach (var item in GetComponentsInChildren<Collider>())
        {
        DestroyImmediate(item);
            
        }

        DestroyImmediate(this);

        return;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        foreach (Transform  item in transform)
        {
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
            
        }
        // var childToFix = transform.GetChild(0);

        // childToFix = transform.GetChild(1);
        // childToFix.localPosition = Vector3.zero;
        // childToFix.localRotation = Quaternion.identity;

    }
}
