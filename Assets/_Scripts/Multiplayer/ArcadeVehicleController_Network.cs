using UnityEngine;
using Photon.Pun;
public class ArcadeVehicleController_Network : ArcadeVehicleController
{

    [HideInInspector] public PhotonView photon_view;
    public GameObject virtual_camera;

    [SerializeField] bool disableOnStart;

    [Header("Input script for Devices")]
    public BoatInputProxy BoatInputProxy;
    public override void Start()
    {
        base.Start();

        photon_view = GetComponent<PhotonView>();

        if (!photon_view.IsMine)
        {
            rb.isKinematic = true;
            carBody.isKinematic = true;
            virtual_camera.SetActive(false);
            engineSound.volume = 0.2f;
        }
        else
        {
            // joystick = FindObjectOfType<Joystick>(true);
            // carBody.useGravity = true;
            // carBody.isKinemat = true;
            radius = rb.GetComponent<SphereCollider>().radius;
            if (movementMode == MovementMode.AngularVelocity)
            {
                Physics.defaultMaxAngularSpeed = 100;
            }
        }

        if(disableOnStart)
            enabled = false;

    }
    public override void Update()
    {
        if (!photon_view.IsMine)
        {
            return;
        }
        base.Update();
    }


    public override void FixedUpdate()
    {
        if (!photon_view.IsMine)
        {
            return;
        }

        base.FixedUpdate();
    }
}
