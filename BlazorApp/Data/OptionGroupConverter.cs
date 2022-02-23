using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public class OptionGroupConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Adversary))
            {
                return true;
            }
            else if (!typeToConvert.IsGenericType)
            {
                return false;
            }
            return typeToConvert.GetGenericTypeDefinition() == typeof(OptionGroup<>);
        }

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