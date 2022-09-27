using UnityEngine;

public class Player_Root : MonoBehaviour
{
    public enum PlayerState {BoatWalking, BoatDriving, GroundWalking};

    [Header("Settings")]
    public PlayerState player_state;

    [Space(10)]
    [Header("FPS Player")]
    public FPS_Movement player_controller;
    public GameObject player_camera;
    public GameObject[] fps_ui_controls;

    [Space(10)]
    [Header("Boat Player")]
    public Boat_Movement boat_controller;
    public GameObject boat_camera;
    public GameObject[] boat_ui_controls;
    public Vector3 player_position_boat = new Vector3(-0.08f, 1.48f, 0f);
    public Vector3 player_position_platform;
    public Transform boat_floating_root;
    public GameObject boat_character;

    private float boat_collider_radius = 2;
    private Vector3 boat_collider_center;

    private void Start()
    {
        boat_collider_radius = boat_controller.controller.radius;
        boat_collider_center = boat_controller.controller.center;

        if (player_state == PlayerState.BoatWalking)
            SwitchState_BoatWalking();
        else if (player_state == PlayerState.BoatDriving)
            SwitchState_BoatDriving();
        else if (player_state == PlayerState.GroundWalking)
            SwitchState_GroundWalking();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchState_BoatDriving();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchState_BoatWalking();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchState_GroundWalking();
        }
    }

    public void SwitchState_BoatWalking()
    {
        player_state = PlayerState.BoatWalking;

        foreach (GameObject ui_control in fps_ui_controls)
        {
            ui_control.SetActive(true);
        }

        boat_controller.enabled = false;
        boat_controller.controller.radius = 0;
        boat_controller.controller.center = new Vector3(0, 999, 0);
        boat_camera.SetActive(false);
        boat_character.SetActive(false);

        player_controller.gameObject.SetActive(true);
        player_controller.transform.SetParent(boat_floating_root);
        player_controller.transform.localPosition = player_position_boat;

        player_camera.gameObject.SetActive(true);

        foreach (GameObject ui_control in boat_ui_controls)
        {
            ui_control.SetActive(false);
        }
    }

    public void SwitchState_GroundWalking()
    {
        player_state = PlayerState.GroundWalking;

        foreach (GameObject ui_control in fps_ui_controls)
        {
            ui_control.SetActive(true);
        }

        boat_controller.enabled = false;
        boat_controller.controller.radius = 0;
        boat_controller.controller.center = new Vector3(0, 999, 0);
        boat_camera.SetActive(false);
        boat_character.SetActive(false);

        player_controller.gameObject.SetActive(true);
        player_controller.transform.SetParent(transform);
        player_controller.transform.position = player_position_platform;

        player_camera.gameObject.SetActive(true);

        foreach (GameObject ui_control in boat_ui_controls)
        {
            ui_control.SetActive(false);
        }
    }

    public void SwitchState_BoatDriving()
    {
        player_state = PlayerState.BoatDriving;

        foreach (GameObject ui_control in fps_ui_controls)
        {
            ui_control.SetActive(false);
        }

        boat_controller.enabled = true;
        boat_controller.controller.radius = boat_collider_radius;
        boat_controller.controller.center = boat_collider_center;
        boat_camera.SetActive(true);
        boat_character.SetActive(true);

        player_controller.gameObject.SetActive(false);
        player_controller.transform.SetParent(boat_floating_root);

        player_camera.gameObject.SetActive(false);

        foreach (GameObject ui_control in boat_ui_controls)
        {
            ui_control.SetActive(true);
        }
    }

    public void DisableMovement()
    {
        if (player_state == PlayerState.GroundWalking || player_state == PlayerState.BoatWalking)
        {
            player_controller.can_move = false;
            player_camera.GetComponent<FPS_Camera>().can_move = false;

            foreach (GameObject ui_control in fps_ui_controls)
            {
                ui_control.SetActive(false);
            }
        }
    }

    public void EnableMovement()
    {
        if (player_state == PlayerState.GroundWalking || player_state == PlayerState.BoatWalking)
        {
            player_controller.can_move = true;
            player_camera.GetComponent<FPS_Camera>().can_move = true;

            foreach (GameObject ui_control in fps_ui_controls)
            {
                ui_control.SetActive(true);
            }
        }
    }
}
