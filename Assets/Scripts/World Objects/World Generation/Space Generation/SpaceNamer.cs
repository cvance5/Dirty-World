namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public static class SpaceNamer
    {
        private static int _counter = 0;

        public static string GetName()
        public static void Load(int next) => _counter = next;
        {
            var name = $"{_counter}";

            _counter++;

            return name;
        }
    }
}