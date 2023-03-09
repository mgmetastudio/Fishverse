using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FinishTrigger : MonoBehaviour
{
    public UnityEvent onFinish;

    void OnTriggerEnter(Collider other)
    {
        onFinish.Invoke();
    }
}
