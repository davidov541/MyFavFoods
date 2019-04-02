using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AjaxLibrary.Internal
{
    internal static class JsonParserUtil
    {
        public static Int32 ReadStartObject(TextReader reader)
        {
            Int32 parenCount = ReadNOptional(reader, 0, true, '(');
            ReadExpected(reader, true, '{');
            return parenCount;
        }

        public static Int32 ReadStartArray(TextReader reader)
        {
            Int32 parenCount = ReadNOptional(reader, 0, true, '(');
            ReadExpected(reader, true, '[');
            return parenCount;
        }

        public static String ReadMemberName(TextReader reader)
        {
            StringBuilder nameBuilder = new StringBuilder();
            Boolean quoted = false;

            Char c = JsonParserUtil.ReadNextChar(reader, true);
            if (c == '"')
            {
                quoted = true;
            }
            else
            {
                nameBuilder.Append(c);
            }

            c = JsonParserUtil.ReadNextChar(reader, false);

            if (quoted)
            {
                Boolean escaped = false;
                while (c != '"' || escaped)
                {
                    escaped = !escaped && (c == '\\');
                    nameBuilder.Append(c);
                    c = JsonParserUtil.ReadNextChar(reader, false);
                }

                if (JsonParserUtil.ReadNextChar(reader, false) != ':') throw new ArgumentException("JsonDeserializationException(\"Expected ':'");
            }
            else
            {
                while (c != ':')
                {
                    nameBuilder.Append(c);
                    c = JsonParserUtil.ReadNextChar(reader, false);
                }
            }

            return nameBuilder.ToString();
        }

        public static void ReadEndObject(TextReader reader, Int32 parens)
        {
            ReadExpected(reader, true, '}');
            if (ReadNOptional(reader, 0, true, ')') != parens)
            {
                throw new ArgumentException("JsonDeserializationException(\"Expected ')'");
            }
        }

        public static void ReadEndArray(TextReader reader, Int32 parens)
        {
            ReadExpected(reader, true, ']');
            if (ReadNOptional(reader, 0, true, ')') != parens)
            {
                throw new ArgumentException("JsonDeserializationException(\"Expected ')'");
            }
        }

        public static Char ReadNextChar(TextReader reader, Boolean ignoreWhitespace)
        {
            Int32 val = reader.Read();

            if (val < 0)
            {
                throw new InvalidOperationException("End of File");
            }

            Char c = (Char)val;
            while (ignoreWhitespace && Char.IsWhiteSpace(c))
            {
                val = reader.Read();

                if (val < 0)
                {
                    throw new InvalidOperationException("End of File");
                }

                c = (Char)val;
            }

            return c;
        }

        public static void ReadExpected(TextReader reader, Boolean ignoreWhitespace, Char expected)
        {
            Char c = ReadNextChar(reader, ignoreWhitespace);
            if (c != expected) throw new ArgumentException("JsonDeserializationException(String.Format(\"Expected '{0}'\", expected)");
        }

        public static Int32 ReadNOptional(TextReader reader, Int32 max, Boolean ignoreWhitespace, Char optional)
        {
            Int32 count = 0;
            Char c = PeekNextChar(reader, ignoreWhitespace);
            while (c == optional && (count < max || max == 0))
            {
                count++;
                ReadNextChar(reader, ignoreWhitespace);
                c = PeekNextChar(reader, ignoreWhitespace);
            }

            return count;
        }

        public static Char PeekNextChar(TextReader reader, Boolean ignoreWhitespace)
        {
            Char c = (Char)reader.Peek();
            while (ignoreWhitespace && Char.IsWhiteSpace(c))
            {
                reader.Read();
                c = (Char)reader.Peek();
            }

            return c;
        }

        public static void SkipWhitespace(TextReader reader)
        {
            Char c = (Char)reader.Peek();
            while (Char.IsWhiteSpace(c))
            {
                reader.Read();
                c = (Char)reader.Peek();
            }
        }

        public static String GetNextToken(TextReader reader)
        {
            StringBuilder str = new StringBuilder();

            JsonParserUtil.SkipWhitespace(reader);
            Char c = JsonParserUtil.PeekNextChar(reader, false);

            while (c != ',' && c != '}' && c != ';' && c != ']' && c != ')' && !Char.IsWhiteSpace(c))
            {
                str.Append(c);
                JsonParserUtil.ReadNextChar(reader, false);
                c = JsonParserUtil.PeekNextChar(reader, false);
            }

            return str.ToString();
        }

        public static readonly Char[] radixTable = new Char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static Char ReadCharCode(TextReader reader, Int32 radix, Int32 minDigits, Int32 maxDigits)
        {
            Int32[] digits = new Int32[maxDigits];
            Int32 digitCount = 0;

            while (digitCount < maxDigits)
            {
                Char c = PeekNextChar(reader, false);

                Int32 val = Array.IndexOf(radixTable, c);

                if (val == -1 || val >= radix)
                {
                    if (digitCount < minDigits)
                    {
                        throw new ArgumentException("JsonDeserializationException(\"Excpected Digit.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    digits[digitCount] = val;
                    digitCount++;

                    ReadNextChar(reader, false);
                }
            }

            Int32 charCode = 0;
            Int32 multiplier = 1;

            for (Int32 i = digitCount - 1; i >= 0; i--, multiplier *= radix)
            {
                charCode += digits[i] * multiplier;
            }

            return (Char)charCode;
        }

        public static String ConsumeObject(TextReader reader, params Char[] endBlock)
        {
            StringBuilder sb = new StringBuilder();
            SkipWhitespace(reader);

            Int32 start = reader.Peek();
            if (start == -1)
            {
                throw new ArgumentException("JsonDeserializationException(\"Unexpected EOF.");
            }

            if (start == '"' || start == '\'')
            {
                sb.Append((String)JsonStringSerializer.DefaultInstance.BaseDeSerialize(reader));
            }
            else if (start == '[')
            {
                sb.Append('[');

                Boolean first = true;
                while (reader.Read() != ']')
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    else
                    {
                        first = false;
                    }

                    sb.Append(ConsumeObject(reader, ',', ']'));
                }
            }
            else if (start == '{')
            {
                sb.Append('{');

                Boolean first = true;
                while (reader.Read() != '}')
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    else
                    {
                        first = false;
                    }

                    sb.Append(JsonParserUtil.ReadMemberName(reader));
                    sb.Append(": ");
                    sb.Append(ConsumeObject(reader, ',', '}'));
                }
            }
            else
            {
                Char c = ReadNextChar(reader, false);
                while (Array.IndexOf(endBlock, c) < 0)
                {
                    sb.Append(c);
                    c = ReadNextChar(reader, false);
                }
            }

            return sb.ToString();
        }
    }
}
