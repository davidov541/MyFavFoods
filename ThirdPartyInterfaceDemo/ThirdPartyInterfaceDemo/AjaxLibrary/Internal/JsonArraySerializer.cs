using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AjaxLibrary.Internal
{
    internal class JsonArraySerializer<TEnumerable, TElement> : JsonSerializer<TEnumerable>
        where TElement : new()
        where TEnumerable : class, IEnumerable<TElement>
    {
        private JsonObjectSerializer<TElement> m_ElementSerializer = JsonObjectSerializer<TElement>.CreateSerializer();

        public JsonArraySerializer()
            : base(JavascriptType.Array, null)
        {
        }

        public override TEnumerable DeSerialize(TextReader reader)
        {
            List<TElement> result = new List<TElement>();

            Int32 parenCount = JsonParserUtil.ReadStartArray(reader);
            Char c = JsonParserUtil.PeekNextChar(reader, true);

            while (c != ']')
            {
                result.Add(this.m_ElementSerializer.DeSerialize(reader));

                c = JsonParserUtil.PeekNextChar(reader, true);

                if (c != ',' && c != ']')
                {
                    throw new ArgumentException("JsonDeserializationException(\"Expected ']'");
                }
                else if (c == ',')
                {
                    JsonParserUtil.ReadNextChar(reader, true);
                }
            }

            JsonParserUtil.ReadEndArray(reader, parenCount);

            if (typeof(TEnumerable).IsArray)
            {
                return result.ToArray() as TEnumerable;
            }
            else
            {
                return (TEnumerable)Activator.CreateInstance(typeof(TEnumerable), result);
            }
        }

        internal override void Serialize(TextWriter writer, TEnumerable obj, JsonOptions options, string format, int tabDepth)
        {
            if ((options & JsonOptions.EnclosingParens) != 0)
            {
                writer.Write('(');
            }

            writer.Write('[');
            Boolean first = true;

            foreach (TElement element in obj)
            {
                if (!first)
                {
                    writer.Write(',');
                }

                first = false;

                if ((options & JsonOptions.Formatted) != 0)
                {
                    writer.WriteLine();
                    writer.Write(new String(' ', tabDepth * 2));
                }

                this.m_ElementSerializer.Serialize(writer, element, options, null, tabDepth + 1);
            }

            writer.Write(']');

            if ((options & JsonOptions.EnclosingParens) != 0)
            {
                writer.Write(')');
            }
        }
    }
}
