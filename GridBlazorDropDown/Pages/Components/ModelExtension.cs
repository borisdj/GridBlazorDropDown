using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GridBlazorDropDown.Pages.Components
{
    public class ModelExtension
    {
        public Type ModelType { get; }
        public object Item { get; set; }

        public ModelExtension(Type type, object item)
        {
            ModelType = type;
            Item = item;
        }

        public void SetValue(string columnName, object? value)
        {
            PropertyInfo[] properties = ModelType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == columnName)
                {
                    if (Int32.TryParse((string?)value, out int ivalue))
                    {
                        property.SetValue(Item, ivalue);
                    }
                    else
                    {
                        property.SetValue(Item, value);
                    }
                    return;
                }
            }
        }

        public object? GetValue(string columnName)
        {
            PropertyInfo[] properties = ModelType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == columnName)
                {
                    return property.GetValue(Item);
                }
            }
            return null;
        }

        public List<string> GetKeyNames()
        {
            PropertyInfo[] properties = ModelType.GetProperties();
            List<string> ret = new();
            foreach (PropertyInfo property in properties)
            {
                if (Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) is KeyAttribute)
                {
                    ret.Add(property.Name);
                }
            }
            return ret;
        }

        public List<object?> GetKeyValues()
        {
            PropertyInfo[] properties = ModelType.GetProperties();
            List<object?> ret = new();
            foreach (PropertyInfo property in properties)
            {
                if (Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) is KeyAttribute)
                {
                    ret.Add(GetValue(property.Name));
                }
            }
            return ret;
        }
    }
}