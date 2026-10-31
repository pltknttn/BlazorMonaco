using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMonaco
{
    /**
     * Provides localization support for monaco-editor.
     * Allows C# to supply localized strings that JavaScript can consume,
     * so the editor UI can be localized without depending on monaco-editor package updates.
     */
    public class MonacoLocalization
    {
        private static readonly Dictionary<string, string> s_localizedStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static DotNetObjectReference<MonacoLocalization> s_reference;

        /**
         * Sets a localized string for a given key.
         */
        public static void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Localization key cannot be null or empty.", nameof(key));
            s_localizedStrings[key] = value;
        }

        /**
         * Sets multiple localized strings at once.
         */
        public static void SetStrings(IDictionary<string, string> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            foreach (var kvp in values)
                s_localizedStrings[kvp.Key] = kvp.Value;
        }

        /**
         * Clears all localized strings.
         */
        public static void Clear()
        {
            s_localizedStrings.Clear();
        }

        /**
         * Gets a localized string by key. Returns null if not found.
         */
        public static string GetString(string key)
        {
            s_localizedStrings.TryGetValue(key, out var value);
            return value;
        }

        /**
         * Creates a DotNetObjectReference for passing to JavaScript.
         * Call this once during app startup and pass the result to JS interop.
         */
        public static DotNetObjectReference<MonacoLocalization> GetReference()
        {
            return s_reference ?? (s_reference = DotNetObjectReference.Create(new MonacoLocalization()));
        }

        /**
         * JS-invokable callback to resolve a localized string by key.
         * JavaScript calls this when it needs a localized string for a key.
         */
        [JSInvokable]
        public string GetLocalizedString(string key)
        {
            return GetString(key);
        }

        /**
         * JS-invokable callback to get all localized strings as a dictionary.
         */
        [JSInvokable]
        public Dictionary<string, string> GetAllStrings()
        {
            return new Dictionary<string, string>(s_localizedStrings);
        }
    }
}
