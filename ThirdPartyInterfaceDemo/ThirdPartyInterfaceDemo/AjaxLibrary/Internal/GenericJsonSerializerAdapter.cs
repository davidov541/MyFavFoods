using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace AjaxLibrary.Internal
{
    internal class GenericJsonSerializerAdapter<T> : JsonObjectSerializer<T> where T : new()
    {
        private JsonSerializer m_InnerSerializer;

        public GenericJsonSerializerAdapter(JsonSerializer innerSerializer)
            : base(false)
        {
            this.m_InnerSerializer = innerSerializer;
        }

        public override JavascriptType SerializerType
        {
            get
            {
                return this.m_InnerSerializer.SerializerType;
            }
        }

        public override string DefaultFormat
        {
            get
            {
                return this.m_InnerSerializer.DefaultFormat;
            }
        }

        public override T DeSerialize(TextReader reader)
        {
            return (T)Convert.ChangeType(this.m_InnerSerializer.BaseDeSerialize(reader), typeof(T), new CultureInfo("en-US"));
        }

        internal override void Serialize(TextWriter writer, T obj, JsonOptions options, String format, int tabDepth)
        {
            this.m_InnerSerializer.BaseSerialize(writer, (Object)obj, options, format, tabDepth);
        }
    }
}
