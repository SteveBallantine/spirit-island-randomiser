using System;
using System.Collections;
using System.Linq;
using SiRandomizer.Data;

namespace SiRandomizer.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Return true if the specified type is a collection of <see cref="INamedComponent"> items.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNamedComponentCollection(this Type type)
        {
            return type.IsAssignableTo(typeof(IEnumerable)) &&
                type.GetInterfaces()
                    .Any(i => i.IsGenericType && 
                        i.IsAssignableTo(typeof(IEnumerable)) &&
                        i.GenericTypeArguments[0].IsAssignableTo(typeof(INamedComponent)));
        }
    }
    
}