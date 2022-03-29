using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class Presets
    {
        public string Current { get; set; }
        public List<string> Available { get; set; }
    }
}