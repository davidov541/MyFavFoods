using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MyFavFoods.AjaxLibrary.Internal
{
    internal class JsonMemberSerializationInfo
    {
        private String m_Name;
        private PropertyInfo m_Property;
        private FieldInfo m_Field;
        private Type m_MemberType;
        private JsonSerializer m_Serializer;
        private JsonPropertyAttribute m_JsonProperty;

        public JsonMemberSerializationInfo(FieldInfo field)
            : this(field, null, field.FieldType, (JsonPropertyAttribute)Attribute.GetCustomAttribute(field, typeof(JsonPropertyAttribute)))
        {
        }

        public JsonMemberSerializationInfo(PropertyInfo property)
            : this(null, property, property.PropertyType, (JsonPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(JsonPropertyAttribute)))
        {
        }

        private JsonMemberSerializationInfo(FieldInfo field, PropertyInfo property, Type memberType, JsonPropertyAttribute jsonProperty)
        {
            this.m_Field = field;
            this.m_Property = property;
            this.m_MemberType = memberType;
            this.m_JsonProperty = jsonProperty;

            if (this.m_JsonProperty != null)
            {
                this.m_Serializer = JsonSerializer.CreateSerializer(this.m_MemberType, this.m_JsonProperty.SerializeAs);
            }
            else
            {
                this.m_Serializer = JsonSerializer.CreateSerializer(this.m_MemberType);
            }

            if (this.m_JsonProperty != null && !String.IsNullOrEmpty(this.m_JsonProperty.Name))
            {
                this.m_Name = this.m_JsonProperty.Name;
            }
            else if (this.IsProperty)
            {
                this.m_Name = this.m_Property.Name;
            }
            else
            {
                this.m_Name = this.m_Field.Name;
            }
        }

        public Boolean IsProperty
        {
            get { return this.m_Property != null; }
        }

        public String Name
        {
            get { return this.m_Name; }
        }

        public Type MemberType
        {
            get { return this.m_MemberType; }
        }

        public PropertyInfo Property
        {
            get { return this.m_Property; }
        }

        public FieldInfo Field
        {
            get { return this.m_Field; }
        }

        public JsonSerializer Serializer
        {
            get { return this.m_Serializer; }
        }

        public JsonPropertyAttribute JsonProperty
        {
            get { return this.m_JsonProperty; }
        }

        public Object GetValue(Object instance)
        {
            if (this.IsProperty)
            {
                return this.m_Property.GetValue(instance, new Object[] { });
            }
            else
            {
                return this.m_Field.GetValue(instance);
            }
        }

        public void SetValue(Object instance, Object value)
        {
            if (this.IsProperty)
            {
                this.m_Property.SetValue(instance, value, new object[] { });
            }
            else
            {
                this.m_Field.SetValue(instance, value);
            }
        }
    }
}
