using System.Collections.Generic;

namespace WorldObjects.Spaces
{
    public abstract class SpaceModifier : Space
    {
        public Space ModifiedSpace { get; private set; }
        public override List<ModifierTypes> Modifiers
        {
            get
            {
                var modifiers = base.Modifiers;
                modifiers.Add(_type);
                return modifiers;
            }
        }

        protected abstract ModifierTypes _type { get; }

        public SpaceModifier(Space modifiedSpace)
        {
            ModifiedSpace = modifiedSpace;
            ModifiedSpace.AddModifier(_type);
            Extents = modifiedSpace.Extents;
        }

        public override bool Contains(IntVector2 position) => ModifiedSpace.Contains(position);
    }
}