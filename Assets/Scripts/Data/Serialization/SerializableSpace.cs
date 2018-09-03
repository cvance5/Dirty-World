using WorldObjects.Spaces;

namespace Data.Serialization
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        public abstract Space ToObject();
    }
}