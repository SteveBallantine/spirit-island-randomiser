
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public interface IComponentCollection<T> : IEnumerable<T>, INamedComponent
        where T : INamedComponent
    {   
        void Add(T item);
    }
}