
namespace SiRandomizer.Data
{
    public enum Complexity
    {
        Low = 1,
        Moderate = 2,
        High = 3,
        VeryHigh = 4,
        // Treat 'unspecified' the same as 'moderate'
        Unspecified = 2
    }

    public enum OptionChoice
    {
        Allow,
        Block,
        Force
    }

}
