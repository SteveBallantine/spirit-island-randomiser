using System;

namespace SiRandomizer.Data
{
    public interface IComponentWithParent<T> 
        where T : INamedComponent
    {
        /// <summary>
        /// This is essentially a more specifically typed variant of the <see cref="INamedComponent.ParentList"> property.
        /// </summary>
        /// <value></value>
         T Parent { get; set; }
    }
}

