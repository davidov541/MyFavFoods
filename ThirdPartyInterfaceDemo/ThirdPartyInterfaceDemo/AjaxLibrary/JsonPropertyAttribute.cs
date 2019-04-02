using System;
using System.Collections.Generic;
using System.Text;

namespace AjaxLibrary
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class JsonPropertyAttribute : Attribute
    {
        private String m_Name;
        private JavascriptType m_SerializeAs;
        private String m_Format;
        private Type m_Converter;

        public JsonPropertyAttribute()
        {
        }

        public JsonPropertyAttribute(String name)
        {
            this.m_Name = name;
        }

        public String Name
        {
            get { return this.m_Name; }
            set { this.m_Name = value; }
        }

        public JavascriptType SerializeAs
        {
            get { return this.m_SerializeAs; }
            set { this.m_SerializeAs = value; }
        }

        public String Format
        {
            get { return this.m_Format; }
            set { this.m_Format = value; }
        }

        public Type Converter
        {
            get { return this.m_Converter; }
            set { this.m_Converter = value; }
        }
    }
}
