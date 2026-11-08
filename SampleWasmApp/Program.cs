using BlazorMonaco;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SampleWasmApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// --- Monaco environment setup -------------------------------------------------
// Point Monaco at its language workers (json, css, html, typescript, ...).
// Must be called before the first editor is created.
await Global.SetDefaultWorkerUrl(
    host.Services.GetRequiredService<IJSRuntime>(),
    "_content/BlazorMonaco/lib/monaco-editor/min"
);

// --- Localization setup -------------------------------------------------------
// Populate localized strings, then register the C# reference with Monaco.
// Example (Russian):
MonacoLocalization.SetStrings(new Dictionary<string, string>
{
    ["editor.emptySelectionClipboard"] = "Копировать пустую выделенную область",
    ["editor.findMatches"] = "Найдено совпадений: {0} из {1}",
    ["editor.wordWrap"] = "Перенос по словам",
});
await Global.RegisterLocalization(host.Services.GetRequiredService<IJSRuntime>());
// ------------------------------------------------------------------------------

await host.RunAsync();
