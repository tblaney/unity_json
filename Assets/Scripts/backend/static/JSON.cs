using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System;
using System.IO.Compression;
using System.Text;

namespace snorri
{
    public static class JSON
    {
        
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented; // Optional: for pretty-printing the JSON
                settings.Converters.Add(new MapJsonConverter());

                return settings;   
            }
        }

        public static bool IsNotify = false;

        public static readonly string FilePathSave = UnityEngine.Application.persistentDataPath + "/saves/";
        public static readonly string FilePathStreamingJson = Application.streamingAssetsPath + "/json/";

        // uncompressed (primarily)
        public static Map GetResourceMap(string name, string typeName = "")
        {
            string resourceName = name;
            if (typeName != "")
            {
                resourceName = typeName + "/" + name;
            }

            // some of these might be compressed, so we should check for both scenarios
            var textAsset = RESOURCES.Load<TextAsset>(RESOURCES.FilePathJson + resourceName);

            if (textAsset == null)
            {
                LOG.Console($"JSON could not find resource map : {name}, looking in streaming");
                return GetStreamingMap(name, typeName);

            } else
            {
                return JsonConvert.DeserializeObject<Map>(textAsset.text, JsonSettings);
            }
        }
        public static Map GetMapFromPath(string path) {
            if (!File.Exists(path))
            {
                LOG.Console($"File not found at {path}");
                return null;
            }
            try
            {
                string jsonData = File.ReadAllText(path);
                return GetMapFromString(jsonData);
            }
            catch (Exception ex)
            {
                LOG.Console($"Error loading map from path {path}: {ex.Message}");
                return null;
            }
        }
        public static Map GetMapFromString(string json) {
            return JsonConvert.DeserializeObject<Map>(json, JsonSettings);
        }
        public static Map GetStreamingMap(string name, string typeName = "")
        {
            string filePathNew = FilePathStreamingJson + name + ".json";
            if (!File.Exists(filePathNew))
                return null;

            string jsonString = File.ReadAllText(filePathNew);
            
            return JsonConvert.DeserializeObject<Map>(jsonString, JsonSettings);
        }

        // compressed
        public static Map GetSaveMap(string name, bool isCompress = true)
        {
            // finds and decompresses   

            if (isCompress)
            {   
                string pathFull = FilePathSave + $"{name}.gz";
                if (!File.Exists(pathFull))
                    return null;

                string jsonString = DecompressToJsonString(pathFull);
                if (jsonString == "")
                    return JsonConvert.DeserializeObject<Map>(jsonString, JsonSettings);
            } else
            {
                string pathFull = FilePathSave + $"{name}.json";
                if (!File.Exists(pathFull))
                {
                    LOG.Console($"JSON get save, could not locate: {pathFull}");
                    return null;
                }

                string jsonString = File.ReadAllText(pathFull);
                if (jsonString != "")
                {
                    return JsonConvert.DeserializeObject<Map>(jsonString, JsonSettings);
                } else
                {
                    LOG.Console($"JSON get save, could not deserialize string: {pathFull}");
                }
            }

            return null;
        }
        public static void SaveMap(string name, Map map, bool isCompress = true, bool isNotify = true)
        {
            // compress and save

            if (isCompress)
            {
                string pathFull = FilePathSave + $"{name}.gz";
                string json = JsonConvert.SerializeObject(map, JsonSettings);

                CompressAndSaveJsonString(json, pathFull);
            } else
            {
                string pathFull = FilePathSave + $"{name}.json";

                string directoryPath = Path.GetDirectoryName(pathFull);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // This will create the directory path if it doesn't exist
                }

                string json = JsonConvert.SerializeObject(map, JsonSettings);

                LOG.Console("file saved to: " + pathFull);

                if (isNotify) {
                    Map notification = new Map();
                    notification.Set<string>("text", "File saved: " + pathFull);
                    GAME.Notify(notification);
                }

                File.WriteAllText(pathFull, json);
            }
        }
        public static string CorrectPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return "/";
            }
            
            // Replace all backslashes with forward slashes
            path = path.Replace("\\", "/");
            
            // Ensure the string ends with a forward slash
            if (!path.EndsWith("/")) {
                path += "/";
            }
            
            return path;
        }
        public static void SaveMapToPath(string name, string filePath, Map map, bool isCompress = true, bool isNotify = true) {

            filePath = CorrectPath(filePath);

            if (isCompress)
            {
                string pathFull = filePath + $"{name}.gz";
                string json = JsonConvert.SerializeObject(map, JsonSettings);

                CompressAndSaveJsonString(json, pathFull);
            } else
            {
                string pathFull = filePath + $"{name}.json";

                string directoryPath = Path.GetDirectoryName(pathFull);
                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // This will create the directory path if it doesn't exist
                }

                string json = JsonConvert.SerializeObject(map, JsonSettings);

                LOG.Console("file saved to: " + pathFull);

                if (isNotify) {
                    Map notification = new Map();
                    notification.Set<string>("text", "File saved: " + pathFull);
                    GAME.Notify(notification);
                }
                
                File.WriteAllText(pathFull, json);
            }
        }
        public static string ToJson(Map map)
        {
            return JsonConvert.SerializeObject(map, JsonSettings);
        }

        public static void CompressAndSaveJsonString(string jsonString, string outputFilePath)
        {
            // Convert the JSON string to bytes
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // Create a MemoryStream with the JSON bytes
            using (MemoryStream originalStream = new MemoryStream(jsonBytes))
            {
                // Create a FileStream for the compressed file
                using (FileStream compressedFileStream = File.Create(outputFilePath))
                {
                    // Create a GZipStream for compression
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        // Copy the JSON data to the compression stream
                        originalStream.CopyTo(compressionStream);
                    }
                }
            }

            LOG.Console("Compressed JSON data and saved: " + outputFilePath);
        }

        public static string DecompressToJsonString(string compressedFilePath)
        {
            // Ensure the compressed file exists
            if (!File.Exists(compressedFilePath))
            {
                LOG.Console("Compressed file does not exist: " + compressedFilePath);
                return "";
            }

            // Initialize a StringBuilder to hold the decompressed data
            StringBuilder jsonContent = new StringBuilder();

            // Open the compressed file
            using (FileStream compressedFileStream = new FileStream(compressedFilePath, FileMode.Open))
            {
                // Create a GZipStream for decompression
                using (GZipStream decompressionStream = new GZipStream(compressedFileStream, CompressionMode.Decompress))
                {
                    // Use a StreamReader to read the decompressed data
                    using (StreamReader reader = new StreamReader(decompressionStream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            jsonContent.AppendLine(line);
                        }
                    }
                }
            }

            LOG.Console("Decompressed JSON content retrieved successfully.");

            // Return the decompressed JSON string
            return jsonContent.ToString();
        }
    }
}