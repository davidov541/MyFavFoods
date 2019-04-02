using System;
using System.IO;

namespace AjaxLibrary.Internal
{
    internal class JsonNumberSerializer : JsonSerializer<Decimal>
    {
        public static JsonNumberSerializer DefaultInstance = new JsonNumberSerializer();

        private JsonNumberSerializer()
            : base(JavascriptType.Number, null)
        {
        }

        public JsonNumberSerializer(String defaultFormat)
            : base(JavascriptType.Number, defaultFormat)
        {
        }

        internal override void Serialize(TextWriter writer, Decimal obj, JsonOptions options, String format, Int32 tabDepth)
        {
            if (String.IsNullOrEmpty(format))
            {
                format = this.DefaultFormat;
            }

            if (String.IsNullOrEmpty(format))
            {
                writer.Write(obj);
            }
            else
            {
                writer.Write(obj.ToString(format));
            }
        }

        public override Decimal DeSerialize(TextReader reader)
        {
            try
            {
                String token = JsonParserUtil.GetNextToken(reader);

                if (token == String.Empty || token.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }
                else
                {
                    return Decimal.Parse(token);
                }
            }
            catch { return 0; }
        }
    }
}
