using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Medoz.JsonLocalizer
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, Lazy<JsonStringLocalizer>> _localizerCache = new ();

        private readonly JsonLocalizedStringSourceProvider _jsonLocalizedStringSourceProvider;

        public JsonStringLocalizerFactory(JsonLocalizedStringSourceProvider jsonLocalizedStringSourceProvider)
        {
            _jsonLocalizedStringSourceProvider = jsonLocalizedStringSourceProvider;
        }

        /// <inheritdoc/>
        public IStringLocalizer Create(string baseName, string location)
        {
            var lazy = _localizerCache.GetOrAdd(location + "." + baseName, _ => new Lazy<JsonStringLocalizer>(CreateJsonStringLocalizer(baseName, location)));
            return lazy.Value;
        }

        /// <inheritdoc/>
        public IStringLocalizer Create(Type resourceSource)
        {
            var lazy = _localizerCache.GetOrAdd(resourceSource.FullName, _ => new Lazy<JsonStringLocalizer>(CreateJsonStringLocalizer(resourceSource)));
            return lazy.Value;
        }


        private JsonStringLocalizer CreateJsonStringLocalizer(Type resourceSource)
        {
            var assemblyName = new AssemblyName(resourceSource.GetTypeInfo().Assembly.FullName ?? string.Empty);
            return CreateJsonStringLocalizer(resourceSource.Name, assemblyName.Name);
        }

        private JsonStringLocalizer CreateJsonStringLocalizer(string baseName, string location)
        {
            JsonLocalizedStringSource source = _jsonLocalizedStringSourceProvider.GetLocalizedStringSource(baseName, location);
            return new JsonStringLocalizer(source);
        }
    }
}
