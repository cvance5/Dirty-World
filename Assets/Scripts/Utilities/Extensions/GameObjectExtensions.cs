using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void Reset(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void LocalReset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void Mirror(this Transform source, Transform target)
    {
        source.position = target.position;
        source.rotation = target.rotation;
        source.localScale = target.localScale;
    }

    public static void SnapToGrid(this Transform source)
    {
        source.position = new Vector3()
        {
            x = Mathf.RoundToInt(source.position.x),
            y = Mathf.RoundToInt(source.position.y),
            z = Mathf.RoundToInt(source.position.z)
        };
    }

    public static void SetActive(this Component component, bool isActive)
    {
        component.gameObject.SetActive(isActive);
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    public static List<Transform> GetAllChildren(this Transform parent)
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            children.Add(child);
            children.AddRange(child.GetAllChildren());
        }

        return children;
    }

    public static void DestroyChildren(this Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }
}