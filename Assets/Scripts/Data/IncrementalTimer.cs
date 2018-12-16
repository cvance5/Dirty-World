namespace Data
{
    public class IncrementalTimer
    {
        private float _lastCheckTime;
        private readonly float _incrementTime;

        public IncrementalTimer(float startTime, float incrementTime)
        {
            _lastCheckTime = startTime;
            _incrementTime = incrementTime;
        }

        public bool CheckIncrement(float time) => time > _lastCheckTime + _incrementTime;

        public void AdvanceIncrement(float newTime) => _lastCheckTime = newTime;
    }
}