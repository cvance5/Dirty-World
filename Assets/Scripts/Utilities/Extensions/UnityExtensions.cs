using System.Collections.Generic;
using UnityEngine;

public static class UnityExtensions
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

    public static Vector3 SetAxis(this Vector3 source, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.X:
                return new Vector3(value, source.y, source.z);
            case Axis.Y:
                return new Vector3(source.x, value, source.z);
            case Axis.Z:
                return new Vector3(source.x, source.y, value);
            default:
                throw new System.ArgumentOutOfRangeException("Value is not an access.");
        }
    }

    public static void SetActive(this Component component, bool isActive)
    {
        component.gameObject.SetActive(isActive);
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

    public static Vector3 RandomPointWithinBounds(this MeshFilter meshFilter)
    {
        Bounds bounds = meshFilter.mesh.bounds;

        float minX = meshFilter.transform.localScale.x * bounds.size.x * 0.5f;
        float minY = meshFilter.transform.localScale.y * bounds.size.y * 0.5f;
        float minZ = meshFilter.transform.localScale.z * bounds.size.z * 0.5f;

        Vector3 randomPoint = new Vector3(
            Random.Range(minX, -minX),
            Random.Range(minY, -minY),
            Random.Range(minZ, -minZ)
            );

        return randomPoint;
    }
    public static Vector3 RandomPointWithinBounds(this Collider collider)
    {
        Bounds bounds = collider.bounds;

        float minX = collider.transform.localScale.x * bounds.size.x * 0.5f;
        float minY = collider.transform.localScale.y * bounds.size.y * 0.5f;
        float minZ = collider.transform.localScale.z * bounds.size.z * 0.5f;

        Vector3 randomPoint = new Vector3(
            Random.Range(minX, -minX),
            Random.Range(minY, -minY),
            Random.Range(minZ, -minZ)
            );

        return randomPoint;
    }

    public enum Axis
    {
        X,
        Y,
        Z
    }
}
