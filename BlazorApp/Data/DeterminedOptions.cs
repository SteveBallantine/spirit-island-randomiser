
namespace SiRandomizer.Data
{
    public class DeterminedOptions
    {
        public int AdditionalBoards { get; set; }

        public Map Map { get; set; }
        public Scenario Scenario { get; set; }
        public Adversary Adversary { get; set; }
        public Adversary SecondaryAdversary { get; set; }
    }
}