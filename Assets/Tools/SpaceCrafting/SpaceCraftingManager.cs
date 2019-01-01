using Data;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;
using Utilities.Editor;
using WorldObjects;
using WorldObjects.WorldGeneration;
using WorldObjects.WorldGeneration.BlockGeneration;

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

        private readonly List<SpaceCrafter> _spaceCrafters = new List<SpaceCrafter>();

        private World _world;
        private WorldBuilder _worldBuilder;
        private ChunkActivationController _activationController;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneHelper.OnSceneIsReady += BuildWorld;
            SpaceCrafter.OnCrafterDestroyed += OnCrafterDestroyed;
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

            var sPicker = new SpacePicker(0);
            var bPicker = new BlockPicker(0);

            _worldBuilder = new WorldBuilder(_world, sPicker, bPicker);
            _world.Register(_worldBuilder);

            foreach (var crafter in _spaceCrafters)
            {
                if (crafter.IsValid)
                {
                    foreach (var affectedChunk in crafter.GetAffectedChunks())
                    {
                        var blueprint = _world.GetBlueprintForPosition(affectedChunk);
                        blueprint.Register(crafter.Build());
                    }
                }
                else _log.Warning($"{crafter} is not valid and won't be built.");
            }

            ChunkGizmoDrawer.Instance.SetWorldToDraw(_world);

            _activationController = new ChunkActivationController(_world, _gridSize);
        }

        public void AddNewCrafter<T>() where T : SpaceCrafter
        {
            var crafter = new GameObject().AddComponent<T>();
            _spaceCrafters.Add(crafter);
        }

        private void OnCrafterDestroyed(SpaceCrafter crafter)
        {
            if (!_spaceCrafters.Remove(crafter))
            {
                _log.Error($"Tried to remove a crafter that the manager didn't know about.");
            }
        }

        private static readonly Log _log = new Log("SpaceCraftingManager");
    }
}