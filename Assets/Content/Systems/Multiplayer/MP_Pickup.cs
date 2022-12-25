using UnityEngine;
using Photon.Pun;

public class MP_Pickup : MonoBehaviour
{
    public float rotation_speed = 20f;

    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotation_speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat") && gameObject.activeSelf)
        {
            other.GetComponent<Boat_PlayerScore>().AddScore();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
