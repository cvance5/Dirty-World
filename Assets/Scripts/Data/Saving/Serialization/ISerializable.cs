namespace Data.Saving.Serialization
{
    public interface ISerializable<T>
    {
        T ToObject();
    }
}