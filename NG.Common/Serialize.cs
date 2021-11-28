namespace NG.Common;

using System.IO.Compression;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProtoBuf;

public class Serialize
    {
        public static byte[] ProtoBufSerialize(Object item)
        {
            if (item != null)
            {
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);
                    var rt = ms.ToArray();
                    return rt;
                }
                catch (ProtoBuf.ProtoException ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }
            }
            else
            {
                throw new Exception("Object serialize is null");
            }
        }

        public static byte[] ProtoBufSerialize(Object item, bool isCompress)
        {
            if (item != null)
            {
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);
                    var rt = ms.ToArray();
                    if (isCompress)
                    {
                        rt = Compress(rt);
                    }

                    return rt;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }
            }
            else
            {
                throw new Exception("Object serialize is null");
            }
        }

        public static Stream ProtoBufSerializeToStream(Object item, bool isCompress)
        {
            if (item != null)
            {
                try
                {
                    var ms = new MemoryStream();
                    Serializer.Serialize(ms, item);

                    if (isCompress)
                    {
                        var rt = ms.ToArray();
                        return CompressToStream(rt);
                    }
                    else
                    {
                        return ms;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to serialize object", ex);
                }
            }
            else
            {
                throw new Exception("Object serialize is null");
            }
        }

        public static T ProtoBufDeserialize<T>(byte[] byteArray)
        {
            if (byteArray != null && byteArray.Length > 0)
            {
                try
                {
                    var ms = new MemoryStream(byteArray);
                    return Serializer.Deserialize<T>(ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object" + typeof(T).FullName, ex);
                    //return default(T);
                }
            }
            else
            {
                throw new Exception("Object Deserialize is null or empty");
                //return default(T);
            }
        }

        public static T ProtoBufDeserialize<T>(byte[] byteArray, bool isDecompress)
        {
            if (byteArray != null && byteArray.Length > 0)
            {
                try
                {
                    if (isDecompress)
                    {
                        byteArray = Decompress(byteArray);
                    }

                    return ProtoBufDeserialize<T>(byteArray);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Unable to deserialize object", ex);
                    return default(T);
                }
            }
            else
            {
                throw new Exception("Object Deserialize is null or empty");
                //return default(T);
            }
        }

        public static object ProtoBufDeserialize(byte[] byteArray, Type type)
        {
            if (byteArray != null && byteArray.Length > 0)
            {
                try
                {
                    var ms = new MemoryStream(byteArray);
                    return Serializer.Deserialize(type, ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object", ex);
                    //return default(T);
                }
            }
            else
            {
                throw new Exception("Object Deserialize is null or empty");
                //return default(T);
            }
        }

        public static object ProtoBufDeserialize(byte[] byteArray, Type type, bool isDecompress)
        {
            if (byteArray != null && byteArray.Length > 0)
            {
                try
                {
                    if (isDecompress)
                    {
                        byteArray = Decompress(byteArray);
                    }

                    return ProtoBufDeserialize(byteArray, type);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to deserialize object", ex);
                    //return default(T);
                }
            }
            else
            {
                throw new Exception("Object Deserialize is null or empty");
                //return default(T);
            }
        }


        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory.ToArray();
            }
        }

        public static Stream CompressToStream(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }

                return memory;
            }
        }

        public static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    } while (count > 0);

                    return memory.ToArray();
                }
            }
        }

        public static string JsonSerializeObject<T>(T obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss",
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
        }

        public static T JsonDeserializeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            var settings = new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Arrays,
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object JsonDeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static object JsonDeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        public static T JsonDeserializeObjectPrivateSet<T>(string json)
        {
            var obj = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
            return obj;
        }

        public static byte[] BinarySerializer(object @object)
        {
            if (@object == null) return null;
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, @object);
                bytes = memoryStream.ToArray();
            }

            bytes = Compress(bytes);
            return bytes;
        }

        public static T BinaryDeserializer<T>(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return default(T);
            byteArray = Decompress(byteArray);
            T returnValue;
            using (var memoryStream = new MemoryStream(byteArray))
            {
                IFormatter binaryFormatter = new BinaryFormatter();
                returnValue = (T)binaryFormatter.Deserialize(memoryStream);
            }

            return returnValue;
        }
    }

    public class PrivateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable) return prop;
            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
            return prop;
        }
    }