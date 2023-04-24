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
            if (other.GetComponent<ArcadeVehicleController_Network>())
            {
                OnTriggerEnter_MP(other);
                return;
            }

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
            if (other.GetComponent<ArcadeVehicleController_Network>())
            {
                OnTriggerExit_MP(other);
                return;
            }

            if (boat)
            {
                boat.MaxSpeed = max_speed_normal;
                boat.accelaration = acceleration_normal;
                boat = null;
            }
        }
    }

    private void OnTriggerEnter_MP(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            ArcadeVehicleController_Network boat_mp = other.GetComponent<ArcadeVehicleController_Network>();

            max_speed_normal = boat_mp.MaxSpeed;
            acceleration_normal = boat_mp.accelaration;

            boat_mp.MaxSpeed *= speed_mp;
            boat_mp.accelaration *= speed_mp;
        }
    }

    private void OnTriggerExit_MP(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            ArcadeVehicleController_Network boat_mp = other.GetComponent<ArcadeVehicleController_Network>();

            boat_mp.MaxSpeed = max_speed_normal;
            boat_mp.accelaration = acceleration_normal;
        }
    }
}
