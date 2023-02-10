using UnityEngine;

public static class BoolExtensions
{
    ///<summary>
    ///Converts bool to int. true = 1, false = 0
    ///</summary>
    public static int Int(this bool b) => b ? 1 : 0;

    ///<summary>
    ///Gets random bool
    ///</summary>
    public static bool GetRandom() => Random.value > .5f;
    
    ///<summary>
    ///Gets random bool with chance. Bigger float value - bigger true chance. float must be between 0 and 1
    ///</summary>
    public static bool GetRandom(float chance) => Random.value < chance;
}
