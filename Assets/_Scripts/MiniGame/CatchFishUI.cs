using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatchFishUI : MonoBehaviour
{
    List<MiniGame_Fishes> _fishSpots;
    MiniGame_Fishes _fishSpot;

    Transform _player;

    void Start()
    {
        _fishSpots = FindObjectsOfType<MiniGame_Fishes>().ToList();

    }

    public void CatchFish()
    {
        if (!_player)
            _player = FindObjectsOfType<ArcadeVehicleController_Network>().First(x => x.photon_view.IsMine).transform;

        _fishSpot = _player.transform.GetClosestTransform(_fishSpots);

        // if (_fishes == null)
        //     _fishes = FindObjectOfType<MiniGame_Fishes>();


        _fishSpot.CatchFish();
    }
}
