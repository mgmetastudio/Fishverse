using UnityEngine;

public class FishingShip_Net : MonoBehaviour
{
    [Header("References")]
    public ArcadeVehicleController boat_controller;
    public Animator net_anim;

    private void Update()
    {
        net_anim.SetFloat("Input", boat_controller.verticalInput);
    }
}
