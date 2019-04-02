using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using AjaxLibrary.Internal;
using System.Globalization;

namespace AjaxLibrary
{
    public abstract class JsonSerializer
    {
        private static readonly Type[] numberTypes = new Type[] { typeof(Byte), typeof(SByte), typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32), typeof(Int64), typeof(UInt64), typeof(IntPtr), typeof(UIntPtr), typeof(Single), typeof(Double), typeof(Decimal),
            typeof(Byte?), typeof(SByte?), typeof(Int16?), typeof(UInt16?), typeof(Int32?), typeof(UInt32?), typeof(Int64?), typeof(UInt64?), typeof(IntPtr?), typeof(UIntPtr?), typeof(Single?), typeof(Double?), typeof(Decimal?)};

        private static readonly Type[] stringTypes = new Type[] { typeof(String), typeof(Char), typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(Uri), typeof(StringBuilder), typeof(UriBuilder),
            typeof(Char?), typeof(DateTime?), typeof(TimeSpan?), typeof(Guid?) };

        private static Dictionary<Type, JsonSerializer> serializers = new Dictionary<Type, JsonSerializer>();
        private static Dictionary<Type, JsonSerializer> arraySerializers = new Dictionary<Type, JsonSerializer>();

        private JavascriptType m_SerializerType = JavascriptType.Object;
        private String m_DefaultFormat;

        protected JsonSerializer()
        {
        }

        protected JsonSerializer(JavascriptType serializerType, String defaultFormat)
        {
            this.m_SerializerType = serializerType;
            this.m_DefaultFormat = defaultFormat;
        }

        public static JsonSerializer CreateSerializer(Type t)
        {
            return CreateSerializer(t, JavascriptType.Unknown);
        }

        public static JsonSerializer CreateSerializer(Type t, JavascriptType serializeAs)
        {
            JsonSerializableAttribute serializableAttribute = (JsonSerializableAttribute)Attribute.GetCustomAttribute(t, typeof(JsonSerializableAttribute));

            if (serializeAs == JavascriptType.Unknown && serializableAttribute != null)
            {
                serializeAs = serializableAttribute.SerializeAs;
            }

            if (serializeAs == JavascriptType.Boolean || (serializeAs == JavascriptType.Unknown && t == typeof(Boolean)))
            {
                if (serializableAttribute == null)
                {
                    return JsonBooleanSerializer.DefaultInstance;
                }
                else
                {
                    return new JsonBooleanSerializer(serializableAttribute.Format);
                }
            }
            else if (serializeAs == JavascriptType.Number || (serializeAs == JavascriptType.Unknown && (Array.IndexOf(numberTypes, t) >= 0 || t.IsEnum)))
            {
                if (serializableAttribute == null)
                {
                    return JsonNumberSerializer.DefaultInstance;
                }
                else
                {
                    return new JsonNumberSerializer(serializableAttribute.Format);
                }
            }
            else if (serializeAs == JavascriptType.String || (serializeAs == JavascriptType.Unknown && Array.IndexOf(stringTypes, t) >= 0))
            {
                if (serializableAttribute == null)
                {
                    return JsonStringSerializer.DefaultInstance;
                }
                else
                {
                    return new JsonStringSerializer(serializableAttribute.Format);
                }
            }
            else if ((serializeAs == JavascriptType.Array || serializeAs == JavascriptType.Unknown) &&
                (t.GetInterface("IEnumerable`1", false) != null))
            {
                Type elementType;
                if (t.IsArray)
                {
                    elementType = t.GetElementType();
                }
                else
                {
                    Type genericEnumerable = t.GetInterface("IEnumerable`1", false);
                    elementType = genericEnumerable.GetGenericArguments()[0];
                }

                if (!arraySerializers.ContainsKey(t))
                {
                    Type serializerType = typeof(JsonArraySerializer<,>);
                    serializerType = serializerType.MakeGenericType(t, elementType);
                    arraySerializers.Add(t, (JsonSerializer)Activator.CreateInstance(serializerType));
                }

                return arraySerializers[t];
            }
            else if (serializeAs == JavascriptType.Object || serializeAs == JavascriptType.Unknown)
            {
                if (!serializers.ContainsKey(t))
                {
                    Type serializerType = typeof(JsonObjectSerializer<>);
                    serializerType = serializerType.MakeGenericType(t);
                    serializers.Add(t, (JsonSerializer)Activator.CreateInstance(serializerType));
                }

                return serializers[t];
            }
            else
            {
                throw new ArgumentException("JsonSerializationException(\"Unable to create serializer.");
            }
        }

        public event EventHandler<UnrecognizedMemberEventArgs> UnrecognizedMemeber;

        public virtual JavascriptType SerializerType
        {
            get { return this.m_SerializerType; }
        }

