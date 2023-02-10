using UnityEngine;

public static class LayerMaskExtensions
{
    ///<summary>
    ///Returns true if LayerMask includes passed layer
    ///</summary>
    public static bool Includes(this LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) > 0;
    }

    ///<summary>
    ///Returns true if LayerMask excludes passed layer
    ///</summary>
    public static bool Excludes(this LayerMask mask, int layer)
    {
        return !mask.Includes(layer);
    }
}
