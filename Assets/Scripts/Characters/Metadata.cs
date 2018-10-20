namespace Characters
{
    public class Metadata
    {
        public double TimePlayed { get; private set; }

        public void AddTimePlayed(double seconds)
        {
            TimePlayed += seconds;
        }
    }
}