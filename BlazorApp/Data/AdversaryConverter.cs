using System.Text.Json;

namespace SiRandomizer.Data
{
    /// <summary>
    /// Handles serialisation/deserialisation of <see cref="Adversary"> instances.
    /// </summary>
    public class AdversaryConverter : ComponentCollectionConverterBase<Adversary, AdversaryLevel> 
    {
        public AdversaryConverter()
        {}
        public AdversaryConverter(JsonSerializerOptions options) : base(options)
        {}
    }
}