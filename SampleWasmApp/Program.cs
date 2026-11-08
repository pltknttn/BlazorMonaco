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

var strings = BlazorMonaco.Resources.MonacoResourceLoader.GetStrings(
    System.Globalization.CultureInfo.CurrentUICulture);
MonacoLocalization.SetStrings(strings);
await Global.RegisterLocalizationStrings(host.Services.GetRequiredService<IJSRuntime>());

// To switch language at runtime:
//   MonacoLocalization.SetCurrentCulture(new CultureInfo("ru-RU"));  // see app code
//   var strings2 = new Dictionary<string, string>();
//   foreach (var localized in localizer.GetAllStrings())
//       strings2[localized.Name] = localized.Value ?? localized.Name;
//   MonacoLocalization.SetStrings(strings2);
//   await Global.RegisterLocalizationStrings(host.Services.GetRequiredService<IJSRuntime>());

// --- Manual override example (Russian) ----------------------------------------
// MonacoLocalization.SetStrings(new Dictionary<string, string>
// {
//     ["editor.emptySelectionClipboard"] = "Копировать пустую выделенную область",
//     ["editor.findMatches"] = "Найдено совпадений: {0} из {1}",
//     ["editor.wordWrap"] = "Перенос по словам",
// });
// await Global.RegisterLocalizationStrings(host.Services.GetRequiredService<IJSRuntime>());

// If you need dynamic per-key lookup (strings added after startup), use instead:
//   await Global.RegisterLocalization(host.Services.GetRequiredService<IJSRuntime>());
// ------------------------------------------------------------------------------

await host.RunAsync();
