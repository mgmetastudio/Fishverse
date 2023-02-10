using Cinemachine;
using UnityEngine;

public static class CinemachineExtensions
{
    ///<summary>
    ///Return position offset of camera
    ///</summary>
    public static Vector3 PositionOffset(this CinemachineVirtualCameraBase cam)
    {
        var camTransposer = (cam as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineTransposer>();
        return camTransposer.m_FollowOffset;
    }

    ///<summary>
    ///Returns look offset of camera
    ///</summary>
    public static Vector3 TrackingOffset(this CinemachineVirtualCameraBase cam)
    {
        var camComposer = (cam as CinemachineVirtualCamera).GetCinemachineComponent<CinemachineComposer>();
        return camComposer.m_TrackedObjectOffset;
    }

    public static void SetTarget(this CinemachineVirtualCameraBase cam, Transform followTarget, Transform lookTarget = null)
    {
        cam.Follow = followTarget;
        cam.LookAt = lookTarget == null ? followTarget : lookTarget;
    }
}
