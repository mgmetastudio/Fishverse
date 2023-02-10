using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    ///<summary>
    ///Returns closest point to transform
    ///</summary>
    public static Transform GetClosestTransform(this Vector3 position, IEnumerable<Transform> points)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        foreach (Transform t in points)
        {
            float dist = Vector3.Distance(t.position, position);
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
    public static T GetClosestTransform<T>(this Vector3 position, IEnumerable<T> points) where T : Component
    {
        T tMin = null;
        float minDist = Mathf.Infinity;
        foreach (T t in points)
        {
            float dist = Vector3.Distance(t.transform.position, position);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

    /// <summary>
    ///Returns a random point on the surface of a circle with radius 1.0
    ///</summary>
    public static Vector3 RandomDirNoY(this Vector3 vector3)
    {
        return new Vector3(Random.value, 0, Random.value).normalized;
    }

    /// <summary>Get direction from 2 positions</summary>
    public static Vector3 Direction(this Vector3 target, Vector3 pos)
    {
        var head = target - pos;
        var dist = head.magnitude;
        return head / dist;
    }

    /// <summary>
    /// rounds X and Z values
    /// with specification of digits after point
    /// </summary>
    public static Vector3 Round(this Vector3 vector3, int digits = 0)
    {
        return vector3.Round(Vector3Int.one, digits);
    }

    /// <summary>
    /// direction value of 0 rounds X, Y and Z values
    /// with specification of digits after point
    /// </summary>
    public static Vector3 Round(this Vector3 vector3, Vector3Int dir, int digits = 0)
    {
        return new Vector3
        (
            dir.x == 0 ? (float)System.Math.Round(vector3.x, digits) : vector3.x,
            dir.y == 0 ? (float)System.Math.Round(vector3.y, digits) : vector3.y,
            dir.z == 0 ? (float)System.Math.Round(vector3.z, digits) : vector3.z
        );
    }

    public static Vector3 Ceiling(this Vector3 vector3)
    {
        return new Vector3
        (
            vector3.x > 0 ? (float)System.Math.Ceiling(vector3.x) : (float)System.Math.Floor(vector3.x),
            vector3.y > 0 ? (float)System.Math.Ceiling(vector3.y) : (float)System.Math.Floor(vector3.y),
            vector3.z > 0 ? (float)System.Math.Ceiling(vector3.z) : (float)System.Math.Floor(vector3.z)
        );
    }

    ///<summary>
    ///Converts Vector3 to Vector2 where x = v3.x, y = v3.z
    ///</summary>
    public static Vector2 VectorZtoY(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    ///<summary>
    ///Converts Vector3 to Vector3 where x = v3.x, y = 0, z = v3.y
    ///</summary>
    public static Vector3 VectorYtoZ(this Vector3 vector3)
    {
        return new Vector3(vector3.x, 0, vector3.y);
    }

    /// <summary>Return absolute vector</summary>
	public static Vector3 Abs(this Vector3 vector)
    {
        var absVector = new Vector3();
        absVector.x = Mathf.Abs(vector.x);
        absVector.y = Mathf.Abs(vector.y);
        absVector.z = Mathf.Abs(vector.z);
        return absVector;
    }

    /// <summary>Floors to int</summary>
    public static Vector3Int FloorToInt(this Vector3 vector)
    {
        var v3i = new Vector3Int();
        v3i.x = Mathf.FloorToInt(vector.x);
        v3i.y = Mathf.FloorToInt(vector.y);
        v3i.z = Mathf.FloorToInt(vector.z);
        return v3i;
    }

    /// <summary>Ceil to int</summary>
    public static Vector3Int CeilToInt(this Vector3 vector)
    {
        var v3i = new Vector3Int();
        v3i.x = Mathf.CeilToInt(vector.x);
        v3i.y = Mathf.CeilToInt(vector.y);
        v3i.z = Mathf.CeilToInt(vector.z);
        return v3i;
    }

    /// <summary>Rounds to int</summary>
    public static Vector3Int RoundToInt(this Vector3 vector)
    {
        var v3i = new Vector3Int();
        v3i.x = Mathf.RoundToInt(vector.x);
        v3i.y = Mathf.RoundToInt(vector.y);
        v3i.z = Mathf.RoundToInt(vector.z);
        return v3i;
    }

    /// <summary>Returns same vector with changed y value</summary>
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    /// <summary>Returns same vector with changed y value</summary>
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    /// <summary>Returns same vector with changed y value</summary>
    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
}
