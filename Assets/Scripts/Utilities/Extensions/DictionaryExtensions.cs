using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
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
