using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;

namespace Medoz.JsonLocalizer
{
    public class JsonLocalizedStringSource
    {
        private readonly IDictionary<string, IDictionary<string, string>> _resource;

        protected internal JsonLocalizedStringSource()
        {
            _resource = new Dictionary<string, IDictionary<string, string>>();
        }

        public JsonLocalizedStringSource(IDictionary<string, IDictionary<string, string>> resource)
        {
            _resource = resource;
        }

        public IEnumerable<string> GetAllKey(CultureInfo cultureInfo)
        {
            if (_resource.Any(x => x.Key == cultureInfo.Name))
            {
                return _resource[cultureInfo.Name].Keys;
            }
            return new List<string>();
        }

        public string GetString(string name, CultureInfo cultureInfo)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException(null, nameof(name));
            IDictionary<string, string> cultureResource;
            if (_resource.Any(x => x.Key == cultureInfo.Name))
            {
                cultureResource = _resource[cultureInfo.Name];
            }
            else if (_resource.Any(x => x.Key == "default"))
            {
                cultureResource = _resource["default"];
            }
            else
            {
                cultureResource = new Dictionary<string, string>();
            }

            if (cultureResource.Any(x => x.Key == name))
            {
                return cultureResource.FirstOrDefault(x => x.Key == name).Value;
            }
            return name;
        }
    }
}
