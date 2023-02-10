using UnityEngine;

public static class BoundingSphereExtensions
{
    public static bool Contains(this BoundingSphere sphere, Vector3 position)
    {
        return Vector3.Distance(sphere.position, position) < sphere.radius;
    }
}
