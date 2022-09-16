using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    /// <summary>
    /// Contains the shared logic for serialisation/deserialisation of 
    /// <see cref="OptionGroup{T}"> and <see cref="Adversary"> instances.
    /// This is needed because the built-in Json serialisation cannot cope with 
    /// classes that implement <see cref="IEnumerable{T}">.
    /// </summary>
    public abstract class ComponentCollectionConverterBase<TCollection, TItem> : JsonConverter<TCollection> 
        where TItem : SelectableComponentBase<TItem>
        where TCollection : IComponentCollection<TItem>, new()
    {
        /// <summary>
        /// The converter to use for items that are stored in the collection to be converted.
        /// </summary>
        private readonly JsonConverter<TItem> _itemConverter;
        /// <summary>
        /// The types of the items that are stored in the collection to be converted.
        /// </summary>
        private readonly Type _itemType;
        /// <summary>
        /// Details of the properties on the collection object that this converter will
        /// be serialising/deserialising.
        /// The key is the name of the property.
        /// </summary>
        private readonly Dictionary<string, PropertyInfo> _properties;

        public ComponentCollectionConverterBase()
        {
            // Cache the item type.
            _itemType = typeof(TItem);
            // Get the reflection info for the properties the converter is interested in
            _properties = typeof(TCollection)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => Attribute.IsDefined(p, typeof(JsonIgnoreAttribute)) == false)
                .ToDictionary(p => p.Name, p => p);              
        }
        public ComponentCollectionConverterBase(JsonSerializerOptions options) : this()
        {
            // For performance, use the existing converter if available.
            _itemConverter = (JsonConverter<TItem>)options
                .GetConverter(typeof(TItem));            
        }

        public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = new TCollection();

            while(reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                string propertyName = reader.GetString();
                
                try
                {
                    // Read this property value and set it on the result object
                    if(_properties.TryGetValue(propertyName, out var propertyInfo))
                    {
                        reader.Read();
                        if(propertyInfo.PropertyType == typeof(bool))
                        {
                            propertyInfo.SetValue(result, reader.GetBoolean());
                        } 
                        else if(propertyInfo.PropertyType == typeof(int?))
                        {
                            var stringValue = reader.GetString();
                            if(int.TryParse(stringValue, out int intValue))
                            {
                                propertyInfo.SetValue(result, intValue);
                            }
                        } 
                        else 
                        {
                            propertyInfo.SetValue(result, reader.GetString());
                        }
                    }
                    // Read the values for the child items
                    else if(propertyName == "$values")
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException($"Expected start of array, not {reader.TokenType}");
                        }
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {                            
                            // Get the item.
                            TItem item;
                            if (_itemConverter != null)
                            {
                                item = _itemConverter.Read(ref reader, _itemType, options);
                            }
                            else
                            {
                                item = JsonSerializer.Deserialize<TItem>(ref reader, options);
                            }
                            // Add to group.
                            result.Add(item);
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception($"Error setting property {propertyName}", ex);
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options)
        {            
            writer.WriteStartObject();

            // Write entries for the properties on this collection
            foreach(var property in _properties)
            {
                writer.WritePropertyName(property.Key);
                var valueObj = property.Value.GetValue(value);
                // Write booleans as booleans. Everything else is converted to string.
                if(property.Value.PropertyType == typeof(bool))
                {
                    writer.WriteBooleanValue((bool)valueObj);
                }
                else 
                {
                    writer.WriteStringValue(valueObj?.ToString());
                }
            }

            // Now write the child items.
            writer.WritePropertyName("$values");
            writer.WriteStartArray();
            foreach(var item in value)
            {
                if (_itemConverter != null)
                {
                    _itemConverter.Write(writer, item, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, item, options);
                }
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}