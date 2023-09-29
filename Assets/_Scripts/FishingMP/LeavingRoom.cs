using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class LeavingRoom : MonoBehaviourPunCallbacks
{
    public MiniGame_Manager MiniGame_Manager;
    public GameObject PanelPause;
    public GameObject PanelEndGame;
    public void Leave()
    {
        if (PhotonNetwork.IsConnected)
        {
            MixerManager.Instance.UpdateAudioSource(SoundType.MainMenu);
            PhotonNetwork.LeaveRoom();
        }
    }
    public void CloseLeaderboard()
    {
        if (MiniGame_Manager.gameLeaderBoard)
        {
            PanelPause.SetActive(true);

        }
        else
        {
            PanelEndGame.SetActive(true);
        }

    }


}