using System.Collections.Generic;

namespace BlazorMonaco.Worker
{
    /// <summary>
    /// A mirror of a text model inside a web worker.
    /// Mirrors Monaco's <c>monaco.worker.IMirrorTextModel</c>.
    /// </summary>
    public interface IMirrorTextModel
    {
        /// <summary>
        /// The version id of the mirrored model.
        /// </summary>
        int Version { get; }
    }

    /// <summary>
    /// A mirror of a text model inside a web worker.
    /// Mirrors Monaco's <c>monaco.worker.IMirrorModel</c>.
    /// </summary>
    public interface IMirrorModel : IMirrorTextModel
    {
        /// <summary>
        /// The URI of the mirrored model.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Gets the current value of the mirrored model.
        /// </summary>
        string GetValue();
    }

    /// <summary>
    /// The context that Monaco passes to a web worker factory.
    /// Mirrors Monaco's <c>monaco.worker.IWorkerContext&lt;H&gt;</c>.
    /// </summary>
    /// <typeparam name="THost">The type of the host object available in the worker.</typeparam>
    public interface IWorkerContext<out THost>
    {
        /// <summary>
        /// A proxy to the host object on the main thread.
        /// </summary>
        THost Host { get; }

        /// <summary>
        /// Gets all models that are mirrored into this worker.
        /// </summary>
        IReadOnlyList<IMirrorModel> GetMirrorModels();
    }

    /// <summary>
    /// Non-generic convenience interface for a worker context without a typed host.
    /// </summary>
    public interface IWorkerContext : IWorkerContext<object>
    {
    }
}
