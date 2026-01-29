namespace TOTDistribution.Shared;

// Same as a percentage divided by 100
public readonly record struct Proportion
{
    public double Value { get; }

    public Proportion(double value)
    {
        if(value < 0 || value > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Proportion must be between 0 and 1");
        }

        Value = value;
    }
}