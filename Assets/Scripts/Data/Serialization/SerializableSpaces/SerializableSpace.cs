using WorldObjects.Spaces;

namespace Data.Serialization.SerializableSpaces
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        public abstract Space ToObject();
    }
}