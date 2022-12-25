using UnityEngine;
using UnityEngine.Events;

public class MiniGame_Bonus : MonoBehaviour
{
    public float rotation_speed = 20f;
    public MiniGame_Manager minigame_manager;
    public BonusType current_type;
    public GameObject destroy_fx;
    public enum BonusType {Time, Coins, CoinsBig, Nitro}
    public UnityEvent on_pickup;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            PickupBonus(current_type);

            on_pickup.Invoke();

            if (destroy_fx)
            {
                Instantiate(destroy_fx, transform.position, transform.rotation);
            }
        }
    }

    public void PickupBonus(BonusType bonus_type)
    {
        minigame_manager.AddBonus(bonus_type);
        gameObject.SetActive(false);
    }
}
