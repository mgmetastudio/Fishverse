using UnityEngine;

namespace EasyCharacterMovement.Examples.Cinemachine.ThirdPersonExample
{
    /// <summary>
    /// This example shows how to extend the Character class to implement a Cinemachine based third person camera.
    ///
    /// This uses the Cinemachine 3rd person follow method to implement the third person camera.
    /// </summary>

    public class CMThirdPersonCharacter : Character
    {
        #region EDITOR EXPOSED FIELDS

        public Transform cameraTarget;

        #endregion

        #region FIELDS

        private float _pitch;
        private float _yaw;

        private CharacterLook _characterLook;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached CharacterLook component.
        /// </summary>

        protected CharacterLook characterLook
        {
            get
            {
                if (_characterLook == null)
                    _characterLook = GetComponent<CharacterLook>();

                return _characterLook;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Rotate the camera along its yaw.
        /// </summary>

        public void AddCameraYawInput(float value)
        {
            _yaw = MathLib.Clamp0360(_yaw + value);
        }

        /// <summary>
        /// Rotate the camera along its pitch.
        /// </summary>

        public void AddCameraPitchInput(float value)
        {
            value = characterLook.invertLook ? value : -value;

            _pitch = Mathf.Clamp(_pitch + value, characterLook.minPitchAngle, characterLook.maxPitchAngle);
        }

        /// <summary>
        /// Extends HandleInput method to add camera input.
        /// </summary>

        protected override void HandleInput()
        {
            // Call base method

            base.HandleInput();

            // Camera input (mouse look),
            // Rotates the camera target independently of the Character's rotation,
            // basically we are manually rotating the Cinemachine camera here

            if (IsDisabled())
                return;

            Vector2 mouseLookInput = new Vector2
            {
                x = Input.GetAxisRaw("Mouse X"),
                y = Input.GetAxisRaw("Mouse Y"),
            };

            if (mouseLookInput.x != 0.0f)
                AddCameraYawInput(mouseLookInput.x * characterLook.mouseHorizontalSensitivity);

            if (mouseLookInput.y != 0.0f)
                AddCameraPitchInput(-mouseLookInput.y * characterLook.mouseVerticalSensitivity);

            // Mouse lock / unlock

            if (Input.GetMouseButtonDown(0))
            {
                _characterLook.LockCursor();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _characterLook.UnlockCursor();
            }
        }

        protected override void OnLateUpdate()
        {
            // Call base method

            base.OnLateUpdate();

            // Set final camera rotation

            cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
        }

        /// <summary>
        /// Overrides OnStart to initialize this.
        /// </summary>

        protected override void OnStart()
        {
            // Call base method

            base.OnStart();

            // Cache camera's initial orientation (yaw / pitch)

            Vector3 cameraTargetEulerAngles = cameraTarget.eulerAngles;

            _pitch = cameraTargetEulerAngles.x;
            _yaw = cameraTargetEulerAngles.y;
        }

        #endregion
    }
}
