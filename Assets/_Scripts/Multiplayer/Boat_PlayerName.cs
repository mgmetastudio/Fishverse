using UnityEngine;
using Photon.Pun;
using TMPro;

public class Boat_PlayerName : MonoBehaviour
{
    private PhotonView photon_view;
    public TMP_Text text_name;

    private void Start()
    {
        photon_view = GetComponent<PhotonView>();
        RefreshNameUI();
    }

    private void RefreshNameUI()
    {
        text_name.text = photon_view.Owner.NickName;
    }

    private void OnEnable()
    {
        MiniGame_Manager.OnNameEnter += NameTextEventHandler;
    }

    private string NameTextEventHandler()
    {
        return text_name.text;
    }

    private void OnDisable()
    {
        MiniGame_Manager.OnNameEnter -= NameTextEventHandler;
    }
}