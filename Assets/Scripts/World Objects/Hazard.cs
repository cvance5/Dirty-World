using UnityEngine;
using WorldObjects;

[RequireComponent(typeof(Collider2D))]
public abstract class Hazard : MonoBehaviour
{
    public SmartEvent<Hazard> OnHazardDestroyed = new SmartEvent<Hazard>();

    public IntVector2 Position => new IntVector2(transform.position);
    public abstract IntVector2 AnchoringPosition { get; }
    public HazardEffects[] Effects { get; protected set; }

    private void Awake() => InitializeEffects();

    protected abstract void InitializeEffects();

    public abstract void SetAnchor(Block anchorBlock);
}
