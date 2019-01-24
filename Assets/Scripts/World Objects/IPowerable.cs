namespace WorldObjects
{
    public interface IPowerable
    {
        bool IsPowered { get; }

        void AddPower();
        void RemovePower();
    }
}