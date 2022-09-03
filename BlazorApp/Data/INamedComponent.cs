using System;

namespace SiRandomizer.Data
{
    public interface INamedComponent
    {
        string Name { get; }

        bool Selected { get; set; }

        /// <summary>
        /// Flag that controls whether this item can be deleted or not.
        /// By default, only homebrew items (which are added by the user) can be deleted.
        /// </summary>
        /// <value></value>
        bool Deletable { get; set; }
        
        bool IsVisible();

        /// <summary>
        /// The collection that contains this item.
        /// </summary>
        /// <value></value>
        IComponentCollection ParentList { get; set; }
    }
}
