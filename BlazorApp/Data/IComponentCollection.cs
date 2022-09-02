
using System.Collections.Generic;

namespace SiRandomizer.Data
{
    public interface IComponentCollection<T> : IEnumerable<T>, INamedComponent
        where T : INamedComponent
    {   
        void Add(T item);
        
        /// <summary>
        /// Remove the specified item from the list of children if it exists
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name);

        /// <summary>
        /// Return true if there is a child item with the specified name. False if not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasChild(string name);
    }
}