using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Globalization;
using System.IO;

namespace DSI.Core
{
    /// <summary>
    /// Represents the properties parsed from a user configuration file.
    /// </summary>
    public class ApplicationConfiguration
    {
        /// <summary>
        /// File information for the configuration file.
        /// </summary>
        private readonly FileInfo configInfo;


        /// <summary>
        /// JsonSerializer for read and write operations on the configuration file.
        /// </summary>
        private readonly JsonSerializer serializer;


        /// <summary>
        /// JSON Schema for configuration file validation.
        /// </summary>
        private readonly string schemaJson = @"{
            'required': [
                'Debug'
            ],
            'additionalProperties': false,
            'properties': {
                'Debug' : {
                    'type' : 'boolean'
                }
            }
        }";


        /// <summary>
        /// ApplicationContext default constructor.
        /// </summary>
        public ApplicationConfiguration()
        {
            configInfo = null;
            serializer = new JsonSerializer()
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            };
        }


        /// <summary>
        /// ApplicationContext constructor.
        /// </summary>
        /// <param name="configPath">The UIControlledApplication representing the currently executing Revit instance.</param>
        public ApplicationConfiguration(string configPath)
        {
            configInfo = new FileInfo(configPath);
            serializer = new JsonSerializer()
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include
            };

            if (!configInfo.Exists)
            {
                InitializeConfig();
                WriteConfig();
            }
            else
            {
                ReadConfig();
            }
        }


        /// <summary>
        /// Enables debugging features in the code path. The default is false.
        /// </summary>
        public bool Debug { get; set; }


        /// <summary>
        /// Property initialization logic for new configuration files or to fixed malformed configuration files.
        /// </summary>
        private void InitializeConfig() 
        {
            Debug = false;
        }


        /// <summary>
        /// Loads the properties from the configuration file and mutates the current ApplicationConfiguration object. 
        /// </summary>
        private void ReadConfig()
        {
            if (configInfo != null)
            {
                using (var file = File.OpenText(configInfo.FullName))
                using (var reader = new JsonTextReader(file))
                {
                    var schema = JSchema.Parse(schemaJson);
                    var loadedConfig = (JObject)JToken.ReadFrom(reader);

                    if (loadedConfig.IsValid(schema))
                    {
                        var configString = loadedConfig.ToString();
                        JsonConvert.PopulateObject(configString, this);
                        file.Close();
                    }
                    else
                    {
                        InitializeConfig();
                        file.Close();
                        WriteConfig();
                    }
                }
            }
        }


        /// <summary>
        /// Updates the configuration file.
        /// </summary>
        private void WriteConfig()
        {
            if (configInfo != null)
            {
                using (StreamWriter file = new StreamWriter(configInfo.FullName))
                using (JsonWriter writer = new JsonTextWriter(file))
                {
                    serializer.Serialize(writer, this);
                    file.Close();
                }
            }
        }
    }
}
