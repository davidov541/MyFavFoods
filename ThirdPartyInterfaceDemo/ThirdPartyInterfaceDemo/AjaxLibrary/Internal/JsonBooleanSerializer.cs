using System;
using System.IO;

namespace AjaxLibrary.Internal
{
    internal class JsonBooleanSerializer : JsonSerializer<Boolean>
    {
        public static JsonBooleanSerializer DefaultInstance = new JsonBooleanSerializer();

        private JsonBooleanSerializer()
            : base(JavascriptType.Boolean, null)
        {
        }

        public JsonBooleanSerializer(String defaultFormat)
            : base(JavascriptType.Boolean, defaultFormat)
        {
        }

        internal override void Serialize(TextWriter writer, Boolean obj, JsonOptions options, String format, Int32 tabDepth)
        {
            writer.Write(obj.ToString().ToLower());
        }

        public override Boolean DeSerialize(TextReader reader)
        {
            String token = JsonParserUtil.GetNextToken(reader);

            if (token == String.Empty || token.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                return Boolean.Parse(token);
            }
        }
    }
}
