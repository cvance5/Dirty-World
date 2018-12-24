using UnityEngine;

namespace Items.ItemActors
{
    public class HealthPack : ItemActor, IHealing
    {
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [SerializeField]
        private int _healing = 0;
        public int Healing => _healing;
#pragma warning restore IDE0044 // Add readonly modifier

        public override InteractionTypes[] Interactions { get; } = new InteractionTypes[] { InteractionTypes.Healing };
        public override ItemActorTypes Type { get; } = ItemActorTypes.HealthPack;

        public override string ObjectName { get; } = "Health Pack";

        public override void HandledItem() => Destroy(gameObject);
    }
}