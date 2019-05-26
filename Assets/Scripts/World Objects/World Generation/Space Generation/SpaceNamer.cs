namespace WorldObjects.WorldGeneration.SpaceGeneration
{
    public static class SpaceNamer
    {
        private static int _counter = 0;

        public static string GetName()
        {
            var name = $"{System.DateTime.Now.Ticks.ToString()}_{_counter}";

            _counter++;

            return name;
        }
    }
}