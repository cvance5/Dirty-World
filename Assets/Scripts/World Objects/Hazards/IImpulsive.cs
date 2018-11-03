using UnityEngine;

namespace WorldObjects.Hazards
{
    public interface IImpulsive
    {
        Vector2 GetImpulse(Vector2 position, Vector2 velocity);
    }
}