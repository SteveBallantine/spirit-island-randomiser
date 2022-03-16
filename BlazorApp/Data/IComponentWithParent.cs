using System;

namespace SiRandomizer.Data
{
    public interface IComponentWithParent<T> 
        where T : INamedComponent
    {
         T Parent { get; set; }
    }
}

