using System;
using System.Collections.Generic;
using System.Text;

namespace AjaxLibrary
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = true)]
    public class JsonSerializableAttribute : Attribute
    {
        private JavascriptType m_SerializeAs;
        private String m_Format;

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
    }
}
