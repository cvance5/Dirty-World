using Data;
using UnityEngine;
using Utilities.Debug;
using WorldObjects;
using WorldObjects.WorldGeneration;

namespace Tools.SpaceCrafting
{
    public class SpaceCraftingManager : Singleton<SpaceCraftingManager>
    {
        public static int ChunkSize => Instance._settings.ChunkSize;
#pragma warning disable IDE0044 // Add readonly modifier, cannot be readonly since we want it serialized by unity
        [Header("World")]
        [SerializeField]
        private int _gridSize = 5;
        [SerializeField]
        private Settings _settings = null;
#pragma warning restore IDE0044 // Add readonly modifier

        private World _world;
        private WorldBuilder _worldBuilder;
        private ChunkActivationController _activationController;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneHelper.OnSceneIsReady += BuildWorld;
        }

        private void Start() => SceneHelper.LoadScene(SceneHelper.Scenes.World);

        public void RebuildWorld()
        {
            _worldBuilder.Destroy();

            SceneHelper.ReloadScene();
        }

        public void BuildWorld()
        {
            _world = new GameObject("World").AddComponent<World>();
            _world.Initialize(0, _settings.ChunkSize);

            _worldBuilder = new WorldBuilder(_world);
            _world.Register(_worldBuilder);

            foreach (var crafter in transform.GetComponentsInChildren<SpaceCrafter>())
            {
                if (crafter.IsValid)
                {
                    foreach (var affectedChunk in crafter.GetAffectedChunks())
                    {
                        var builder = _worldBuilder.GetBuilderAtPosition(affectedChunk);
                        builder.AddSpace(crafter.Build());
                    }
                }
                else _log.Warning($"{crafter} is not valid and won't be built.");
            }

            _activationController = new ChunkActivationController(_world, _gridSize);
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