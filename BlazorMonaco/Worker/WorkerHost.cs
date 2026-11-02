using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorMonaco.Editor;

namespace BlazorMonaco.Worker
{
    /// <summary>
    /// A C# host object that can be called from a Monaco web worker.
    /// </summary>
    /// <remarks>
    /// Pass an instance in <see cref="WebWorkerOptions.Host"/>. Monaco exposes it to the
    /// worker as <c>context.host</c>. Calls from JavaScript are routed to
    /// <see cref="HandleAsync"/> through a JS proxy, so no JavaScript host object has to be written.
    /// </remarks>
    public abstract class WorkerHost : IDisposable
    {
        private DotNetObjectReference<WorkerHost> _reference;
        private bool _disposed;

        /// <summary>
        /// The .NET reference that is passed to JavaScript.
        /// </summary>
        internal DotNetObjectReference<WorkerHost> Reference
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(WorkerHost));
                if (_reference == null)
                {
                    _reference = DotNetObjectReference.Create(this);
                }
                return _reference;
            }
        }

        /// <summary>
        /// Called by JavaScript for every <c>context.host.method(...)</c> invocation in the worker.
        /// </summary>
        [JSInvokable]
        public Task<object> InvokeAsync(string method, JsonElement[] arguments)
            => HandleAsync(method, arguments ?? Array.Empty<JsonElement>());

        /// <summary>
        /// Handles a call from the worker. Override this method to expose methods to the worker.
        /// </summary>
        /// <param name="method">The name of the called method.</param>
        /// <param name="arguments">The JSON arguments passed by the worker.</param>
        /// <returns>A JSON-serializable result.</returns>
        protected virtual Task<object> HandleAsync(string method, JsonElement[] arguments)
            => Task.FromResult<object>(null);

        /// <summary>
        /// Gets a string argument or <c>null</c> when it is missing.
        /// </summary>
        protected static string GetStringArgument(JsonElement[] arguments, int index)
        {
            if (arguments == null || index < 0 || index >= arguments.Length)
                return null;
            return arguments[index].ValueKind == JsonValueKind.String
                ? arguments[index].GetString()
                : arguments[index].ToString();
        }

        /// <summary>
        /// Deserializes an argument to <typeparamref name="T"/>.
        /// </summary>
        protected static T GetArgument<T>(JsonElement[] arguments, int index, JsonSerializerOptions options = null)
        {
            if (arguments == null || index < 0 || index >= arguments.Length)
                return default;
            return JsonSerializer.Deserialize<T>(arguments[index].GetRawText(), options);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _reference?.Dispose();
            _reference = null;
            GC.SuppressFinalize(this);
        }
    }
}
