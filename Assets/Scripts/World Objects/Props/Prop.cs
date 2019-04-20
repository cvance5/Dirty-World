using Utilities.Debug;
using WorldObjects.Blocks;
using WorldObjects.Spaces;
using WorldObjects.WorldGeneration.PropGeneration;

namespace WorldObjects.Props
{
    public abstract class Prop : WorldObject
    {
        public SmartEvent<Prop> OnPropDestroyed = new SmartEvent<Prop>();

        public abstract PropTypes Type { get; }

        protected Block _attachedBlock;
        protected Space _containingSpace;

        public virtual void Initialize() { }

        public void Assign(Block block)
        {
            _attachedBlock = block;
            _attachedBlock.OnBlockCrumbled += HandleAttachedBlockRemoval;
            _attachedBlock.OnBlockDestroyed += HandleAttachedBlockRemoval;
        }

        public void Assign(Space space)
        {
            _containingSpace = space;
        }

        private void HandleAttachedBlockRemoval(Block attachedBlock)
        {
            if (attachedBlock != _attachedBlock)
            {
                _log.Error($"Prop received attached block removal from `{attachedBlock}` but is actually attached to `{_attachedBlock}`!");
            }
            else
            {
                _attachedBlock.OnBlockCrumbled -= HandleAttachedBlockRemoval;
                _attachedBlock.OnBlockDestroyed -= HandleAttachedBlockRemoval;

                Destroy(gameObject);
            }
        }

        private static readonly Log _log = new Log("Prop");
    }
}