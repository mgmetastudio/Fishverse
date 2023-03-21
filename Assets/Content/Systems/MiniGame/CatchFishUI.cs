using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchFishUI : MonoBehaviour
{
    MiniGame_Fishes _fishes;

    public void CatchFish()
    {
        if (_fishes == null)
            _fishes = FindObjectOfType<MiniGame_Fishes>();

        _fishes.CatchFish();
    }
}
