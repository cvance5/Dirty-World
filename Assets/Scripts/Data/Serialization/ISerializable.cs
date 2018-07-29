namespace Data.Serialization
{
    public interface ISerializable<T>
    {
        T ToObject();
    }
}