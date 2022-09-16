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

        /// <summary>
        /// The collection that contains this item.
        /// </summary>
        /// <value></value>
        IComponentCollection ParentList { get; set; }

        OverallConfiguration Config { get; }

        /// <summary>
        /// The chance that this item will be picked out of all items in the parent's list.
        /// </summary>
        /// <value></value>
        float Weight { get; set; }

        /// <summary>
        /// The chance that has been assigned by the user for this item to be selected.
        /// </summary>
        /// <value></value>
        int? AssignedWeight { get; set; }

        /// <summary>
        /// If true then the <see cref="Weight"> can be modified by the user.
        /// </summary>
        /// <value></value>
        bool HasAssignableWeight { get; }  
        
        bool IsVisible();
    }
}
