using UnityEngine;

public class MiniGame_Bonus : MonoBehaviour
{
    public float rotation_speed = 20f;
    public MiniGame_Manager minigame_manager;
    public BonusType current_type;
    public enum BonusType {Time, Coins, CoinsBig, Nitro}

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            PickupBonus(current_type);
        }
    }

    public void PickupBonus(BonusType bonus_type)
    {
        minigame_manager.AddBonus(bonus_type);
        gameObject.SetActive(false);
    }
}
