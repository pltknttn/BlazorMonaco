using BlazorMonaco;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var host = builder.Build();

// Configure Monaco language workers (Wasm client part of the hybrid app).
await Global.SetDefaultWorkerUrl(
    host.Services.GetRequiredService<IJSRuntime>(),
    "_content/BlazorMonaco/lib/monaco-editor/min"
);

await host.RunAsync();
