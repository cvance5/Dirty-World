using WorldObjects;

namespace Data.Serialization
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        public abstract Space ToObject();
    }
}