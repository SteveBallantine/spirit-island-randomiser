using System.Text.Json;

namespace SiRandomizer.Data
{
    /// <summary>
    /// Handles serialisation/deserialisation of <see cref="Spirit"> instances.
    /// </summary>
    public class SpiritConverter : ComponentCollectionConverterBase<Spirit, SpiritAspect> 
    {
        public SpiritConverter()
        {}
        public SpiritConverter(JsonSerializerOptions options) : base(options)
        {}
    }
}