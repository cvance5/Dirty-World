using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public static class SerializableSpaceHelper
    {
        public static SerializableSpace ToSerializableSpace(Space space)
        {
            if(space is ElevatorShaft)
            {
                return new SerializableElevatorShaft(space as ElevatorShaft);
            }
            else if (space is Tunnel)
            {
                return new SerializableTunnel(space as Tunnel);
            }
            else if (space is MonsterDen)
            {
                return new SerializableMonsterDen(space as MonsterDen);
            }
            else if (space is Room)
            {
                return new SerializableRoom(space as Room);
            }
            else if (space is TreasureRoom)
            {
                return new SerializableTreasureRoom(space as TreasureRoom);
            }
            else throw new System.Exception($"Unknown space type: {space.GetType().Name}. Cannot serialize.");
        }
    }
}