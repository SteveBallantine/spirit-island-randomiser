using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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