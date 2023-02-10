using System.Linq;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public static partial class Extensions
{
    /// <summary>Returns random element of the List</summary>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default(T);

        return list[Random.Range(0, list.Count)];
    }

    /// <summary>Returns random element of the List</summary>
    public static T GetRandom<T>(this List<T> list, out int index)
    {
        index = -1;
        if (list.Count == 0)
            return default(T);

        index = Random.Range(0, list.Count);
        return list[index];
    }

    /// <summary>Returns random element of the List</summary>
    public static T GetRandomWithout<T>(this List<T> list, int without)
    {
        if (list.Count == 0)
            return default(T);

        return list[GetRandomIndexWithout(list, without)];
    }

    /// <summary>Returns random element of the List</summary>
    public static T GetRandomWithout<T>(this List<T> list, int without, out int index)
    {
        index = -1;
        if (list.Count == 0)
            return default(T);

        index = GetRandomIndexWithout(list, without);
        return list[index];
    }

    /// <summary>Returns random element of the List</summary>
    /// <param name="index">Start index</param>
    public static T GetRandom<T>(this List<T> list, int index)
    {
        if (list.Count == 0)
            return default(T);

        if (list.Count <= index)
            return list[0];

        return list[Random.Range(index, list.Count)];
    }

    /// <summary>Populates Array</summary>
    /// <param name="value">Value to fill all elements</param>
    /// <example>
    /// <code>
    /// var array = new int[10];
    /// array.Populate(5);
    /// </code>
    /// </example>
    public static T[] Populate<T>(this T[] arr, T value)
    {
        for (var i = 0; i < arr.Length; i++)
            arr[i] = value;

        return arr;
    }

    /// <summary>Populates List</summary>
    /// <param name="value">Value to fill all elements</param>
    /// <example>
    /// <code>
    /// var list = new List<int>(10);
    /// list.Populate(5);
    /// </code>
    /// </example>
    public static List<T> Populate<T>(this List<T> list, T value)
    {
        for (var i = 0; i < list.Count; i++)
            list[i] = value;

        return list;
    }

    /// <summary>Returns list in readable form</summary>
    public static string Print<T>(this IList<T> list)
    {
        var res = "[ " + string.Join(", ", list.Select(s => $"{s}\n")) + " ]";
        return res;
    }

    /// <summary>Shuffles all alemnts in the List</summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();

        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>Returns random index of the List</summary>
    public static int GetRandomIndex<T>(this List<T> list)
    {
        if (list.Count == 0)
            return -1;

        return Random.Range(0, list.Count);
    }

    /// <summary>Returns random index of the List with exclusion</summary>
    /// <param name="exclusion">exclusion index</param>
    public static int GetRandomIndexWithout<T>(this List<T> list, int exclusion = -1)
    {
        if (list.Count == 0)
            return -1;

        return IntExtensions.RandomRangeWithout(0, list.Count, exclusion);
    }
}
