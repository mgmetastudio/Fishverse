using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerVehicleManager : MonoBehaviour
{
    public GameObject spawnedBoat;

    public void DestroyBoat()
    {
        PhotonNetwork.Destroy(spawnedBoat);
    }
}
