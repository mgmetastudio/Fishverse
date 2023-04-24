using UnityEngine;

public class FishingShip_Animations : MonoBehaviour
{
    [Header("References")]
    public ArcadeVehicleController boat_controller;
    public Transform motor_r;
    public Transform motor_l;
    public Transform rotor_r;
    public Transform rotor_l;

    [Header("Settings")]
    public float max_rotor_speed = 3000f;
    public float min_motor_rotation = -15f;
    public float max_motor_rotation = 15f;

    private float current_rotor_speed;
    private Vector3 start_motor_rotation;

    private void Start()
    {
        start_motor_rotation = motor_l.localEulerAngles;
    }

    void Update()
    {
        current_rotor_speed = Mathf.Lerp(0, max_rotor_speed, boat_controller.verticalInput);
        rotor_r.Rotate(Vector3.up, current_rotor_speed * Time.deltaTime);
        rotor_l.Rotate(Vector3.up, current_rotor_speed * Time.deltaTime);

        start_motor_rotation.z = Mathf.Lerp(min_motor_rotation, max_motor_rotation, 1 - (boat_controller.horizontalInput + 1) * 0.5f);
        motor_r.localEulerAngles = start_motor_rotation;
        motor_l.localEulerAngles = start_motor_rotation;
    }
}
