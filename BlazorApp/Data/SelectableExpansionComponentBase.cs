using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{    
    // Adds default visibility logic, which is based on the currently selected expansions.
    public abstract class SelectableExpansionComponentBase<T> : SelectableComponentBase<T>, IExpansionContent
        where T : SelectableComponentBase<T>
    {
        [JsonIgnore]
        public Expansion Expansion { get; set; }

        public SelectableExpansionComponentBase() {}

        public SelectableExpansionComponentBase(
            string name,
            Expansion expansion) 
            : base(name)
        {
            Expansion = expansion;
        }

        public override bool IsVisible(OverallConfiguration config)
        {
            if(config == null) 
            {
                throw new ArgumentNullException(nameof(config));
            }
            var visible = Expansion == null || config.Expansions[Expansion.Name].Selected;
            // If this component is not visible then also ensure it is not selected.
            if(visible == false)
            {
                Selected = false;
            }
            return visible;
        }
    }
}