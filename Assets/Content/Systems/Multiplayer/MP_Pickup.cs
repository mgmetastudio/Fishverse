using UnityEngine;
using Photon.Pun;

public class MP_Pickup : MonoBehaviour
{
    public float rotation_speed = 400f;
    public GameObject pickup_fx;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            other.GetComponent<Boat_PlayerScore>().AddScore();

            if (pickup_fx)
            {
                Instantiate(pickup_fx, transform.position, transform.rotation);
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }
}
