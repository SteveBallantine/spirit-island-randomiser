namespace SiRandomizer.Data
{
    public class BoardSetup
    {
        public Board Board { get; set; }
        public SpiritAspect SpiritAspect { get; set; }

        public string SpiritDisplayName 
        {
            get 
            {
                if(SpiritAspect == null) return "No Spirit";
                return SpiritAspect.ToString();
            }
        }
    }
}
