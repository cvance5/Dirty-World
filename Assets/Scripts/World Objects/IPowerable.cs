namespace WorldObjects
{
    public interface IPowerable
    {
        bool CanBePowered { get; }
        bool HasPower { get; }

        void AddPower();
        void RemovePower();
    }
}