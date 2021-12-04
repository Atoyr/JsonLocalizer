using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;

namespace Medoz.JsonLocalizer
{
    public class JsonLocalizedStringSourceProvider
    {
        private string _resourcePath { get; set; } = "Resources";
        private string _baseFileName { get; set; } = "languageResource";
        private const string extend = ".json";

        public JsonLocalizedStringSourceProvider() {}
        public JsonLocalizedStringSourceProvider(string resourcePath, string baseFileName)
        {
            _resourcePath = resourcePath;
            _baseFileName = baseFileName;
        }

        public JsonLocalizedStringSource GetLocalizedStringSource(string baseName, string location)
        {
            IDictionary<string, IDictionary<string, string>> resource;

            var targetFileName1 = Path.Combine(location, baseName) + extend;
            var targetFileName2 = targetFileName1.Replace('\\','.');

            if (File.Exists(targetFileName1))
            {
                resource = getLocalizedStringSource(targetFileName1);
            }
            else if (File.Exists(targetFileName2))
            {
                resource = getLocalizedStringSource(targetFileName2);
            }
            else
            {
                resource = getLocalizedStringSource("");
            }
            return new JsonLocalizedStringSource(resource);
        }

        private IDictionary<string, IDictionary<string, string>> getLocalizedStringSource(string defaultFileName)
        {
            var resource = readJsonFile(getGlobalJsonFile());
            if (string.IsNullOrEmpty(defaultFileName )) return resource;
            foreach (var cultureKv in readJsonFile(defaultFileName))
            {
                if (resource.Any(x => x.Key == cultureKv.Key))
                {
                    foreach(var kv in cultureKv.Value)
                    {
                        resource[cultureKv.Key][kv.Key] = kv.Value;
                    }
                }
                else
                {
                    resource[cultureKv.Key] = cultureKv.Value;
                }
            }
            return resource;
        }

        private Dictionary<string, IDictionary<string, string>> readJsonFile(string basePath)
        {
            var resource = new Dictionary<string, IDictionary<string, string>>();
            foreach(var fileName in Directory.GetFiles(basePath + "*" + extend))
            {
                var f = Path.GetFileName(basePath);
                var source = Path.GetFileNameWithoutExtension(fileName).Replace(f, "");
                string location = "default";
                if (!string.IsNullOrEmpty(source))
                {
                    location = source.Substring(1);
                }
                using FileStream openStream = File.OpenRead(fileName);
                resource[location] = JsonSerializer.Deserialize<Dictionary<string, string>>(openStream);
            }
            return resource;
        }

        private string getGlobalJsonFile() => Path.Combine(_resourcePath, _baseFileName);
    }
}
