namespace NServiceBus
{
    struct Timer
    {
        public readonly string Name;
        public readonly long TotalTime;

        public Timer(string name, long totalTime)
        {
            Name = name;
            TotalTime = totalTime;
        }
    }
}