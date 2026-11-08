using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace BlazorMonaco.Resources
{
    /**
     * Loads localized strings from the embedded Monaco .resx resources using ResourceManager.
     * This avoids the dependency on Microsoft.Extensions.Localization / IStringLocalizer,
     * which does not work reliably with resources from referenced assemblies in Blazor WebAssembly.
     */
    public static class MonacoResourceLoader
    {
        private static readonly ResourceManager _resourceManager =
            new ResourceManager("BlazorMonaco.Resources.Monaco", typeof(MonacoResourceLoader).Assembly);

        /**
         * Gets all localized strings for the specified culture.
         * Falls back to the invariant Monaco.resx if the culture has no dedicated .resx file.
         */
        public static Dictionary<string, string> GetStrings(CultureInfo culture)
        {
            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            var resourceSet = _resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: true);
            var result = new Dictionary<string, string>();

            if (resourceSet != null)
            {
                foreach (System.Collections.DictionaryEntry entry in resourceSet)
                {
                    if (entry.Key is string key && entry.Value is string value)
                        result[key] = value;
                }
            }

            return result;
        }
    }
}
