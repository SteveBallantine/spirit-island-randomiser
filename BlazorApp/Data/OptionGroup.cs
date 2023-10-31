using System.Text.Json.Serialization;
namespace SiRandomizer.Data
{
    [JsonConverter(typeof(OptionGroupConverter))]
    public class OptionGroup<T> : ComponentWithChildrenBase<OptionGroup<T>, T>
        where T : SelectableComponentBase<T>
    {
        public const string ADVERSARIES = "Adversaries";
        public const string BOARDS = "Boards";
        public const string MAPS = "Maps";
        public const string SCENARIOS = "Scenarios";
        public const string SPIRITS = "Spirits";
        public const string EXPANSIONS = "Expansions";

        public OptionGroup() {}

        public OptionGroup(
            string name, 
            OverallConfiguration config,
            IComponentCollection parentList,
            Expansion[] expansions) 
            : base(name, config, parentList, expansions) 
        {}
    }
}
