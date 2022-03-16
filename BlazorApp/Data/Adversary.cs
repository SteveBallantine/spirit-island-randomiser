using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    [JsonConverter(typeof(AdversaryConverter))]
    public class Adversary : ComponentWithChildrenBase<Adversary, AdversaryLevel>
    {
        public const string NoAdversary = "No Adversary";
        public const string England = "England";
        public const string BrandenburgPrussia = "Brandenburg Prussia";
        public const string Sweden = "Sweden";
        public const string France = "France";
        public const string Habsburg = "Habsburg";
        public const string Russia = "Russia";
        public const string Scotland = "Scotland";

        public Adversary() {}

        public Adversary(
            string name,  
            OverallConfiguration config,
            Expansion expansion)
            : base (name, config, expansion)
        {
        }

        [JsonIgnore]
        /// <summary>
        /// Convenience property for accessing the AdversaryLevel child elements
        /// </summary>
        /// <value></value>
        public IEnumerable<AdversaryLevel> Levels 
        {
            get 
            {
                return this;
            }
        }
    }
}
