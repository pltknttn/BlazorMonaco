using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Threading;

namespace BlazorMonaco.Helpers
{
    internal static class JsRuntimeExt
    {
        // Use AsyncLocal for thread-safe storage in async context
        // This allows each async flow to have its own IJSRuntime (important for Blazor Server)
        private static readonly AsyncLocal<IJSRuntime> s_runtime = new AsyncLocal<IJSRuntime>();

        public static IJSRuntime UpdateRuntime(IJSRuntime jsRuntime)
        {
            // Only update if a non-null runtime is provided
            if (jsRuntime != null)
            {
                // Store the runtime for this async flow (including WASM where it's IJSInProcessRuntime)
                s_runtime.Value = jsRuntime;
            }
            
            // Return the stored runtime if available
            return s_runtime.Value;
        }

        public static async Task SafeInvokeAsync(this IJSRuntime jsRuntime, string identifier, params object[] args)
        {
            if (jsRuntime == null) return;
            await jsRuntime.InvokeVoidAsync(identifier, args);
        }

        public static async Task<T> SafeInvokeAsync<T>(this IJSRuntime jsRuntime, string identifier, params object[] args)
        {
            if (jsRuntime == null)
                return default;
            return await jsRuntime.InvokeAsync<T>(identifier, args);
        }
    }
}
