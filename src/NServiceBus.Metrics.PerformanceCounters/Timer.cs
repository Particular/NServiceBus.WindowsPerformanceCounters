struct Timer
{
    public Histogram Histogram { get; set; }
    public string Name { get; set; }
}

struct Histogram
{
    public long LastValue { get; set; }
}