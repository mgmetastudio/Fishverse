using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Boat_Horn : MonoBehaviour
{
    private PhotonView photon_view;
    public AudioSource sound_horn;
    private Button button_horn;

    private void Start()
    {
        photon_view = GetComponent<PhotonView>();

        if (photon_view.IsMine)
        {
            button_horn = GameObject.Find("Btn_Horn").GetComponent<Button>();
            button_horn.onClick.AddListener(PlayHorn);
        }
    }

    public void PlayHorn()
    {
        if (photon_view.IsMine)
        {
            photon_view.RPC("RPC_PlayBoatHorn", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_PlayBoatHorn()
    {
        sound_horn.Play();
    }
}
