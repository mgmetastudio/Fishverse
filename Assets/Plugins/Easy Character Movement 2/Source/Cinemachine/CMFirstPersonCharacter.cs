using UnityEngine;
using Cinemachine;

namespace EasyCharacterMovement.Examples.Cinemachine.FirstPersonExample
{
    /// <summary>
    /// This example shows how to extend the FirstPersonCharacter to use Cinemachine Cameras
    /// instead of default camera, this way you can take advantage of Cinemachine features
    /// while retaining the same ECM2 functionality.
    ///
    /// Here shows how to replace the default programatically animation to use Cinemachine to perform crouch / un crouch animation.
    /// </summary>

    public class CMFirstPersonCharacter : FirstPersonCharacter
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Cinemachine")]
        public GameObject cmWalkingCamera;
        public GameObject cmCrouchedCamera;

        [SerializeField] CinemachineVirtualCamera fpCam;
        [SerializeField] CinemachineFreeLook tpCam;

        CinemachinePOV fpPOV;

        bool _cameraLockInput;
        float _speedY;

        #endregion

        #region METHODS

        protected override void Start()
        {
            base.Start();
            fpPOV = fpCam.GetCinemachineComponent<CinemachinePOV>();
            _speedY = tpCam.m_YAxis.m_MaxSpeed;
        }

        protected override void AnimateEye()
        {
            // Removes programatically crouch / un crouch animation as this will be handled by Cinemachine cameras
        }

        protected override void OnCrouched()
        {
            // Call base method

            base.OnCrouched();

            // Transition to crouched cinemachine camera

            cmWalkingCamera.SetActive(false);
            cmCrouchedCamera.SetActive(true);
        }

        protected override void OnUnCrouched()
        {
            // Call base method

            base.OnUnCrouched();

            // Transition to un crouched cinemachine camera

            cmCrouchedCamera.SetActive(false);
            cmWalkingCamera.SetActive(true);
        }

        public void ToggleCameraInput(bool value)
        {
            _cameraLockInput = value;
            tpCam.m_YAxis.m_MaxSpeed = value ? 0f : _speedY;
        }

        protected override void HandleCameraInput()
        {
            if (_cameraLockInput)
            {
                return;
            }

            base.HandleCameraInput();

            if (customInput)
            {
                fpPOV.m_VerticalAxis.m_InputAxisValue = LookInput.y * .5f;
                tpCam.m_YAxis.m_InputAxisValue = LookInput.y * .5f;
            }
        }

        #endregion
    }
}