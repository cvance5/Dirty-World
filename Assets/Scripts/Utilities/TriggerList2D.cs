using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class TriggerList2D : MonoBehaviour
{
    protected readonly List<GameObject> _overlaps = new List<GameObject>();
    public List<GameObject> Overlaps => new List<GameObject>(_overlaps);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_overlaps.Contains(collision.gameObject))
        {
            _overlaps.Add(collision.gameObject);
        }
        UpdateOverlaps();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _overlaps.Remove(collision.gameObject);
        UpdateOverlaps();
    }

    protected virtual void UpdateOverlaps() => _overlaps.RemoveAll(overlap => overlap == null);
}