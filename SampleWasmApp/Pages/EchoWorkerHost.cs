using BlazorMonaco.Worker;
using System.Text.Json;
using System.Threading.Tasks;

namespace SampleWasmApp.Pages;

/// <summary>
/// The main-thread host that is exposed to the web worker as <c>context.host</c>.
/// </summary>
public sealed class EchoWorkerHost : WorkerHost
{
    private readonly Action<string> _log;

    public EchoWorkerHost(Action<string> log)
    {
        _log = log;
    }

    protected override Task<object> HandleAsync(string method, JsonElement[] arguments)
    {
        _log($"Worker called host.{method} with {arguments.Length} argument(s).");

        return method switch
        {
            "getHostInfo" => Task.FromResult<object>("BlazorMonaco WorkerHost (C#)"),
            _ => Task.FromResult<object>($"Unknown host method: {method}")
        };
    }
}
