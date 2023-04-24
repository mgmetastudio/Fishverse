using UnityEngine;

public class FishingShip_Effects : MonoBehaviour
{
    public ArcadeVehicleController boat_controller;
    [Tooltip("Trail_R, Trail_L, WaterSpray")]
    public ParticleSystem[] particles;
    public Transform boat_root;
    public float trail_backwards_z = 0f;


    public float max_y = 1.5f;
    private bool particles_enabled;
    private bool is_moving_forward;
    private Vector3 trail_r_local_pos;
    private Vector3 trail_l_local_pos;

    private ParticleSystem.EmissionModule emission_module;

    private void Start()
    {
        trail_r_local_pos = particles[0].transform.localPosition;
        trail_l_local_pos = particles[1].transform.localPosition;
    }

    private void Update()
    {
        if (boat_root.position.y > max_y && particles_enabled)
        {
            particles_enabled = false;

            foreach (ParticleSystem particle in particles)
            {
                ToggleEmissionModule(particle, false);
            }
        }
        else if(boat_root.position.y <= max_y && !particles_enabled)
        {
            particles_enabled = true;

            foreach (ParticleSystem particle in particles)
            {
                ToggleEmissionModule(particle, true);
            }
        }

        if (boat_controller.verticalInput < 0 && is_moving_forward)
        {
            is_moving_forward = false;
            SetBackwardsMovement();
        }
        else if (boat_controller.verticalInput >= 0 && !is_moving_forward)
        {
            is_moving_forward = true;
            SetForwardMovement();
        }
    }

    private void SetForwardMovement()
    {
        ToggleEmissionModule(particles[0], false);
        ToggleEmissionModule(particles[1], false);

        particles[0].transform.localPosition = trail_r_local_pos;
        particles[1].transform.localPosition = trail_l_local_pos;

        ToggleEmissionModule(particles[0], true);
        ToggleEmissionModule(particles[1], true);
    }

    private void SetBackwardsMovement()
    {
        ToggleEmissionModule(particles[0], false);
        ToggleEmissionModule(particles[1], false);

        particles[0].transform.localPosition = new Vector3(trail_r_local_pos.x, trail_r_local_pos.y, trail_backwards_z);
        particles[1].transform.localPosition = new Vector3(trail_l_local_pos.x, trail_l_local_pos.y, trail_backwards_z);

        ToggleEmissionModule(particles[0], true);
        ToggleEmissionModule(particles[1], true);
    }

    private void ToggleEmissionModule(ParticleSystem particle, bool new_visibility)
    {
        emission_module = particle.emission;
        emission_module.enabled = new_visibility;
    }
}
