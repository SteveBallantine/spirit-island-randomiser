using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SiRandomizer.Data
{
    public abstract class ComponentCollectionConverterBase<TCollection, TItem> : JsonConverter<TCollection> 
        where TItem : SelectableComponentBase<TItem>
        where TCollection : IComponentCollection<TItem>, new()
    {
        private readonly JsonConverter<TItem> _itemConverter;
        private readonly Type _itemType;
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
                    // Read the property values for the group
                    if(_properties.TryGetValue(propertyName, out var propertyInfo))
                    {
                        reader.Read();                    
                        if(propertyInfo.PropertyType == typeof(bool))
                        {
                            propertyInfo.SetValue(result, reader.GetBoolean());
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

            // Write entries for the properties on this group
            foreach(var property in _properties)
            {
                writer.WritePropertyName(property.Key);
                var valueObj = property.Value.GetValue(value);
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