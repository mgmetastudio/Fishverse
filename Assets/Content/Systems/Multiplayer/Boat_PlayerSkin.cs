using UnityEngine;
using Photon.Pun;

public class Boat_PlayerSkin : MonoBehaviour
{
    private PhotonView photon_view;
    public MeshRenderer boat_mesh;
    public Material[] boat_skins;

    public void Start()
    {
        photon_view = GetComponent<PhotonView>();
        if (photon_view.IsMine)
        {
            SetRandomSkin();
        }
    }

    public void SetRandomSkin()
    {
        int skin_index = Random.Range(0, boat_skins.Length);
        photon_view.RPC("RPC_SetRandomSkin", RpcTarget.AllBuffered, skin_index);
    }

    [PunRPC]
    public void RPC_SetRandomSkin(int skin_index)
    {
        boat_mesh.material = boat_skins[skin_index];
    }
}