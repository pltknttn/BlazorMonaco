using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlazorMonaco
{
    /**
     * Provides localization support for monaco-editor.
     * Allows C# to supply localized strings that JavaScript can consume,
     * so the editor UI can be localized without depending on monaco-editor package updates.
     *
     * Two registration modes are supported:
     * 1. Bulk (fast) — all strings are passed to JavaScript once as a plain object.
     *    Use {@link RegisterLocalizationStrings} for this.
     * 2. Per-key (dynamic) — JavaScript calls back into .NET for every missing key.
     *    Use {@link RegisterLocalizationCallback} for this.
     *
     * The bulk mode is recommended for the initial set of strings because it avoids
     * a JSInterop round-trip per string. The callback mode is useful when strings are
     * added after startup or need to be resolved dynamically.
     *
     * Integration with .NET IStringLocalizer / .resx files is done by the hosting app:
     * load strings from IStringLocalizer into a dictionary and pass them via
     * {@link SetStrings} or {@link RegisterLocalizationStrings}. This keeps
     * BlazorMonaco free of a hard dependency on Microsoft.Extensions.Localization.
     */
    public static class MonacoLocalization
    {
        private static readonly Dictionary<string, string> s_localizedStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly object s_lock = new object();
        private static DotNetObjectReference<MonacoLocalizationImpl> s_reference;
        private static bool s_disposed;

        /**
         * Sets a localized string for a given key.
         */
        public static void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Localization key cannot be null or empty.", nameof(key));

            lock (s_lock)
            {
                s_localizedStrings[key] = value;
            }
        }

        /**
         * Sets multiple localized strings at once.
         */
        public static void SetStrings(IDictionary<string, string> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            lock (s_lock)
            {
                foreach (var kvp in values)
                    s_localizedStrings[kvp.Key] = kvp.Value;
            }
        }

        /**
         * Clears all localized strings.
         */
        public static void Clear()
        {
            lock (s_lock)
            {
                s_localizedStrings.Clear();
            }
        }

        /**
         * Gets a localized string by key. Returns null if not found.
         */
        public static string GetString(string key)
        {
            lock (s_lock)
            {
                s_localizedStrings.TryGetValue(key, out var value);
                return value;
            }
        }

        /**
         * Gets a read-only snapshot of all localized strings.
         */
        public static IReadOnlyDictionary<string, string> GetAllStrings()
        {
            lock (s_lock)
            {
                return new Dictionary<string, string>(s_localizedStrings);
            }
        }

        /**
         * Returns the number of registered localized strings.
         */
        public static int Count
        {
            get
            {
                lock (s_lock)
                {
                    return s_localizedStrings.Count;
                }
            }
        }

        /**
         * Creates (or returns the existing) DotNetObjectReference for passing to JavaScript.
         * Call this once during app startup and pass the result to JS interop.
         */
        public static DotNetObjectReference<MonacoLocalizationImpl> GetReference()
        {
            lock (s_lock)
            {
                if (s_disposed)
                    throw new ObjectDisposedException(nameof(MonacoLocalization));

                return s_reference ?? (s_reference = DotNetObjectReference.Create(new MonacoLocalizationImpl()));
            }
        }

        /**
         * Disposes the cached DotNetObjectReference. Call this when the app shuts down
         * (e.g. in Blazor Server's ApplicationLifetime or when the localization service
         * is no longer needed) to release the JSInterop reference.
         */
        public static void Dispose()
        {
            lock (s_lock)
            {
                s_reference?.Dispose();
                s_reference = null;
                s_disposed = true;
            }
        }

        /**
         * The actual JS-invokable implementation. Kept as a separate class so the
         * DotNetObjectReference lifetime can be managed independently from the static
         * string store.
         */
        public sealed class MonacoLocalizationImpl
        {
            /**
             * JS-invokable callback to resolve a localized string by key.
             * JavaScript calls this when it needs a localized string for a key.
             */
            [JSInvokable]
            public string GetLocalizedString(string key)
            {
                return MonacoLocalization.GetString(key);
            }

            /**
             * JS-invokable callback to get all localized strings as a dictionary.
             */
            [JSInvokable]
            public IReadOnlyDictionary<string, string> GetAllStrings()
            {
                return MonacoLocalization.GetAllStrings();
            }
        }
    }
}
