using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    /// <summary>
    /// Handles serialisation/deserialisation of <see cref="OptionGroup{T}"> instances.
    /// </summary>
    public class OptionGroupConverter : JsonConverterFactory
    {
        /// <summary>
        /// This converter is used for <see cref="OptionGroup{T}"> instances.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }
            return typeToConvert.GetGenericTypeDefinition() == typeof(OptionGroup<>);
        }

        /// <summary>
        /// Create the convertor for the specific generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            Type itemType = type.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(OptionGroupConverterInner<>).MakeGenericType(
                    new Type[] { itemType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        /// <summary>
        /// We use the <see cref="ComponentCollectionConverterBase{TCollection, TItem}"> base
        /// class, which is also used for the <see cref="AdversaryConverter">.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class OptionGroupConverterInner<T> : ComponentCollectionConverterBase<OptionGroup<T>, T> 
            where T : SelectableComponentBase<T>
        {
            public OptionGroupConverterInner() : base()
            {}
            public OptionGroupConverterInner(JsonSerializerOptions options) : base(options)
            {}
        }
    }

}