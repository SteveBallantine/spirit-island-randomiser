using System;

namespace SiRandomizer.Data
{
    public interface INamedComponent
    {
        string Name { get; }
        bool Selected { get; set; }

        bool Deletable { get; set; }
        
        bool IsVisible();
    }
}
