#if GDTK
using UnityEngine;

namespace NullSave.GDTK.Stats.Demos
{
    public class DemoStatCharacterController : MonoBehaviour
    {

        #region Fields

        [Header("Input")]
        public string inputHorizontal = "Horizontal";
        public string inputVertical = "Vertical";
        public string inputSprint = "Fire3";

        public bool cameraTranslation = true;

        [Header("Speeds")]
        public float standingRotationSpeed = 360;
        public float movingRotationSpeed = 360;
        public float moveSpeedMultiplier = 1;

        [Header("Stats")]
        public string speedStatId = "speed";
        public string sprintStatId = "sprintSpeed";
        public string sprintCostStatId = "sprintCost";
        public string staminaStatId = "stamina";

        [Header("Animator Settings")]
        public string movement = "MoveSpeed";

        private BasicStats stats;
        private GDTKStat speed, sprint, sprintCost, stamina;
        private bool wantsSprint;

        #endregion

        #region Properties

        public Animator Animator { get; private set; }

        public CharacterController CharacterController { get; private set; }

        public bool InAction { get; set; }

        public bool IsSprinting { get; private set; }

        public Vector3 Movement { get; set; }

        public bool PreventMovement { get; set; }

        public Camera ViewCamera { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            CharacterController = GetComponent<CharacterController>();
            ViewCamera = Camera.main;
            stats = GetComponent<BasicStats>();
            stats.onStatsReloaded += GetStats;
            GetStats();
        }

        private void Update()
        {
            UpdateInput();
            Move();
        }

        #endregion

        #region Private Methods
        
        private void GetStats()
        {
            speed = stats.GetStat(speedStatId);
            sprint = stats.GetStat(sprintStatId);
            sprintCost = stats.GetStat(sprintCostStatId);
            stamina = stats.GetStat(staminaStatId);
        }

        private void Move()
        {
            if (!enabled || Movement.magnitude < 0.1f)
            {
                Animator.SetFloat(movement, 0, 0.1f, Time.deltaTime);
                CharacterController.Move(Physics.gravity * Time.deltaTime);
                return;
            }

            Vector3 move = Movement;
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);

            Vector3 groundNormal = Vector3.up;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out RaycastHit hitInfo, 0.3f))
            {
                groundNormal = hitInfo.normal;
            }

            // Apply rotation
            if (!InAction && Movement.magnitude > 0)
            {
                float turnAmount = Mathf.Atan2(move.x, move.z);
                float turnSpeed = Mathf.Lerp(standingRotationSpeed, movingRotationSpeed, move.z);
                transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
            }

            if (PreventMovement || InAction)
            {
                Animator.SetFloat(movement, 0, 0.1f, Time.deltaTime);
                return;
            }

            move = Vector3.ProjectOnPlane(move, groundNormal);

            float useSpeed = speed.value;

            if (!IsSprinting)
            {
                if (InterfaceManager.Input.GetButtonDown(inputSprint) || wantsSprint)
                {
                    if (stamina.value >= sprintCost.value)
                    {
                        IsSprinting = true;
                        useSpeed *= sprint.value;
                        stamina.value -= sprintCost.value * Time.deltaTime;
                        wantsSprint = false;
                    }
                    else
                    {
                        wantsSprint = true;
                    }
                }
            }
            else
            {
                if (!InterfaceManager.Input.GetButton(inputSprint) || stamina.value == 0)
                {
                    IsSprinting = false;
                }
                else
                {
                    useSpeed *= sprint.value;
                    stamina.value -= sprintCost.value * Time.deltaTime;
                }
            }

            // Update Animator
            Animator.SetFloat(movement, IsSprinting ? move.z * sprint.value : move.z, 0.1f, Time.deltaTime);

            if (!Animator.applyRootMotion)
            {
                Vector3 moveDir = transform.forward * (useSpeed * moveSpeedMultiplier) + Physics.gravity;
                CharacterController.Move(moveDir * Time.deltaTime);
            }
            else
            {
                CharacterController.Move(Physics.gravity * Time.deltaTime);
            }
        }

        private void UpdateInput()
        {
            float v = InterfaceManager.Input.GetAxis(inputVertical);
            float h = InterfaceManager.Input.GetAxis(inputHorizontal);

            // Normalize input
            if (cameraTranslation && ViewCamera != null)
            {
                Movement = v * Vector3.Scale(ViewCamera.transform.forward, new Vector3(1, 0, 1)).normalized + h * ViewCamera.transform.right;
            }
            else
            {
                Movement = v * Vector3.forward + h * Vector3.right;
            }
        }

        #endregion

    }
}
#endif