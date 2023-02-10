using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class MonoBehaviourExtensions
{
    ///<summary>
    ///Sets MonoBehaviour dirty
    ///</summary>
    public static void SetSelfDirty(this MonoBehaviour monoBehaviour)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(monoBehaviour);
#endif
    }

    ///<summary>
    ///Invokes action with delay
    ///<param = "time">deley in seconds before invoking Action </param>
    ///<param = "action">Action to invoke</param>
    ///</summary>
    public static async void Invoke(this MonoBehaviour monoBehaviour, float delay, Action action)
    {
	await UniTask.WaitForSeconds(delay);
        action.Invoke();
    }

    ///<summary>
    ///Invokes action with delay
    ///<param = "frames">deley in frames before invoking Action </param>
    ///<param = "action">Action to invoke</param>
    ///</summary>
    public static async void Invoke(this MonoBehaviour monoBehaviour, int frames, Action action)
    {
        await UniTask.DelayFrame(frames);
        action.Invoke();
    }

    /// <summary>Enables MonoBehaviour</summary>
	public static void SetEnabled(this MonoBehaviour monoBehaviour)
    {
        monoBehaviour.enabled = true;
    }

    /// <summary>Disables MonoBehaviour</summary>
	public static void SetDisabled(this MonoBehaviour monoBehaviour)
    {
        monoBehaviour.enabled = false;
    }

    /// <summary>Enables or disables MonoBehaviour</summary>
	public static void SetEnabled(this MonoBehaviour monoBehaviour, bool enabled)
    {
        monoBehaviour.enabled = enabled;
    }

    /// <summary>Enables MonoBehaviour</summary>
    /// <param name="delay">Time after which MonoBehaviour will be enabled or disabled.</param>
    public static void SetEnabled(this MonoBehaviour monoBehaviour, float delay)
    {
        monoBehaviour.SetEnabled(true, delay);
    }

    /// <summary>Disables MonoBehaviour</summary>
    /// <param name="delay">Time after which MonoBehaviour will be enabled or disabled.</param>
    public static void SetDisabled(this MonoBehaviour monoBehaviour, float delay)
    {
        monoBehaviour.SetEnabled(false, delay);
    }

    /// <summary>Enables or disables MonoBehaviour</summary>
    /// <param name="delay">Time after which MonoBehaviour will be enabled or disabled.</param>
    public static async void SetEnabled(this MonoBehaviour monoBehaviour, bool enabled, float delay)
    {
	await UniTask.WaitForSeconds(delay);
        monoBehaviour.enabled = enabled;
    }

    /// <summary>Enables MonoBehaviour</summary>
    /// <param name="frames">Frames after which MonoBehaviour will be enabled or disabled.</param>
    public static void SetEnabled(this MonoBehaviour monoBehaviour, int frames)
    {
        monoBehaviour.SetEnabled(true, frames);
    }

    /// <summary>Disables MonoBehaviour</summary>
    /// <param name="frames">Frames after which MonoBehaviour will be enabled or disabled.</param>
    public static void SetDisabled(this MonoBehaviour monoBehaviour, int frames)
    {
        monoBehaviour.SetEnabled(false, frames);
    }

    /// <summary>Enables or disables MonoBehaviour</summary>
    /// <param name="frames">Frames after which MonoBehaviour will be enabled or disabled.</param>
    public static async void SetEnabled(this MonoBehaviour monoBehaviour, bool enabled, int frames)
    {
        await UniTask.DelayFrame(frames);
        monoBehaviour.enabled = enabled;
    }
}