        public virtual String DefaultFormat
        {
            get { return this.m_DefaultFormat; }
        }
        
        protected virtual void OnUnrecognizedMember(String memeberName, String value)
        {
            if (this.UnrecognizedMemeber != null)
            {
                this.UnrecognizedMemeber(this, new UnrecognizedMemberEventArgs(memeberName, value));
            }
        }

        public abstract Object BaseDeSerialize(TextReader reader);

        public void BaseSerialize(TextWriter writer, Object obj)
        {
            this.BaseSerialize(writer, obj, JsonOptions.None, null, 0);
        }

        public void BaseSerialize(TextWriter writer, Object obj, JsonOptions options)
        {
            this.BaseSerialize(writer, obj, JsonOptions.None, null, 0);
        }

        public void BaseSerialize(TextWriter writer, Object obj, JsonOptions options, String format)
        {
            this.BaseSerialize(writer, obj, options, format, 0);
        }

        public abstract void BaseSerialize(TextWriter writer, Object obj, JsonOptions options, String format, Int32 tabDepth);
    }

    public abstract class JsonSerializer<T> : JsonSerializer
    {
        protected JsonSerializer()
        {
        }

        protected JsonSerializer(JavascriptType serializerType, String defaultFormat)
            : base(serializerType, defaultFormat)
        {
        }

        public void Serialize(TextWriter writer, T obj)
        {
            this.Serialize(writer, obj, JsonOptions.None, null, 0);
        }

        public void Serialize(TextWriter writer, T obj, JsonOptions options)
        {
            this.Serialize(writer, obj, options, null, 0);
        }

        public void Serialize(TextWriter writer, T obj, JsonOptions options, String format)
        {
            this.Serialize(writer, obj, options, format, 0);
        }

        internal abstract void Serialize(TextWriter writer, T obj, JsonOptions options, String format, Int32 tabDepth);

        public override Object BaseDeSerialize(TextReader reader)
        {
            return (Object)this.DeSerialize(reader);
        }

        public override void BaseSerialize(TextWriter writer, Object obj, JsonOptions options, String format, Int32 tabDepth)
        {
            this.Serialize(writer, (T)obj, options, format, tabDepth);
        }

        public abstract T DeSerialize(TextReader reader);

    }

    public class JsonObjectSerializer<T> : JsonSerializer<T> where T : new()
    {
        Dictionary<String, JsonMemberSerializationInfo> memberSerializers = new Dictionary<String, JsonMemberSerializationInfo>();

        public static JsonObjectSerializer<T> CreateSerializer()
        {
            return CreateSerializer(JavascriptType.Unknown);
        }

        public static JsonObjectSerializer<T> CreateSerializer(JavascriptType serializeAs)
        {
            JsonSerializer serializer = CreateSerializer(typeof(T), serializeAs);
            if (serializer is JsonObjectSerializer<T>)
            {
                return (JsonObjectSerializer<T>)serializer;
            }
            else
            {
                return new GenericJsonSerializerAdapter<T>(serializer);
            }
        }

        public JsonObjectSerializer()
            : this(true)
        {
        }

        protected internal JsonObjectSerializer(Boolean createSerializers)
        {
            if (createSerializers)
            {
                Boolean markedSerializeable = Attribute.IsDefined(typeof(T), typeof(JsonSerializableAttribute));

                if (markedSerializeable)
                {
                    foreach (PropertyInfo pInfo in typeof(T).GetProperties())
                    {
                        if (Attribute.IsDefined(pInfo, typeof(JsonPropertyAttribute)))
                        {
                            JsonMemberSerializationInfo serializationInfo = new JsonMemberSerializationInfo(pInfo);
                            this.memberSerializers.Add(serializationInfo.Name, serializationInfo);
                        }
                    }

                    foreach (FieldInfo fInfo in typeof(T).GetFields())
                    {
                        if (Attribute.IsDefined(fInfo, typeof(JsonPropertyAttribute)))
                        {
                            JsonMemberSerializationInfo serializationInfo = new JsonMemberSerializationInfo(fInfo);
                            this.memberSerializers.Add(serializationInfo.Name, serializationInfo);
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo pInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        JsonMemberSerializationInfo serializationInfo = new JsonMemberSerializationInfo(pInfo);
                        this.memberSerializers.Add(serializationInfo.Name, serializationInfo);
                    }
                }
            }
        }

        public override T DeSerialize(TextReader reader)
        {
            T result = Activator.CreateInstance<T>();

            Int32 parenCount = JsonParserUtil.ReadStartObject(reader);

            Boolean hasMembers = (JsonParserUtil.PeekNextChar(reader, true) != '}');
            while (hasMembers)
            {
                this.ReadMemeber(reader, result);
                hasMembers = (JsonParserUtil.PeekNextChar(reader, true) == ',');

                if (hasMembers)
                {
                    JsonParserUtil.ReadNextChar(reader, true);
                }
            }

            JsonParserUtil.ReadEndObject(reader, parenCount);

            return result;
        }

        private void ReadMemeber(TextReader reader, T result)
        {
            String name = JsonParserUtil.ReadMemberName(reader);
            Debug.WriteLine(name);
            
            JsonMemberSerializationInfo sInfo;

            if (!this.memberSerializers.TryGetValue(name, out sInfo))
            {
                this.OnUnrecognizedMember(name, JsonParserUtil.ConsumeObject(reader, ',', '}'));
                return;
            }

            Object memberValue = sInfo.Serializer.BaseDeSerialize(reader);

            //System.Diagnostics.Debug.WriteLine(name + ' ' + memberValue);

            Type targetType = sInfo.MemberType;

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            if (memberValue == null)
            {
                sInfo.SetValue(result, null);
            }
            else
            {
                if (targetType.IsEnum)
                {
                    Type integerType = Enum.GetUnderlyingType(targetType);
                    if (integerType == typeof(UInt64))
                    {
                        sInfo.SetValue(result, Convert.ToUInt64(memberValue));
                    }
                    else if (integerType == typeof(Int64))
                    {
                        sInfo.SetValue(result, Convert.ToInt64(memberValue));
                    }
                    else if (integerType == typeof(UInt32))
                    {
                        sInfo.SetValue(result, Convert.ToUInt32(memberValue));
                    }
                    else if (integerType == typeof(Int32))
                    {
                        sInfo.SetValue(result, Convert.ToInt32(memberValue));
                    }
                    else if (integerType == typeof(UInt16))
                    {
                        sInfo.SetValue(result, Convert.ToUInt16(memberValue));
                    }
                    else if (integerType == typeof(Int16))
                    {
                        sInfo.SetValue(result, Convert.ToInt16(memberValue));
                    }
                    else if (integerType == typeof(Byte))
                    {
                        sInfo.SetValue(result, Convert.ToByte(memberValue));
                    }
                    else if (integerType == typeof(SByte))
                    {
                        sInfo.SetValue(result, Convert.ToSByte(memberValue));
                    }
                }
                else
                {
                    try
                    {
                        sInfo.SetValue(result, Convert.ChangeType(memberValue, targetType, new CultureInfo("en-US")));
                    }
                    catch
                    {
                        if (memberValue != null)
                        {
                            ConstructorInfo ctorInfo = targetType.GetConstructor(new Type[] { memberValue.GetType() });

                            if (ctorInfo != null)
                            {
                                sInfo.SetValue(result, ctorInfo.Invoke(new Object[] { memberValue }));
                            }
                            else
                            {
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        internal override void Serialize(TextWriter writer, T obj, JsonOptions options, String format, Int32 tabDepth)
        {
            if ((options & JsonOptions.EnclosingParens) != 0)
            {
                writer.Write('(');
            }

            writer.Write('{');
            Boolean first = true;

            foreach (JsonMemberSerializationInfo sInfo in this.memberSerializers.Values)
            {
                Object val = sInfo.GetValue(obj);

                if (val != null || (options & JsonOptions.IncludeNulls) != 0)
                {
                    if (!first)
                    {
                        writer.Write(',');
                    }
                    else
                    {
                        first = false;
                    }

                    if ((options & JsonOptions.Formatted) != 0)
                    {
                        writer.WriteLine();
                        writer.Write(new String(' ', tabDepth * 2));
                    }

                    if ((options & JsonOptions.QuoteNames) != 0)
                    {
                        writer.Write('"');
                    }

                    writer.Write(sInfo.Name);

                    if ((options & JsonOptions.QuoteNames) != 0)
                    {
                        writer.Write('"');
                    }

                    writer.Write(':');

                    if ((options & JsonOptions.Formatted) != 0)
                    {
                        writer.Write(' ');
                    }

                    if (val == null)
                    {
                        writer.Write("null");
                    }
                    else
                    {
                        if (sInfo.JsonProperty == null)
                        {
                            sInfo.Serializer.BaseSerialize(writer, val, options, null, tabDepth + 1);
                        }
                        else
                        {
                            sInfo.Serializer.BaseSerialize(writer, val, options, sInfo.JsonProperty.Format, tabDepth + 1);
                        }
                    }
                }
            }

            writer.Write('}');

            if ((options & JsonOptions.EnclosingParens) != 0)
            {
                writer.Write(')');
            }
        }
    }

    public class UnrecognizedMemberEventArgs : EventArgs
    {
        public String Name { get; set; }

        public String Value { get; set; }

        public UnrecognizedMemberEventArgs()
        {
        }

        public UnrecognizedMemberEventArgs(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
