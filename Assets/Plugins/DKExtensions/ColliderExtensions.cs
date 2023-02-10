using UnityEngine;
using Cysharp.Threading.Tasks;

public static class ColliderExtensions
{

    /// <summary>Enables Collider</summary>
    public static void SetEnabled(this Collider collider)
    {
        collider.enabled = true;
    }

    /// <summary>Disables Collider</summary>
	public static void SetDisabled(this Collider collider)
    {
        collider.enabled = false;
    }

    /// <summary>Enables or disables Collider</summary>
	public static void SetEnabled(this Collider collider, bool enabled)
    {
        collider.enabled = enabled;
    }

    /// <summary>Enables Collider</summary>
    /// <param name="delay">Time after which Collider will be enabled or disabled.</param>
    public static void SetEnabled(this Collider collider, float delay)
    {
        collider.SetEnabled(true, delay);
    }

    /// <summary>Disables Collider</summary>
    /// <param name="delay">Time after which Collider will be enabled or disabled.</param>
    public static void SetDisabled(this Collider collider, float delay)
    {
        collider.SetEnabled(false, delay);
    }

    /// <summary>Enables or disables Collider</summary>
    /// <param name="delay">Time after which Collider will be enabled or disabled.</param>
    public static async void SetEnabled(this Collider collider, bool enabled, float delay)
    {
	await UniTask.WaitForSeconds(delay);
        collider.enabled = enabled;
    }

    /// <summary>Enables Collider</summary>
    /// <param name="frames">Frames after which Collider will be enabled or disabled.</param>
    public static void SetEnabled(this Collider collider, int frames)
    {
        collider.SetEnabled(true, frames);
    }

    /// <summary>Disables Collider</summary>
    /// <param name="frames">Frames after which Collider will be enabled or disabled.</param>
    public static void SetDisabled(this Collider collider, int frames)
    {
        collider.SetEnabled(false, frames);
    }

    /// <summary>Enables or disables Collider</summary>
    /// <param name="frames">Frames after which Collider will be enabled or disabled.</param>
    public static async void SetEnabled(this Collider collider, bool enabled, int frames)
    {
        await UniTask.DelayFrame(frames);
        collider.enabled = enabled;
    }
}
