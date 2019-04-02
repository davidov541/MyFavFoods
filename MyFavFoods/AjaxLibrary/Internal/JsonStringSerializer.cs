using System;
using System.IO;
using System.Text;

namespace MyFavFoods.AjaxLibrary.Internal
{
    internal class JsonStringSerializer : JsonSerializer
    {
        public static JsonStringSerializer DefaultInstance = new JsonStringSerializer();

        private JsonStringSerializer()
            : base(JavascriptType.String, null)
        {
        }

        public JsonStringSerializer(String defaultFormat)
            : base(JavascriptType.String, defaultFormat)
        {
        }

        public override Object BaseDeSerialize(TextReader reader)
        {
            StringBuilder result = new StringBuilder();

            Char c = JsonParserUtil.ReadNextChar(reader, true);

            if (c != '"')
            {
                throw new ArgumentException("JsonDeserializationException(\"Expected '\"'");
            }

            Boolean escape = false;
            c = JsonParserUtil.ReadNextChar(reader, true);

            while (c != '"' || escape)
            {
                if (escape)
                {
                    switch (c)
                    {
                        case 'b':
                            c = '\b';
                            break;
                        case 'f':
                            c = '\f';
                            break;
                        case 'n':
                            c = '\n';
                            break;
                        case 'r':
                            c = '\r';
                            break;
                        case 't':
                            c = '\t';
                            break;
                        case 'v':
                            c = '\v';
                            break;
                        case 'x':
                            c = JsonParserUtil.ReadCharCode(reader, 16, 2, 2);
                            break;
                        case 'u':
                            c = JsonParserUtil.ReadCharCode(reader, 16, 4, 4);
                            break;
                    }
                }

                escape = !escape && (c == '\\');

                if (!escape)
                {
                    result.Append(c);
                }
                else if (Char.IsDigit(JsonParserUtil.PeekNextChar(reader, false)))
                {
                    result.Append(JsonParserUtil.ReadCharCode(reader, 8, 1, 3));
                }
                
                c = JsonParserUtil.ReadNextChar(reader, false);
            }

            return result.ToString();
        }

        public override void BaseSerialize(TextWriter writer, Object obj, JsonOptions options, String format, Int32 tabDepth)
        {
            if (obj != null)
            {
                if (String.IsNullOrEmpty(format))
                {
                    format = this.DefaultFormat;
                }

                String serialized;

                if (String.IsNullOrEmpty(format))
                {
                    serialized = String.Format("{0}", obj);
                }
                else
                {
                    serialized = String.Format("{0:" + format + "}", obj);
                }

                writer.Write('"');
                writer.Write(serialized.Replace(@"\", @"\\").Replace("\"", "\\\""));
                writer.Write('"');
            }
            else
            {
                writer.Write("null");
            }
        }
    }
}
