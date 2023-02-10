using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    ///<summary>
    ///Returns list of all siblings
    ///</summary>
    public static IEnumerable<Transform> GetSiblings(this Transform t)
    {
        List<Transform> l = new List<Transform>();
        l = t.parent.GetChildren();
        l.Remove(t);
        return l;
    }

    ///<summary>
    ///Counts all siblings
    ///</summary>
    public static int SiblingCount(this Transform transform)
    {
        return transform.parent.childCount;
    }

    ///<summary>
    ///Returns closest point to transform
    ///</summary>
    public static Transform GetClosestTransform(this Transform transform, IEnumerable<Transform> points)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in points)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    ///<summary>
    ///Returns closest point to transform as Component
    ///</summary>
    public static T GetClosestTransform<T>(this Transform transform, IEnumerable<T> points) where T : Component
    {
        T tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (T t in points)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    /// <summary>Sets parent of transform</summary>
	public static void SetToParent(this Transform transform, Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    ///<summary>
    ///Sets all children of transform to passed value
    ///</summary>
    public static void SetChildrenActive(this Transform t, bool active)
    {
        foreach (Transform item in t)
            item.gameObject.SetActive(active);
    }

    ///<summary>
    ///Sets all children of transform active
    ///</summary>
    public static void SetChildrenActive(this Transform t)
    {
        t.SetChildrenActive(true);
    }

    ///<summary>
    ///Sets all children of transform inactive
    ///</summary>
    public static void SetChildrenInctive(this Transform t)
    {
        t.SetChildrenActive(false);
    }

    ///<summary>
    ///Gets random child of the transform
    ///</summary>
    public static Transform GetRandom(this Transform t)
    {
        return t.GetChildren().GetRandom();
    }

    ///<summary>
    ///Gets child by index with and checks if it exists
    ///</summary>
    public static bool GetChild(this Transform t, int index, out Transform outT)
    {
        if (index >= t.childCount || index < 0)
        {
            outT = null;
            return false;
        }

        outT = t.GetChild(index);
        return true;
    }

    ///<summary>
    ///Returns list of all children
    ///</summary>
    public static List<Transform> GetChildren(this Transform t)
    {
        List<Transform> l = new List<Transform>();
        foreach (Transform item in t)
            l.Add(item);
        return l;
    }

    /// <summary>Destroys all children of transform</summary>
	public static void DestroyAllChild(this Transform transform)
    {
        foreach (Transform child in transform)
            UnityEngine.Object.Destroy(child.gameObject);
    }

    ///<summary>
    ///Like DestroyImmediate, but called for all childeren of the transform
    ///</summary>
    public static void DestroyAllChildImmediate(this Transform t)
    {
        foreach (Transform item in t)
            MonoBehaviour.DestroyImmediate(item.gameObject);
    }

    ///<summary>
    ///Rotates the transform so y axis points at worldPosition
    ///</summary>
    public static void LookAtY(this Transform t, Vector3 worldPosition)
    {
        Vector3 rot = t.localEulerAngles;
        t.LookAt(worldPosition);
        t.localEulerAngles = new Vector3(rot.x, t.localEulerAngles.y, rot.z);
    }

    ///<summary>
    ///Rotates the transform so y axis points at worldPosition
    ///</summary>
    public static void LookAtY(this Transform t, Transform target)
    {
        t.LookAt(target.position);
        t.localEulerAngles = new Vector3(0, t.localEulerAngles.y, 0);
    }

    public static void LookAtX(this Transform t, Transform target)
    {
        t.LookAt(target.position);
        t.localEulerAngles = new Vector3(t.localEulerAngles.x, 0, 0);
    }

    /// <summary>
    /// Sets the layer of the transform's children.
    /// </summary>
    /// <param name="transform">Parent transform.</param>
    /// <param name="layerName">Name of layer.</param>
    /// <param name="recursive">Also set ancestor layers?</param>
    public static void SetChildLayers(this Transform transform, string layerName, bool recursive = false)
    {
        var layer = LayerMask.NameToLayer(layerName);
        SetChildLayersHelper(transform, layer, recursive);
    }

    static void SetChildLayersHelper(Transform transform, int layer, bool recursive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layer;

            if (recursive)
            {
                SetChildLayersHelper(child, layer, recursive);
            }
        }
    }
	
    public static Transform FindRecursiveContains(this Transform findIn, string nameToFind)
    {
        if (findIn.name.Contains(nameToFind))
            return findIn;

        foreach (Transform child in findIn)
        {
            var res = FindRecursiveContains(child, nameToFind);
            if (res) return res;
        }

        return null;
    }
}
