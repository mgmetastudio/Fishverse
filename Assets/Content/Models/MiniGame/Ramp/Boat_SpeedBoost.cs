using UnityEngine;

public class Boat_SpeedBoost : MonoBehaviour
{
    private float max_speed_normal;
    private float acceleration_normal;
    private ArcadeVehicleController boat;
    public float speed_mp = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            boat = other.GetComponent<ArcadeVehicleController>();

            max_speed_normal = boat.MaxSpeed;
            acceleration_normal = boat.accelaration;

            boat.MaxSpeed *= speed_mp;
            boat.accelaration *= speed_mp;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            if (boat)
            {
                boat.MaxSpeed = max_speed_normal;
                boat.accelaration = acceleration_normal;
                boat = null;
            }
        }
    }
}
