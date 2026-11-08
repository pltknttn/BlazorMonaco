using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorMonaco;
using BlazorMonaco.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Range = BlazorMonaco.Range;
using LangGlobal = BlazorMonaco.Languages.Global;
using EditorGlobal = BlazorMonaco.Editor.Global;

namespace SampleWasmApp.Pages;

public partial class DiffEditor
{
    private const string OriginalUri = "demo://diff/original.js";
    private const string ModifiedUri = "demo://diff/modified.js";

    private const string OriginalValue = """
        "use strict";
        function Person(age) {
        	if (age) {
        		this.age = age;
        	}
        }
        Person.prototype.getAge = function () {
        	return this.age;
        };
        """;

    private const string ModifiedValue = """
        "don't use strict";
        function Person(age_is_just_a_number) {
        	if (age_is_just_a_number) {
        		this.age_is_just_a_number = age_is_just_a_number;
        	}
        }
        Person.prototype.getAge = function () {
        	return this.age_is_just_a_number;
        };
        //Really, it is just a number people!
        """;

    private MonacoDiffEditor? _diffEditor;
    private string _valueToSetOriginal = "";
    private string _valueToSetModified = "";
    private string? _decorationId;
    private bool _readOnly;
    private readonly List<LogLine> _logLines = new();

    private static StandaloneDiffEditorConstructionOptions DiffEditorConstructionOptions(StandaloneDiffEditor editor)
    {
        return new()
        {
            OriginalEditable = true,
            AutomaticLayout = true,
            RenderSideBySide = true,
            RenderWhitespace = "all",
            IgnoreTrimWhitespace = false,
            LineNumbers = "on",
            StickyScroll = new EditorStickyScrollOptions { Enabled = true }
        };
    }

    private async Task EditorOnDidInit()
    {
        Log("init", "diff editor initialized (OnDidInit).");

        var original = await EditorGlobal.GetModel(jsRuntime, OriginalUri);
        if (original == null)
            original = await EditorGlobal.CreateModel(jsRuntime, OriginalValue, "javascript", OriginalUri);

        var modified = await EditorGlobal.GetModel(jsRuntime, ModifiedUri);
        if (modified == null)
            modified = await EditorGlobal.CreateModel(jsRuntime, ModifiedValue, "javascript", ModifiedUri);

        if (_diffEditor == null)
            return;
        await _diffEditor.SetModel(new DiffEditorModel { Original = original, Modified = modified });
        Log("model", "attached shared original & modified models.");
    }

    private void OnDidUpdateDiff()
    {
        Log("diff", "diff recomputed (OnDidUpdateDiff).");
    }

    private void OnDidChangeModel()
    {
        Log("model", "diff model changed (OnDidChangeModel).");
    }

    private void OnDidChangeCursorPositionModified(CursorPositionChangedEvent e)
    {
        if (e.Position != null)
            Log("cursor", $"modified cursor at line {e.Position.LineNumber}, col {e.Position.Column}.");
    }

    private void OnKeyUpModified(KeyboardEvent e)
    {
        Log("key", $"key up: {e.Code}.");
    }

    private async Task SetValueOriginal()
    {
        if (_diffEditor == null)
            return;
        Log("action", $"setting original value: {Shorten(_valueToSetOriginal)}");
        await _diffEditor.OriginalEditor.SetValue(_valueToSetOriginal);
    }

    private async Task SetValueModified()
    {
        if (_diffEditor == null)
            return;
        Log("action", $"setting modified value: {Shorten(_valueToSetModified)}");
        await _diffEditor.ModifiedEditor.SetValue(_valueToSetModified);
    }

    private async Task GetValueOriginal()
    {
        if (_diffEditor == null)
            return;
        var value = await _diffEditor.OriginalEditor.GetValue();
        Log("action", $"original value: {Shorten(value)}");
    }

    private async Task GetValueModified()
    {
        if (_diffEditor == null)
            return;
        var value = await _diffEditor.ModifiedEditor.GetValue();
        Log("action", $"modified value: {Shorten(value)}");
    }

    private async Task RefreshFromModels()
    {
        if (_diffEditor == null)
            return;
        var original = await EditorGlobal.GetModel(jsRuntime, OriginalUri);
        var modified = await EditorGlobal.GetModel(jsRuntime, ModifiedUri);
        if (original == null || modified == null)
        {
            Log("model", "models not found; refresh failed.");
            return;
        }
        await _diffEditor.SetModel(new DiffEditorModel { Original = original, Modified = modified });
        Log("model", "re-attached the original & modified models from global registry.");
    }

    private async Task RevealFirstDiff()
    {
        if (_diffEditor == null)
            return;
        await _diffEditor.RevealFirstDiff();
        Log("diff", "scrolled to the first diff (RevealFirstDiff).");
    }

