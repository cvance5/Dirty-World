using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class Scrimmer
    {
        private static readonly Dictionary<Transform, Scrim> _scrims
                          = new Dictionary<Transform, Scrim>();

        public static Scrim ScrimOver(Transform target)
        {
            if (!_scrims.TryGetValue(target, out var scrim))
            {
                scrim = CreateScrim();
                scrim.transform.SetParent(target);
                scrim.transform.SetAsFirstSibling();

                _scrims.Add(target, scrim);
            }
            scrim.Initialize();
            scrim.SetVisible(true);
            return scrim;
        }

        public static Scrim GetScrim(Transform target)
        {
            _scrims.TryGetValue(target, out var scrim);
            return scrim;
        }

        public static void ClearScrim(Transform target)
        {
            if (_scrims.TryGetValue(target, out var scrim))
            {
                Object.Destroy(scrim.gameObject);
            }
        }

        private static Scrim CreateScrim()
        {
            var gameObject = new GameObject("Scrim");
            var scrim = gameObject.AddComponent<Scrim>();
            scrim.OnScrimDestroyed += OnScrimDestroyed;
            return scrim;
        }

        private static void OnScrimDestroyed(Transform target)
        {
            if (_scrims.TryGetValue(target, out var scrim))
            {
                _scrims.Remove(target);
                scrim.OnScrimDestroyed -= OnScrimDestroyed;
                _log.Info($"Scrim removed from {target.gameObject.name}.");
            }
            else
            {
                _log.Error($"Scrim for {target.gameObject.name} not found.");
            }
        }

        private static readonly Utilities.Debug.Log _log = new Utilities.Debug.Log("Scrimmer");
    }
}