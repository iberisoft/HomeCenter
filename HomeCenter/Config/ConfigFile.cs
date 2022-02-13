using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HomeCenter.Config
{
    public static class ConfigFile
    {
        static readonly ISerializer m_Serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .Build();
        static readonly IDeserializer m_Deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        public static T Load<T>(string filePath)
            where T : IValidator
        {
            var config = m_Deserializer.Deserialize<T>(File.ReadAllText(filePath));
            config.Validate();
            return config;
        }

        public static void Save<T>(string filePath, T config) => File.WriteAllText(filePath,  m_Serializer.Serialize(config));
    }
}
