using System;

namespace SiRandomizer.Data
{
    public interface INamedComponent
    {
        string Name {get;}
        bool Selected { get; set; }
        bool Hide { get; }
        bool Disabled { get; set; }
    }
}
