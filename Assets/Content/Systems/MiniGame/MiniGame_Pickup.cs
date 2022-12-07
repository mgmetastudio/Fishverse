using UnityEngine;

public class MiniGame_Pickup : MonoBehaviour
{
    public float rotation_speed = 20f;
    public MiniGame_Manager minigame_manager;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            Pickup();
        }
    }

    public void Pickup()
    {
        minigame_manager.AddScore();
        gameObject.SetActive(false);
    }
}
