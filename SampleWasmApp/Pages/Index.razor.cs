using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorMonaco;
using BlazorMonaco.Editor;
using BlazorMonaco.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Range = BlazorMonaco.Range;
using LangGlobal = BlazorMonaco.Languages.Global;
using EditorGlobal = BlazorMonaco.Editor.Global;

namespace SampleWasmApp.Pages;

public partial class Index
{
    private const string DefaultValue = """
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

    private const string ModelUri2 = "demo://person.js";

    private MonacoCodeEditor? _editor;
    private TextModel? _secondModel;
    private string _theme = "vs";
    private string _valueToSet = "";
    private string _newModelValue = "const hero = { name: 'Blazor', language: 'C#/JS' };\nconsole.log(hero);\n";
    private int _navLine = 5;
    private string[]? _decorationIds;
    private string? _markerOwner;         // set once so quick-fix sees our markers
    private readonly List<LogLine> _logLines = new();
    private bool _hoverRegistered;
    private bool _completionRegistered;
    private bool _codeActionRegistered;
    private bool _formattingRegistered;

    private static StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new()
        {
            Language = "javascript",
            GlyphMargin = true,
            AutomaticLayout = true,
            Value = DefaultValue + "\n// Place the caret on a word, then press Ctrl+Space, hover, or Ctrl+.",
            WrappingIndent = "indent",
            WordWrap = "on",
            BracketPairColorization = new BracketPairColorizationOptions { Enabled = true },
            StickyScroll = new EditorStickyScrollOptions { Enabled = true }
        };
    }



    private async Task EditorOnDidInit()
    {
        Log("editor", "initialized (OnDidInit)");

        // Built-in command: Ctrl+H opens the accessibility help -> instead log a custom hint.
        await _editor!.AddCommand((int)KeyMod.CtrlCmd | (int)KeyCode.KeyH, args =>
        {
            Log("command", "Ctrl+H triggered the editor command.");
        });

        // Seed a whole-line decoration right away.
        _decorationIds = await _editor.DeltaDecorations(null, new[]
        {
            new ModelDeltaDecoration
            {
                Range = new Range(3, 1, 3, 1),
                Options = new ModelDecorationOptions
                {
                    IsWholeLine = true,
                    ClassName = "demo-decoration-line",
                    GlyphMarginClassName = "demo-decoration-glyph",
                    GlyphMarginHoverMessage = [new MarkdownString { Value = "Seeded decoration (line 3)", SupportThemeIcons = false }]
                }
            }
        });
        Log("decoration", $"seeded {_decorationIds.Length} whole-line decoration(s).");
    }

    private void OnDidChangeCursorPosition(CursorPositionChangedEvent e)
    {
        if (e.Position == null)
            return;
        Log("cursor", $"line {e.Position.LineNumber}, column {e.Position.Column}");
    }

    private void OnDidChangeModelContent(ModelContentChangedEvent e)
    {
        Log("content", $"model changed ({e.Changes.Count} change(s), version {e.VersionId}).");
    }

    private void OnContextMenu(EditorMouseEvent e)
    {
        var line = e.Target?.Position?.LineNumber.ToString() ?? "?";
        Log("contextmenu", $"right-click at line {line}.");
    }

    private async Task SetValue()
    {
        if (_editor == null)
            return;
        Log("action", $"setting value to: {Shorten(_valueToSet)}");
        await _editor.SetValue(_valueToSet);
    }

    private async Task GetValue()
    {
        if (_editor == null)
            return;
        var value = await _editor.GetValue();
        Log("action", $"value is: {Shorten(value)}");
    }

    private async Task ResetValue()
    {
        if (_editor == null)
            return;
        await _editor.SetValue(DefaultValue);
        Log("action", "value reset to the sample.");
    }

    private async Task CreateAndSwitchModel()
    {
        if (_editor == null)
            return;

        if (_secondModel != null)
            await _secondModel.DisposeModel();

        _secondModel = await EditorGlobal.CreateModel(jsRuntime, _newModelValue, "javascript", ModelUri2);
        Log("model", $"created shared model '{ModelUri2}'.");
        await _editor.SetModel(_secondModel);
        Log("model", "switched editor model to the shared model.");
    }

    private async Task ModelInfo()
    {
        if (_editor == null)
            return;
        var model = await _editor.GetModel();
        if (model == null)
        {
            Log("model", "no model attached.");
            return;
        }
        Log("model", $"uri={model.Uri}, language={await model.GetLanguageId()}, lines={await model.GetLineCount()}, version={await model.GetVersionId()}.");
    }

    private async Task SwitchBack()
    {
        if (_editor == null)
            return;
        var model = await EditorGlobal.GetModel(jsRuntime, "inmemory://model/1");
        if (model == null)
        {
            Log("model", "default model not found.");
            return;
        }
        await _editor.SetModel(model);
        Log("model", "switched back to the default model.");
    }

    private async Task RevealLine()
    {
        if (_editor == null)
            return;
        await _editor.RevealLine(_navLine, ScrollType.Smooth);
        Log("navigation", $"revealed line {_navLine}.");
    }

    private async Task SelectWord()
    {
        if (_editor == null)
            return;
        var position = await _editor.GetPosition();
        if (position == null)
        {
            Log("selection", "no caret position.");
            return;
        }
        var model = await _editor.GetModel();
        if (model == null)
            return;
        var word = await model.GetWordAtPosition(position);
        if (word == null)
        {
            Log("selection", "no word at caret.");
            return;
        }
        await _editor.SetSelection(new Selection
        {
            SelectionStartLineNumber = position.LineNumber,
            SelectionStartColumn = word.StartColumn,
            PositionLineNumber = position.LineNumber,
            PositionColumn = word.EndColumn,
            StartLineNumber = position.LineNumber,
            StartColumn = word.StartColumn,
            EndLineNumber = position.LineNumber,
            EndColumn = word.EndColumn
        }, "demo");
        Log("selection", $"selected word '{word.Word}' at line {position.LineNumber}.");
    }

    private async Task GetSelection()
    {
        if (_editor == null)
            return;
        var selection = await _editor.GetSelection();
        if (selection == null)
        {
            Log("selection", "no selection.");
            return;
        }
        Log("selection", $"selection {selection.StartLineNumber}:{selection.StartColumn}-{selection.EndLineNumber}:{selection.EndColumn}.");
    }

    private async Task FocusEditor()
    {
        if (_editor == null)
            return;
        await _editor.Focus();
        Log("focus", "editor focused.");
    }

    private async Task AddDecorations()
    {
        if (_editor == null)
            return;
        _decorationIds = await _editor.DeltaDecorations(_decorationIds, new[]
        {
            new ModelDeltaDecoration
            {
                Range = new Range(7, 1, 7, 1),
                Options = new ModelDecorationOptions
                {
                    IsWholeLine = true,
                    ClassName = "demo-decoration-line",
                    GlyphMarginClassName = "demo-decoration-glyph"
                }
            }
        });
        Log("decoration", $"added whole-line decoration on line 7 ({_decorationIds.Length} active).");
    }

    private async Task AddInlineDecoration()
    {
        if (_editor == null)
            return;
        var model = await _editor.GetModel();
        if (model == null)
            return;
        var line = 1;
        var word = await model.GetWordAtPosition(new Position { LineNumber = line, Column = 2 });
        if (word != null)
        {
            await model.DeltaDecorations(null, new List<ModelDeltaDecoration>
            {
                new()
                {
                    Range = new Range(line, word.StartColumn, line, word.EndColumn),
                    Options = new ModelDecorationOptions
                    {
                        ClassName = "demo-decoration-inline",
                        HoverMessage = [new MarkdownString { Value = "Inline decoration over a word", SupportThemeIcons = false }]
                    }
                }
            }, 0);
            Log("decoration", $"inline decoration spanning '{word.Word}' on line {line}.");
        }
        else
        {
            Log("decoration", "no word found for inline decoration.");
        }
    }

    private async Task RemoveDecorations()
    {
        if (_editor == null)
            return;
        await _editor.ResetDeltaDecorations();
        _decorationIds = null;
        Log("decoration", "removed all editor decorations (ResetDeltaDecorations).");
    }

    private async Task AddMarker()
    {
        if (_editor == null)
            return;
        var model = await _editor.GetModel();
        if (model == null)
            return;

        var markers = new List<MarkerData>
        {
            new()
            {
                Message = "Uppercase style: 'this' should be 'THIS'",
                Severity = MarkerSeverity.Warning,
                StartLineNumber = 4,
                StartColumn = 3,
                EndLineNumber = 4,
                EndColumn = 7,
                CodeAsObject = new MarkerCode { TargetUri = "https://microsoft.github.io/monaco-editor/", Value = "demo-code" }
            }
        };
        await EditorGlobal.SetModelMarkers(jsRuntime, model, "demo", markers);
        _markerOwner = "demo";
        Log("marker", "added an error marker on line 4 (use Ctrl+. / quick-fix).");
    }

    private async Task ClearMarkers()
    {
        await EditorGlobal.RemoveAllMarkers(jsRuntime, _markerOwner ?? "demo");
        _markerOwner = null;
        Log("marker", "cleared demo markers (RemoveAllMarkers).");
    }

    private async Task RegisterHoverProvider()
    {
        if (_hoverRegistered)
        {
            Log("provider", "hover provider already registered.");
            return;
        }

        await LangGlobal.RegisterHoverProviderAsync(jsRuntime, "javascript", async (uri, position, context) =>
        {
            var model = await EditorGlobal.GetModel(jsRuntime, uri);
            if (model == null)
                return null;

            var word = await model.GetWordAtPosition(position);
            if (word == null)
                return null;

            return new Hover
            {
                Contents =
                [
                    new MarkdownString { Value = "**BlazorMonaco hover**", SupportThemeIcons = false },
                    new MarkdownString { Value = $"Word: `{word.Word}`", SupportThemeIcons = false }
                ],
                Range = new Range
                {
                    StartLineNumber = position.LineNumber,
                    EndLineNumber = position.LineNumber,
                    StartColumn = word.StartColumn,
                    EndColumn = word.EndColumn
                }
            };
        });

        _hoverRegistered = true;
        Log("provider", "registered hover provider for 'javascript' (hover a word).");
    }

    private async Task RegisterCompletionProvider()
    {
        if (_completionRegistered)
        {
            Log("provider", "completion provider already registered.");
            return;
        }

        await LangGlobal.RegisterCompletionItemProvider(jsRuntime, "javascript", (modelUri, position, context) =>
        {
            var suggestions = new List<CompletionItem>
            {
                new()
                {
                    LabelAsString = "console.log",
                    Kind = CompletionItemKind.Function,
                    Detail = "logs to the console",
                    InsertText = "console.log(${1:value});",
                    SortText = "a",
                    Preselect = true
                },
                new()
                {
                    LabelAsString = "blazorMonaco",
                    Kind = CompletionItemKind.Variable,
                    Detail = "this -> THIS example",
                    InsertText = "THIS",
                    SortText = "b"
                }
            };
            return Task.FromResult(new CompletionList { Suggestions = suggestions });
        });

        _completionRegistered = true;
        Log("provider", "registered completion provider (Ctrl+Space).");
    }

    private async Task RegisterCodeActionProvider()
    {
        if (_codeActionRegistered)
        {
            Log("provider", "code action provider already registered.");
            return;
        }

        // Ensure a marker exists for the quick-fix to act on.
        await AddMarker();

        await LangGlobal.RegisterCodeActionProvider(jsRuntime, "javascript", async (modelUri, range, context) =>
        {
            var innerModel = await EditorGlobal.GetModel(jsRuntime, modelUri);

            var actions = new CodeActionList();
            if (context?.Markers.Count > 0)
            {
                actions.Actions =
                [
                    new CodeAction
                    {
                        Title = "Fix 'this' to 'THIS'",
                        Kind = "quickfix",
                        Diagnostics = context.Markers,
                        IsPreferred = true,
                        Edit = new WorkspaceEdit
                        {
                            Edits =
                            [
                                new WorkspaceTextEdit
                                {
                                    ResourceUri = modelUri,
                                    TextEdit = new TextEditWithOptions
                                    {
                                        Range = range,
                                        Text = "THIS"
                                    }
                                }
                            ]
                        }
                    }
                ];
            }
            return actions;
        });

        _codeActionRegistered = true;
        Log("provider", "registered quick-fix provider (Ctrl+.).");
    }

    private async Task RegisterFormattingProvider()
    {
        if (_formattingRegistered)
        {
            Log("provider", "formatting provider already registered.");
            return;
        }

        await LangGlobal.RegisterDocumentFormattingEditProvider(jsRuntime, "javascript", async (modelUri, options) =>
        {
            if (_editor == null)
                return [];

            var model = await _editor.GetModel();
            if (model == null)
                return [];

            var lines = await model.GetLineCount();
            var columns = await model.GetLineMaxColumn(lines);
            var value = await _editor.GetValue();
            var formatted = value.Split('\n').Select(line =>
            {
                // A very simple "formatter": collapse deep indentation to one tab.
                var trimmed = line.TrimStart('\t', ' ');
                var indent = line.Length - (line.Length - trimmed.Length);
                return new string('\t', Math.Min(indent, 1)) + trimmed;
            });

            return
            [
                new TextEdit
                {
                    Range = new Range(1, 1, lines, columns),
                    Text = string.Join('\n', formatted)
                }
            ];
        });

        _formattingRegistered = true;
        Log("provider", "registered formatting provider (Shift+Alt+F).");
    }

    private async Task TokenizeSample()
    {
        var tokens = await EditorGlobal.Tokenize(jsRuntime, "var x = 1; // comment", "javascript");
        var firstLine = tokens.FirstOrDefault();
        var types = firstLine?.Take(4).Select(t => t.Type) ?? Enumerable.Empty<string>();
        Log("global", $"tokenize -> {tokens.Count} line(s); first line: {string.Join(", ", types)}.");
    }

    private async Task ColorizeSample()
    {
        var html = await EditorGlobal.Colorize(jsRuntime, "function hello() { return 1; }", "javascript", new ColorizerOptions());
        Log("global", $"colorize -> {Shorten(html)}");
    }

    private async Task AddCommand()
    {
        if (_editor == null)
            return;
        await _editor.AddCommand((int)KeyMod.CtrlCmd | (int)KeyCode.Enter, args =>
        {
            Log("command", "Ctrl+Enter executed the editor command.");
        });
        Log("action", "added command (Ctrl+Enter).");
    }

    private async Task AddAction()
    {
        if (_editor == null)
            return;
        var actionDescriptor = new ActionDescriptor
        {
            Id = "demoAction",
            Label = "Demo Action (Ctrl+B)",
            Keybindings = [(int)KeyMod.CtrlCmd | (int)KeyCode.KeyB],
            ContextMenuGroupId = "navigation",
            ContextMenuOrder = 1.5f,
            Run = editor =>
            {
                Log("action", "Ctrl+B executed the editor action (context menu too).");
            }
        };
        await _editor.AddAction(actionDescriptor);
        Log("action", "added action 'demoAction' (Ctrl+B + context menu).");
    }

    private async Task DefineTheme()
    {
        await EditorGlobal.DefineTheme(jsRuntime, "demo-synth", new StandaloneThemeData
        {
            Base = "vs-dark",
            Inherit = true,
            Rules =
            [
                new TokenThemeRule { Token = "comment", Foreground = "6A9955", FontStyle = "italic" },
                new TokenThemeRule { Token = "keyword", Foreground = "569CD6" },
                new TokenThemeRule { Token = "string", Foreground = "CE9178" }
            ],
            Colors = new Dictionary<string, string>
            {
                ["editor.background"] = "#1E1B3A",
                ["editorLineNumber.foreground"] = "#8B87B8"
            }
        });
        await EditorGlobal.SetTheme(jsRuntime, "demo-synth");
        _theme = "demo-synth";
        Log("theme", "defined and applied custom theme 'demo-synth'.");
    }

    private async Task ApplyBuiltinTheme()
    {
        _theme = "vs";
        await EditorGlobal.SetTheme(jsRuntime, _theme);
        Log("theme", "reset theme to 'vs'.");
    }

    private async Task ListEditors()
    {
        var ids = await EditorGlobal.GetEditors(jsRuntime);
        var diffs = await EditorGlobal.GetDiffEditors(jsRuntime);
        Log("global", $"editors: {string.Join(", ", ids ?? Enumerable.Empty<string>())}");
        if (diffs?.Count > 0)
            Log("global", $"diff editors: {string.Join(", ", diffs)}");
    }

    private async Task ListModels()
    {
        var models = await EditorGlobal.GetModels(jsRuntime);
        Log("global", $"{models.Count} model(s): {string.Join(", ", models.Select(m => m.Uri))}");
    }

    private async Task GetMarkers()
    {
        var markers = await EditorGlobal.GetModelMarkers(jsRuntime, new EditorGlobal.GetModelMarkersFilter { Owner = _markerOwner ?? "demo" });
        Log("marker", $"markers for owner '{_markerOwner ?? "demo"}': {markers.Count} found.");
    }

    private async Task RemeasureFonts()
    {
        await EditorGlobal.RemeasureFonts(jsRuntime);
        Log("global", "font measurements cleared (RemeasureFonts).");
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
