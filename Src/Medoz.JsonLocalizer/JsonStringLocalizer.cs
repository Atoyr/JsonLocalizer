using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Medoz.JsonLocalizer
{
    public class JsonStringLocalizer: IStringLocalizer
    {
        private readonly JsonLocalizedStringSource _localizedStringSource;

        private JsonStringLocalizer()
        {
            _localizedStringSource = new JsonLocalizedStringSource();
        }

        public JsonStringLocalizer(JsonLocalizedStringSource localizedStringSource)
        {
            _localizedStringSource = localizedStringSource;
        }

        /// <inheritdoc/>
        public LocalizedString this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                string value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: null);
            }
        }

        /// <inheritdoc/>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }
                string format = GetString(name);
                string value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: null);
            }
        }

        private string GetString(string name, CultureInfo? culture = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            CultureInfo keyCulture = culture ?? CultureInfo.CurrentUICulture;
            return _localizedStringSource.GetString(name, keyCulture);
        }

        /// <inheritdoc/>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            CultureInfo? culture = CultureInfo.CurrentUICulture;
            IEnumerable<string> allKey = _localizedStringSource.GetAllKey(culture);
            foreach (var key in allKey)
            {
                var value = GetString(key, culture);
                yield return new LocalizedString(key, value ?? key, resourceNotFound: value == null, searchedLocation: null);
            }
        }
    }
}

