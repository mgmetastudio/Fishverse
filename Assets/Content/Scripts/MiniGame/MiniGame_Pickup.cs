using UnityEngine;

public class MiniGame_Pickup : MonoBehaviour
{
    public float rotation_speed = 20f;
    public bool pickup_energy;
    public bool pickup_score;
    public bool pickup_question;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void Pickup()
    {
        if (pickup_energy)
        {
            FindObjectOfType<MiniGame_API>().AddEnergy();
        }

        if (pickup_score)
        {
            FindObjectOfType<MiniGame_API>().AddScore();
        }

        if(pickup_question)
        {
            FindObjectOfType<MiniGame_API>().RandomBonus();
        }

        Destroy(gameObject);
    }
}