    private async Task NextDiff()
    {
        if (_diffEditor == null)
            return;
        await _diffEditor.GoToDiff("next");
        Log("diff", "jumped to the next diff (GoToDiff next).");
    }

    private async Task PreviousDiff()
    {
        if (_diffEditor == null)
            return;
        await _diffEditor.GoToDiff("previous");
        Log("diff", "jumped to the previous diff (GoToDiff previous).");
    }

    private async Task GetDiffModel()
    {
        if (_diffEditor == null)
            return;
        var model = await _diffEditor.GetModel();
        if (model == null)
        {
            Log("model", "no diff model attached.");
            return;
        }
        Log("model", $"original uri={model.Original?.Uri}, modified uri={model.Modified?.Uri}.");
    }

    private async Task AddDecorationModified()
    {
        if (_diffEditor == null)
            return;
        var ids = await _diffEditor.ModifiedEditor.DeltaDecorations(null, new[]
        {
            new ModelDeltaDecoration
            {
                Range = new Range(2, 1, 2, 1),
                Options = new ModelDecorationOptions
                {
                    IsWholeLine = true,
                    ClassName = "demo-decoration-line",
                    GlyphMarginClassName = "demo-decoration-glyph"
                }
            }
        });
        _decorationId = ids.FirstOrDefault();
        Log("decoration", $"added whole-line decoration to modified side ({_decorationId}).");
    }

    private async Task RemoveDecorationModified()
    {
        if (_diffEditor == null)
            return;
        if (!string.IsNullOrEmpty(_decorationId))
        {
            await _diffEditor.ModifiedEditor.RemoveDecorations([_decorationId]);
            _decorationId = null;
            Log("decoration", "removed modified-side decoration.");
        }
        else
        {
            Log("decoration", "no decoration to remove.");
        }
    }

    private async Task AddMarkerModified()
    {
        if (_diffEditor == null)
            return;
        var model = await _diffEditor.ModifiedEditor.GetModel();
        if (model == null)
            return;
        var markers = new List<MarkerData>
        {
            new()
            {
                Message = "Typo: 'furction' -> 'function'",
                Severity = MarkerSeverity.Error,
                StartLineNumber = 2,
                StartColumn = 9,
                EndLineNumber = 2,
                EndColumn = 17,
                CodeAsString = "demo"
            }
        };
        await EditorGlobal.SetModelMarkers(jsRuntime, model, "demo-diff", markers);
        Log("marker", "added an error marker on the modified side (line 2).");
    }

    private async Task ClearMarker()
    {
        await EditorGlobal.RemoveAllMarkers(jsRuntime, "demo-diff");
        Log("marker", "cleared diff markers.");
    }

    private async Task AddCommand()
    {
        if (_diffEditor == null)
            return;
        await _diffEditor.AddCommand((int)KeyMod.CtrlCmd | (int)KeyCode.Enter, args =>
        {
            Log("command", "Ctrl+Enter ran the diff command.");
        });
        Log("action", "added diff command (Ctrl+Enter).");
    }

    private async Task AddAction()
    {
        if (_diffEditor == null)
            return;
        var action = new ActionDescriptor
        {
            Id = "demoDiffAction",
            Label = "Demo Diff Action (Ctrl+B)",
            Keybindings = [(int)KeyMod.CtrlCmd | (int)KeyCode.KeyB],
            ContextMenuGroupId = "navigation",
            ContextMenuOrder = 1.5f,
            Run = editor =>
            {
                Log("action", $"Ctrl+B ran the diff action on editor {editor.Id}.");
            }
        };
        await _diffEditor.AddAction(action);
        Log("action", "added diff action 'demoDiffAction'.");
    }

    private async Task FormatModified()
    {
        if (_diffEditor == null)
            return;
        await _diffEditor.ModifiedEditor.Trigger("toolbar", "editor.action.formatDocument", null);
        Log("action", "formatted modified editor.");
    }

    private async Task SetReadOnly()
    {
        if (_diffEditor == null)
            return;
        var readOnly = _readOnly = !_readOnly;
        await _diffEditor.SetReadOnlyAsync(readOnly);
        Log("action", $"set diff read-only to {readOnly}.");
    }

    private void ClearLog() => _logLines.Clear();

    private void Log(string kind, string message)
    {
        _logLines.Insert(0, new LogLine(DateTime.Now, kind, message));
        if (_logLines.Count > 120)
            _logLines.RemoveRange(120, _logLines.Count - 120);
    }

    private static string Shorten(string value)
    {
        var single = value.Replace("\r\n", " ").Replace('\n', ' ');
        return single.Length <= 40 ? single : single[..40] + "…";
    }

    private sealed class LogLine
    {
        public LogLine(DateTime time, string kind, string message)
        {
            Time = time.ToString("HH:mm:ss.fff");
            Kind = kind;
            Message = message;
        }

        public string Time { get; }
        public string Kind { get; }
        public string Message { get; }
    }
}
