using UnityEngine;

public static class FloatExtensions
{
    /// <summary>Return 1 or -1</summary>
	/// <param name="probability">Probability of 1</param>
	public static int ProbabilitySign(float probability = 0.5f)
    {
        probability = Mathf.Clamp(probability, 0f, 1f);
        return Random.Range(0f, 1f) < probability ? 1 : -1;
    }

    ///<summary>
    ///Returns random value between 0 and float
    ///</summary>
    public static float GetRandom(this float num) => UnityEngine.Random.Range(0f, num);

    ///<summary>
    ///Returns random value between -float and float
    ///</summary>
    public static float GetRandomSign(this float num) => UnityEngine.Random.Range(-num, num);

    ///<summary>
    ///Converts float to bool. float must be between 0 and 1
    ///</summary>
    public static bool Bool(this float num) => num > .5f;

    ///<summary>
    ///Gets random bool with float as chance. Bigger float value - bigger true chance. float must be between 0 and 1
    ///</summary>
    public static bool Chance(this float num) => Random.value < num;

    public static float Percent(this float num, float percent)
    {
        return num / 100 * percent;
    }

    public static float Percent(this float num, int percent)
    {
        return num / 100 * percent;
    }
}
