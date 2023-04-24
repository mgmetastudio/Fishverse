using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FishCatchVFXController : MonoBehaviour
{
    [SerializeField] Transform fishGfx;
    [SerializeField] float jumpPower;
    [SerializeField] float jumpTime;

    void Start()
    {

    }

    public void Play(Vector3 startPos, Vector3 endPos)
    {
        //change to pooling
        var fish = Instantiate(fishGfx, startPos, Camera.main.transform.rotation * Quaternion.Euler(0, 90, 0));
        fish.DOJump(endPos, jumpPower, 1, jumpTime).OnComplete(fish.SetInactive);
        fish.DOScale(0, jumpTime).SetEase(Ease.InExpo);

    }
}
