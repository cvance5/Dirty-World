using System.Collections.Generic;
using UnityEngine;
using Utilities.Debug;

namespace WorldObjects.Spaces
{
    public class CustomSpaceLoader : Singleton<CustomSpaceLoader>
    {
#pragma warning disable IDE0044 // Add readonly modifier, Unity serialization requires it not be readonly
        [SerializeField]
        private List<CustomSpace> _customSpaces = null;
#pragma warning restore IDE0044 // Add readonly modifier

        public static Space Load(string spaceName)
        {
            var customSpace = Instance._customSpaces.Find(space => space.name == spaceName);

            if (customSpace != null)
            {
                return customSpace.Build();
            }
            else
            {
                _log.Error($"No custom space found by name `{spaceName}`.");
                return null;
            }
        }

        private static readonly Log _log = new Log("CustomSpaceLoader");
    }
}