using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample;
using NullSave.TOCK.Inventory;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoatSpawner : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;
    [SerializeField] KeyCode inputKey = KeyCode.F;
    [SerializeField] Transform spawnPoint;

    [SerializeField] Button promptBtn;

    InventoryCog _player;

    public static readonly string boatTag = "Boat";

    void Start()
    {
        promptBtn.SetInactive();
    }

    void Update()
    {
        if (!_player) return;

        if (Input.GetKeyDown(inputKey))
            SpawnBoat();

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != _player.gameObject) return;

        promptBtn.SetInactive();

        _player = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!playerMask.Includes(other.gameObject.layer)) return;

        promptBtn.SetActive();

        _player = other.gameObject.GetComponent<InventoryCog>();
    }

    void SpawnBoat()
    {
        var boatItem = _player.Items.FirstOrDefault(x => x.customTags.Exists(x => x.Name == boatTag));
        if (!boatItem) return;

        var tag = boatItem.customTags.First(x => x.Name == boatTag);

        if(tag.Value == "1") return;

        tag.Value = "1";
        PhotonNetwork.Instantiate(boatItem.previewObject.name, spawnPoint.position, spawnPoint.rotation);
        
    }
}
