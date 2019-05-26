namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public static class SpaceNamer
    {
        private static int _counter = 0;

        public static string GetName()
        {
            var name = $"{_counter}";

            _counter++;

            return name;
        }
    }
}