
using System.Collections.Generic;
using System.Collections;

namespace SiRandomizer.Data
{
    public interface IComponentCollection : IEnumerable
    {        
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

        /// <summary>
        /// Calculate the weight to use for all selected items that do not have assigned weights.
        /// </summary>
        /// <returns></returns>
        public float CalculateSelectedItemsWeight();

        /// <summary>
        /// Get count of items in collection
        /// </summary>
        /// <value></value>
        public int Count();
    }

    public interface IComponentCollection<T> : IComponentCollection, IEnumerable<T>, INamedComponent
        where T : INamedComponent
    {   
        void Add(T item);
    }
}