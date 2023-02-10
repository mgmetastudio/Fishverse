using Random = UnityEngine.Random;

public static class IntExtensions
{
    /// <summary>Return Random value without number</summary>
    /// <param name="min">Min value (inclusive)</param>
    /// <param name="max">Max value (exclusive)</param>
    /// <param name="exclusion">Exclusion</param>
    /// <example>
    /// <code>
    /// int i = Extensions.RandomRangeWithout(0, 4, 2);
    /// </code>
    /// Returns 0, 1 or 3
    /// </example>
    public static int RandomRangeWithout(int min, int max, int exclusion)
    {
        if (exclusion < min || exclusion > max)
            return Random.Range(min, max);

        var random = Random.Range(min, max - 1);

        if (random >= exclusion)
            random++;

        return random;
    }

    ///<summary>
    ///Returns random value between 0 and int
    ///</summary>
    public static int GetRandom(this int num) => UnityEngine.Random.Range(0, num + 1);

    ///<summary>
    ///Returns random value between -int and int
    ///</summary>
    public static int GetRandomSign(this int num) => UnityEngine.Random.Range(-num, num + 1);

    ///<summary>
    ///Converts int to bool. int must be between 0 or 1
    ///</summary>
    public static bool Bool(this int num) => num == 1;

    public static int Percent(this int num, float percent)
    {
        return (int)((float)num / 100f * percent);
    }

    public static int Percent(this int num, int percent)
    {
        return (int)((float)num / 100f * percent);
    }

    public static string KiloFormat(this int num)
    {
        if (num >= 100000000)
            return (num / 1000000).ToString("#,0M");

        if (num >= 10000000)
            return (num / 1000000).ToString("0.#") + "M";

        if (num >= 100000)
            return (num / 1000).ToString("#,0K");

        if (num >= 10000)
            return (num / 1000).ToString("0.#") + "K";

        return num.ToString("#,0");
    }
}
