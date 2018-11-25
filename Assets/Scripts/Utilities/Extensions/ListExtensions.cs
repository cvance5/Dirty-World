using System.Collections.Generic;
using System.Linq;

public static class List
{
    public static T RandomItem<T>(this List<T> list)
    {
        var randomValue = UnityEngine.Random.Range(0, list.Count);
        return list[randomValue];
    }

    public static T RandomItem<T>(this List<T> list, params T[] excludedItems)
    {
        var newList = new List<T>();

        foreach (var item in list)
        {
            if (!excludedItems.Contains(item))
            {
                newList.Add(item);
            }
        }

        return newList.RandomItem();
    }

    public static void RemoveAtRandom<T>(this List<T> list)
    {
        var randomValue = UnityEngine.Random.Range(0, list.Count);
        list.RemoveAt(randomValue);
    }

    public static T LoopedNext<T>(this List<T> list, int index)
    {
        if (list == null || list.Count == 0 || index > list.Count || index < 0)
        {
            throw new System.IndexOutOfRangeException();
        }
        else if (index == list.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        return list[index];
    }

    public static T LoopedNext<T>(this T[] array, int index)
    {
        if (array == null || array.Length == 0 || index > array.Length || index < 0)
        {
            throw new System.IndexOutOfRangeException();
        }
        else if (index == array.Length - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        return array[index];
    }
    public static T LoopedPrevious<T>(this List<T> list, int index)
    {
        if (list == null || list.Count == 0 || index > list.Count || index < 0)
        {
            throw new System.IndexOutOfRangeException();
        }
        else if (index == 0)
        {
            index = list.Count - 1;
        }
        else
        {
            index--;
        }

        return list[index];
    }
    public static T LoopedPrevious<T>(this T[] array, int index)
    {
        if (array == null || array.Length == 0 || index > array.Length || index < 0)
        {
            throw new System.IndexOutOfRangeException();
        }
        else if (index == 0)
        {
            index = array.Length - 1;
        }
        else
        {
            index--;
        }

        return array[index];
    }
}
