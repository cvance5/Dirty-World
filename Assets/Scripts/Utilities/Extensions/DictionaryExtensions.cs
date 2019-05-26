using MathConcepts;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    public static Dictionary<T, U> RandomOrder<T, U>(this Dictionary<T, U> dict) => 
        dict.OrderBy(kvp => Chance.Percent)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public static void RemoveAll<T, U>(this Dictionary<T, U> dict, Func<KeyValuePair<T, U>, bool> predicate)
    {
        var newDict = new Dictionary<T, U>(dict);
        foreach (var kvp in newDict)
        {
            if (predicate(kvp))
            {
                dict.Remove(kvp.Key);
            }
        }
    }
}
