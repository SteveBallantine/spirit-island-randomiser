using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class AdversaryConverter : ComponentCollectionConverterBase<Adversary, AdversaryLevel> 
    {
        public AdversaryConverter()
        {}
        public AdversaryConverter(JsonSerializerOptions options) : base(options)
        {}
    }
}