using WorldObjects;

namespace Data.Saving.Serialization
{
    public abstract class SerializableSpace : ISerializable<Space>
    {
        public abstract Space ToObject();
    }
}