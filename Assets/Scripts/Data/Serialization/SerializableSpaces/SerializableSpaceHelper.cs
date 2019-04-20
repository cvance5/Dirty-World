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
            else if (space is Shaft)
            {
                return new SerializableShaft(space as Shaft);
            }
            else if (space is Corridor)
            {
                return new SerializableCorridor(space as Corridor);
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
            else if (space is Laboratory)
            {
                return new SerializableLaboratory(space as Laboratory);
            }
            else throw new System.Exception($"Unknown space type: {space.GetType().Name}. Cannot serialize.");
        }
    }
}