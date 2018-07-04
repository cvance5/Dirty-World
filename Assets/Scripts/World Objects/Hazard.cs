using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Hazard : MonoBehaviour
{
    public HazardEffects[] Effects { get; protected set; }

    private void Awake() => InitializeEffects();

    protected abstract void InitializeEffects();
}
