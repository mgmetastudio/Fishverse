using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Boat_PlayerName : NetworkBehaviour
{
    public NetworkVariable<string> player_name = new NetworkVariable<string>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public TMP_Text text_name;

    public override void OnNetworkSpawn()
    {
        player_name.OnValueChanged += (string old_name, string new_name) =>
        {
            RefreshNameUI();
        };
    }

    private void Start()
    {
        if (IsOwner)
        {
            int random_index = Random.Range(1000, 10000);
            player_name.Value = "Guest_" + random_index.ToString();
        }
    }

    private void RefreshNameUI()
    {
        text_name.text = player_name.Value;
    }
}