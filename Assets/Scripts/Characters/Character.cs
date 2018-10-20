namespace Characters
{
    public class Character
    {
        public Inventory Inventory { get; set; }
        public Equipment Equipment { get; set; }

        public Metadata Metadata { get; set; }

        public Character()
        {
            Inventory = new Inventory();
            Equipment = new Equipment();
            Metadata = new Metadata();
        }
    }
}