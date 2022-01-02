
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public interface IComponentCollection<T> : IEnumerable<T>
        where T : INamedComponent
    {   
    }
}