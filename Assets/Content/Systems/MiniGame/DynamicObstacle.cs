using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DynamicObstacle : MonoBehaviour
{
    [SerializeField] Vector2 moveForce;
    [SerializeField] float moveTime;
    [SerializeField] Ease ease;

    Vector3 _startPos;
    void Start()
    {
        _startPos = transform.position;
    }


    void OnCollisionEnter(Collision other)
    {
        var dir = other.gameObject.transform.position.Direction(transform.position).WithY(0);
        transform.DOBlendableMoveBy(-dir * moveForce.GetRandom(), moveTime).SetEase(ease);
        transform.DOPunchScale(.5f * Vector3.one, moveTime);
    }
}
