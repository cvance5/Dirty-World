using UnityEngine;
using WorldObjects.Blocks;

namespace Tools.SpaceCrafting
{
    public class BlockOverrideCrafter : MonoBehaviour
    {
        public BlockTypes Type;
        public IntVector2 Position => new IntVector2(transform.position);

        private void Awake() => OnValidate();

        private void OnValidate() => transform.position = new IntVector2(transform.position);
    }
}