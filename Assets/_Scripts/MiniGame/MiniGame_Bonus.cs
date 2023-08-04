using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class MiniGame_Bonus : MonoBehaviour
{
    public float rotation_speed = 20f;
    public MiniGame_Manager minigame_manager;
    public BonusType current_type;
    public GameObject destroy_fx;
    public enum BonusType { Time, Coins, CoinsBig, Nitro, Fuel }
    public UnityEvent on_pickup;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            PickupBonus(current_type, other.gameObject);

            on_pickup.Invoke();

            if (destroy_fx)
            {
                Instantiate(destroy_fx, transform.position, transform.rotation);
            }
        }
    }

    public void PickupBonus(BonusType bonus_type, GameObject target)
    {
        if (bonus_type == MiniGame_Bonus.BonusType.Time)
        {
            if (target.GetComponent<PhotonView>().IsMine)
                minigame_manager.AddBonus(bonus_type, target);
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.Coins)
        {
            target.GetComponent<Boat_PlayerScore>().AddScore(15);
            if (target.GetComponent<PhotonView>().IsMine)
                minigame_manager.AddBonus(bonus_type, target);
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.CoinsBig)
        {
            target.GetComponent<Boat_PlayerScore>().AddScore(35);
            if (target.GetComponent<PhotonView>().IsMine)
                minigame_manager.AddBonus(bonus_type, target);
        }

        else if (bonus_type == MiniGame_Bonus.BonusType.Nitro)
        {
            if (target.GetComponent<PhotonView>().IsMine)
            {

                var vehicle_nitro = target.GetComponent<ArcadeVehicleNitro>();
                vehicle_nitro.nitro = 1;
                vehicle_nitro.RefreshNitroUI();
            }
        }
        else if (bonus_type == MiniGame_Bonus.BonusType.Fuel)
        {
            if (target.GetComponent<PhotonView>().IsMine)
            {
                var vehicle_fuel = target.GetComponent<ArcadeVehicleController>();

                vehicle_fuel.Refuel(20);
            }
        }
        target.GetComponent<Boat_PlayerScore>().AddScore();
        // gameObject.SetActive(false);
        PhotonNetwork.Destroy(gameObject);

        FindObjectOfType<UserListManager>().RefreshUserList();
    }
}
