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
            Scrim scrim;

            if (!_scrims.TryGetValue(target, out scrim))
            {
                scrim = CreateScrim();
                scrim.transform.SetParent(target);
                scrim.transform.SetAsFirstSibling();
            }

            scrim.SetVisible(true);
            return scrim;
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
            Scrim scrim;

            if (_scrims.TryGetValue(target, out scrim))
            {
                _scrims.Remove(target);
                scrim.OnScrimDestroyed -= OnScrimDestroyed;
            }
            else
            {
                _log.Error($"Scrim for {target.gameObject.name} not found.");
            }
        }

        private static readonly Log _log = new Log("Scrimmer");
    }
}