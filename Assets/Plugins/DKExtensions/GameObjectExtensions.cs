using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public static class GameObjectExtensions
{
    ///<summary>
    ///Sets GameObject dirty
    ///</summary>
    public static void SetSelfDirty(this GameObject gameObject)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    ///<summary>
    ///Instantiates object as prefab
    ///</summary>
    public static GameObject InstantiatePrefab(this GameObject gameObject, Object assetComponentOrGameObject, Vector3 position, Quaternion rotation, Transform parent = null)
    {
#if UNITY_EDITOR
        var go = UnityEditor.PrefabUtility.InstantiatePrefab(assetComponentOrGameObject as GameObject) as GameObject;
        go.transform.SetPositionAndRotation(position, rotation);
        go.transform.SetParent(parent);
        return go;
#endif
        return null;
    }

    public static void SetLayer(this GameObject gameObject, int newLayer)
    {
        gameObject.layer = newLayer;
    }

    public static void SetLayer(this GameObject gameObject, string newLayer)
    {
        gameObject.layer = LayerMask.NameToLayer(newLayer);
    }

    public static void SetLayerRecursively(this GameObject gameObject, int newLayer)
    {
        gameObject.layer = newLayer;

        foreach (Transform child in gameObject.transform)
        {
            if (null == child)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static void SetLayerRecursively(this GameObject gameObject, string newLayer)
    {
        gameObject.SetLayerRecursively(LayerMask.NameToLayer(newLayer));
    }

    ///<summary>
    ///Returns list of all children
    ///</summary>
    public static List<GameObject> GetChildren(this GameObject gameObject)
    {
        List<GameObject> l = new List<GameObject>();
        foreach (Transform item in gameObject.transform)
            l.Add(item.gameObject);
        return l;
    }

    ///<summary>
    ///Sets all children of gameObject to passed value
    ///</summary>
    public static void SetChildrenActive(this GameObject go, bool active)
    {
        go.transform.SetChildrenActive(active);
    }

    ///<summary>
    ///Sets all children of gameObject active
    ///</summary>
    public static void SetChildrenActive(this GameObject go)
    {
        go.transform.SetChildrenActive(true);
    }

    ///<summary>
    ///Sets all children of gameObject inactive
    ///</summary>
    public static void SetChildrenInctive(this GameObject go)
    {
        go.transform.SetChildrenActive(false);
    }

    ///<summary>
    ///Like DestroyImmediate, but called for all childeren of the gameObject
    ///</summary>
    public static void DestroyAllChildImmediate(this GameObject go)
    {
        go.transform.DestroyAllChildImmediate();
    }

    /// <summary>
    /// Gets a component attached to the given game object.
    /// If one isn't found, a new one is attached and returned.
    /// </summary>
    /// <param name="gameObject">Game object.</param>
    /// <returns>Previously or newly attached component.</returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Checks whether a game object has a component of type T attached.
    /// </summary>
    /// <param name="gameObject">Game object.</param>
    /// <returns>True when component is attached.</returns>
    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() != null;
    }

    /// <summary>Inactivates gameObject</summary>
	public static void SetInactive(this GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    /// <summary>Activates gameObject</summary>
    public static void SetActive(this GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    /// <summary>Inactivates gameObject</summary>
    /// <param name="delay">Time after which gameObject will be Inactivated.</param>
	public static async void SetInactive(this GameObject gameObject, float delay)
    {
	await UniTask.WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    /// <summary>Activates gameObject</summary>
    /// <param name="delay">Time after which gameObject will be Activated.</param>
    public static async void SetActive(this GameObject gameObject, float delay)
    {
	await UniTask.WaitForSeconds(delay);
        gameObject.SetActive(true);
    }

    /// <summary>Inactivates gameObject</summary>
    /// <param name="frames">Frames after which gameObject will be Deactivated.</param>
	public static async void SetInactive(this GameObject gameObject, int frames)
    {
        await UniTask.DelayFrame(frames);
        gameObject.SetActive(false);
    }

    /// <summary>Activates gameObject</summary>
    /// <param name="frames">Frames after which gameObject will be Activated.</param>
    public static async void SetActive(this GameObject gameObject, int frames)
    {
        await UniTask.DelayFrame(frames);
        gameObject.SetActive(true);
    }
}
