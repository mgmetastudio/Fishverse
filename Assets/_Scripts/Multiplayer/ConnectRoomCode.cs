using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ConnectRoomCode : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField roomCode;
    [SerializeField] LobbyManager lobby;

    public void Connect()
    {
        string code = roomCode.text.ToUpper();
        
        if (!PhotonNetwork.JoinRoom(code))
        {
            print("No such room");
        }
    }
}
