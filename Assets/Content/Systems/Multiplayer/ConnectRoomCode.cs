using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectRoomCode : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField roomCode;
    [SerializeField] LobbyManager lobby;

    public void Connect()
    {
        string code = roomCode.text.ToUpper();

        lobby.ConnectJoin(code);
    }
}
