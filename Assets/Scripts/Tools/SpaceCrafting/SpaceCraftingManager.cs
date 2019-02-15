using UnityEngine;
using Utilities.Debug;
using WorldObjects;
using WorldObjects.WorldGeneration;

namespace Tools.SpaceCrafting
{
    public class SpaceCraftingManager : Singleton<SpaceCraftingManager>
    {
        public const int CHUNK_SIZE = 16;
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("World")]
        [SerializeField]
        private int _gridSize = 5;
#pragma warning restore IDE0044 // Add readonly modifier

        private World _world;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneHelper.OnSceneIsReady += BuildWorld;
        }

        private void Start() => SceneHelper.LoadScene(SceneHelper.Scenes.World);

        private void OnValidate()
        {
            if (_world != null)
            {
                _world.ChunkActivationDepth = _gridSize;
            }
        }

        public void RebuildWorld()
        {
            _world.ChunkArchitect.Destroy();
            SceneHelper.ReloadScene();
        }

        public void BuildWorld()
        {
            _world = new GameObject("World").AddComponent<World>();

            var chunkArchitect = new ChunkArchitect();
            var spaceArchitect = new SpaceArchitect();
            _world.Initialize(chunkArchitect, spaceArchitect);

            foreach (var crafter in transform.GetComponentsInChildren<SpaceCrafter>())
            {
                if (crafter.IsValid)
                {
                    foreach (var affectedChunk in crafter.GetAffectedChunks())
                    {
                        var builder = _world.ChunkArchitect.GetBuilderAtPosition(affectedChunk);
                        builder.AddSpace(crafter.Build());
                    }
                }
                else _log.Warning($"{crafter} is not valid and won't be built.");
            }
        }

        public SpaceCrafter AddNewCrafter<T>() where T : SpaceCrafter
        {
            var crafter = new GameObject().AddComponent<T>();
            crafter.transform.SetParent(transform);
            return crafter;
        }

        private static readonly Log _log = new Log("SpaceCraftingManager");
    }
}