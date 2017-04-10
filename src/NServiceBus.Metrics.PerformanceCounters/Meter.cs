namespace NServiceBus
{
    struct Meter
    {
        public readonly string Name;
        public readonly long Count;

        public Meter(string name, long count)
        {
            Name = name;
            Count = count;
        }
    }
}