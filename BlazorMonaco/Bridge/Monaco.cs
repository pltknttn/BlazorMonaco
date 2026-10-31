using BlazorMonaco.Editor;
using BlazorMonaco.Helpers;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace BlazorMonaco
{
    public class Environment
    {
        /**
         * Define a global `monaco` symbol.
         * This is true by default in AMD and false by default in ESM.
         */
        public bool? GlobalAPI { get; set; }
        /**
         * The base url where the editor sources are found (which contains the vs folder)
         */
        public string BaseUrl { get; set; }
    }

    public enum MarkerTag
    {
        Unnecessary = 1,
        Deprecated = 2
    }

    public enum MarkerSeverity
    {
        Hint = 1,
        Info = 2,
        Warning = 4,
        Error = 8
    }

    /**
     * Uniform Resource Identifier (Uri) http://tools.ietf.org/html/rfc3986.
     * This class is a simple parser which creates the basic component parts
     * (http://tools.ietf.org/html/rfc3986#section-3) with minimal validation
     * and encoding.
     */
    public class UriComponents
    {
        public string Scheme { get; set; }
        public string Authority { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }
        public string Fragment { get; set; }
    }

    /**
     * Virtual Key Codes, the value does not hold any inherent meaning.
     * Inspired somewhat from https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
     * But these are "more general", as they should work across browsers & OS`s.
     */
    public enum KeyCode
    {
        DependsOnKbLayout = -1,
        /**
         * Placed first to cover the 0 value of the enum.
         */
        Unknown = 0,
        Backspace = 1,
        Tab = 2,
        Enter = 3,
        Shift = 4,
        Ctrl = 5,
        Alt = 6,
        PauseBreak = 7,
        CapsLock = 8,
        Escape = 9,
        Space = 10,
        PageUp = 11,
        PageDown = 12,
        End = 13,
        Home = 14,
        LeftArrow = 15,
        UpArrow = 16,
        RightArrow = 17,
        DownArrow = 18,
        Insert = 19,
        Delete = 20,
        Digit0 = 21,
        Digit1 = 22,
        Digit2 = 23,
        Digit3 = 24,
        Digit4 = 25,
        Digit5 = 26,
        Digit6 = 27,
        Digit7 = 28,
        Digit8 = 29,
        Digit9 = 30,
        KeyA = 31,
        KeyB = 32,
        KeyC = 33,
        KeyD = 34,
        KeyE = 35,
        KeyF = 36,
        KeyG = 37,
        KeyH = 38,
        KeyI = 39,
        KeyJ = 40,
        KeyK = 41,
        KeyL = 42,
        KeyM = 43,
        KeyN = 44,
        KeyO = 45,
        KeyP = 46,
        KeyQ = 47,
        KeyR = 48,
        KeyS = 49,
        KeyT = 50,
        KeyU = 51,
        KeyV = 52,
        KeyW = 53,
        KeyX = 54,
        KeyY = 55,
        KeyZ = 56,
        Meta = 57,
        ContextMenu = 58,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        F11 = 69,
        F12 = 70,
        F13 = 71,
        F14 = 72,
        F15 = 73,
        F16 = 74,
        F17 = 75,
        F18 = 76,
        F19 = 77,
        F20 = 78,
        F21 = 79,
        F22 = 80,
        F23 = 81,
        F24 = 82,
        NumLock = 83,
        ScrollLock = 84,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the ';:' key
         */
        Semicolon = 85,
        /**
         * For any country/region, the '+' key
         * For the US standard keyboard, the '=+' key
         */
        Equal = 86,
        /**
         * For any country/region, the ',' key
         * For the US standard keyboard, the ',<' key
         */
        Comma = 87,
        /**
         * For any country/region, the '-' key
         * For the US standard keyboard, the '-_' key
         */
        Minus = 88,
        /**
         * For any country/region, the '.' key
         * For the US standard keyboard, the '.>' key
         */
        Period = 89,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the '/?' key
         */
        Slash = 90,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the '`~' key
         */
        Backquote = 91,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the '[{' key
         */
        BracketLeft = 92,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the '\|' key
         */
        Backslash = 93,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the ']}' key
         */
        BracketRight = 94,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         * For the US standard keyboard, the ''"' key
         */
        Quote = 95,
        /**
         * Used for miscellaneous characters; it can vary by keyboard.
         */
        OEM_8 = 96,
        /**
         * Either the angle bracket key or the backslash key on the RT 102-key keyboard.
         */
        IntlBackslash = 97,
        Numpad0 = 98,// VK_NUMPAD0, 0x60, Numeric keypad 0 key
        Numpad1 = 99,// VK_NUMPAD1, 0x61, Numeric keypad 1 key
        Numpad2 = 100,// VK_NUMPAD2, 0x62, Numeric keypad 2 key
        Numpad3 = 101,// VK_NUMPAD3, 0x63, Numeric keypad 3 key
        Numpad4 = 102,// VK_NUMPAD4, 0x64, Numeric keypad 4 key
        Numpad5 = 103,// VK_NUMPAD5, 0x65, Numeric keypad 5 key
        Numpad6 = 104,// VK_NUMPAD6, 0x66, Numeric keypad 6 key
        Numpad7 = 105,// VK_NUMPAD7, 0x67, Numeric keypad 7 key
        Numpad8 = 106,// VK_NUMPAD8, 0x68, Numeric keypad 8 key
        Numpad9 = 107,// VK_NUMPAD9, 0x69, Numeric keypad 9 key
        NumpadMultiply = 108,// VK_MULTIPLY, 0x6A, Multiply key
        NumpadAdd = 109,// VK_ADD, 0x6B, Add key
        NUMPAD_SEPARATOR = 110,// VK_SEPARATOR, 0x6C, Separator key
        NumpadSubtract = 111,// VK_SUBTRACT, 0x6D, Subtract key
        NumpadDecimal = 112,// VK_DECIMAL, 0x6E, Decimal key
        NumpadDivide = 113,// VK_DIVIDE, 0x6F,
        /**
         * Cover all key codes when IME is processing input.
         */
        KEY_IN_COMPOSITION = 114,
        ABNT_C1 = 115,// Brazilian (ABNT) Keyboard
        ABNT_C2 = 116,// Brazilian (ABNT) Keyboard
        AudioVolumeMute = 117,
        AudioVolumeUp = 118,
        AudioVolumeDown = 119,
        BrowserSearch = 120,
        BrowserHome = 121,
        BrowserBack = 122,
        BrowserForward = 123,
        MediaTrackNext = 124,
        MediaTrackPrevious = 125,
        MediaStop = 126,
        MediaPlayPause = 127,
        LaunchMediaPlayer = 128,
        LaunchMail = 129,
        LaunchApp2 = 130,
        /**
         * VK_CLEAR, 0x0C, CLEAR key
         */
        Clear = 131,
        /**
         * Placed last to cover the length of the enum.
         * Please do not depend on this value!
         */
        MAX_VALUE = 132
    }

    public enum KeyMod
    {
        CtrlCmd = 2048,
        Shift = 1024,
        Alt = 512,
        WinCtrl = 256,
    }

    public class MarkdownString
    {
        public string Value { get; set; }
        public object IsTrusted { get; set; }
        public bool? SupportThemeIcons { get; set; }
        public bool? SupportHtml { get; set; }
        public UriComponents BaseUri { get; set; }
        public Dictionary<string, UriComponents> Uris { get; set; }
    }

    public class MarkdownStringTrustedOptions
    {
        public string[] EnabledCommands { get; set; }
    }

    public class KeyboardEvent
    {
        //readonly _standardKeyboardEventBrand: true;
        public KeyboardEvent BrowserEvent { get; set; }
        public JsonElement? Target { get; set; }
        public bool CtrlKey { get; set; }
        public bool ShiftKey { get; set; }
        public bool AltKey { get; set; }
        public bool MetaKey { get; set; }
        public bool AltGraphKey { get; set; }
        public KeyCode KeyCode { get; set; }
        public string Code { get; set; }
    }

    public class MouseEvent
    {
        public MouseEvent BrowserEvent { get; set; }
        public bool LeftButton { get; set; }
        public bool MiddleButton { get; set; }
        public bool RightButton { get; set; }
        public int Buttons { get; set; }
        public JsonElement? Target { get; set; }
        public int Detail { get; set; }
        public double Posx { get; set; }
        public double Posy { get; set; }
        public bool CtrlKey { get; set; }
        public bool ShiftKey { get; set; }
        public bool AltKey { get; set; }
        public bool MetaKey { get; set; }
        public long Timestamp { get; set; }
        public bool DefaultPrevented { get; set; }
    }

    public class ScrollEvent
    {
        public double ScrollTop { get; set; }
        public double ScrollLeft { get; set; }
        public double ScrollWidth { get; set; }
        public double ScrollHeight { get; set; }
        public bool ScrollTopChanged { get; set; }
        public bool ScrollLeftChanged { get; set; }
        public bool ScrollWidthChanged { get; set; }
        public bool ScrollHeightChanged { get; set; }
    }

    public class ScrolledVisiblePosition
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Height { get; set; }
    }

    /**
     * A position in the editor. This interface is suitable for serialization.
     */
    public class Position
    {
        /**
         * line number (starts at 1)
         */
        public int LineNumber { get; set; }
        /**
         * column (the first character in a line is between column 1 and column 2)
         */
        public int Column { get; set; }
    }

    /**
     * A range in the editor. (startLineNumber,startColumn) is <= (endLineNumber,endColumn)
     */
    public class Range
    {
        /**
         * Line number on which the range starts (starts at 1).
         */
        public int StartLineNumber { get; set; }
        /**
         * Column on which the range starts in line `startLineNumber` (starts at 1).
         */
        public int StartColumn { get; set; }
        /**
         * Line number on which the range ends.
         */
        public int EndLineNumber { get; set; }
        /**
         * Column on which the range ends in line `endLineNumber`.
         */
        public int EndColumn { get; set; }

        public Range() { }

        public Range(int startLineNumber, int startColumn, int endLineNumber, int endColumn)
        {
            StartLineNumber = startLineNumber;
            StartColumn = startColumn;
            EndLineNumber = endLineNumber;
            EndColumn = endColumn;
        }
    }

    /**
     * A selection in the editor.
     * The selection is a range that has an orientation.
     */
    public class Selection : Range
    {
        /**
         * The line number on which the selection has started.
         */
        public int SelectionStartLineNumber { get; set; }
        /**
         * The column on `selectionStartLineNumber` where the selection has started.
         */
        public int SelectionStartColumn { get; set; }
        /**
         * The line number on which the selection has ended.
         */
        public int PositionLineNumber { get; set; }
        /**
         * The column on `positionLineNumber` where the selection has ended.
         */
        public int PositionColumn { get; set; }

    }

    /**
     * The direction of a selection.
     */
    public enum SelectionDirection
    {
        /**
         * The selection starts above where it ends.
         */
        LTR = 0,
        /**
         * The selection starts below where it ends.
         */
        RTL = 1
    }

    public class Token
    {
        public int Offset { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        //_tokenBrand: void;
    }
}

namespace BlazorMonaco.Editor
{

    /**
     * A global keybinding rule.
     */
    public class KeybindingRule
    {
        public int Keybinding { get; set; }
        public string Command { get; set; }
        public object CommandArgs { get; set; }
        public string When { get; set; }
    }

    /**
     * A link opener that can be registered with `Global.RegisterLinkOpener`.
     */
    public interface ILinkOpener
    {
        Task<object> Open(string resource);
    }

    /**
     * Wrapper around `ILinkOpener` so the handler can be invoked from JavaScript.
     */
    public class LinkOpener : ILinkOpener
    {
        public delegate Task<object> OpenDelegate(string resource);
        public OpenDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(OpenLink))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> OpenLink(string resource)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(resource));

        public Task<object> Open(string resource)
            => ProvideMethod?.Invoke(resource) ?? Task.FromResult<object>(null);

        public LinkOpener(OpenDelegate open) => ProvideMethod = open;
    }

    /**
     * A code editor opener that can be registered with `Global.RegisterEditorOpener`.
     */
    public interface ICodeEditorOpener
    {
        Task<object> OpenCodeEditor(string sourceId, string resource, object selectionOrPosition);
    }

    /**
     * Wrapper around `ICodeEditorOpener` so the handler can be invoked from JavaScript.
     */
    public class CodeEditorOpener : ICodeEditorOpener
    {
        public delegate Task<object> OpenCodeEditorDelegate(string sourceId, string resource, object selectionOrPosition);
        public OpenCodeEditorDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(OpenCodeEditorLink))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> OpenCodeEditorLink(string sourceId, string resource, object selectionOrPosition)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(sourceId, resource, selectionOrPosition));

        public Task<object> OpenCodeEditor(string sourceId, string resource, object selectionOrPosition)
            => ProvideMethod?.Invoke(sourceId, resource, selectionOrPosition) ?? Task.FromResult<object>(null);

        public CodeEditorOpener(OpenCodeEditorDelegate openCodeEditor) => ProvideMethod = openCodeEditor;
    }

    /**
     * A collection of decorations tracked by an editor, as returned by `Editor.CreateDecorationsCollection`.
     */
    public class EditorDecorationsCollection
    {
        internal IJSRuntime JsRuntime { get; set; }
        internal string EditorId { get; set; }

        public string Owner { get; set; }

        public int Length { get; set; }

        public List<Range> Ranges { get; set; }

        public Task<List<string>> Set(object newDecorations)
            => JsRuntime.SafeInvokeAsync<List<string>>("blazorMonaco.editor.decorationsSet", EditorId, Owner, newDecorations);

        public Task<List<string>> Append(object newDecorations)
            => JsRuntime.SafeInvokeAsync<List<string>>("blazorMonaco.editor.decorationsAppend", EditorId, Owner, newDecorations);

        public Task Clear()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.decorationsClear", EditorId, Owner);

        public Task<bool> Has(object decoration)
            => JsRuntime.SafeInvokeAsync<bool>("blazorMonaco.editor.decorationsHas", EditorId, Owner, decoration);

        public Task<Range> GetRange(int index)
            => JsRuntime.SafeInvokeAsync<Range>("blazorMonaco.editor.decorationsGetRange", EditorId, Owner, index);

        public Task<List<Range>> GetRanges()
            => JsRuntime.SafeInvokeAsync<List<Range>>("blazorMonaco.editor.decorationsGetRanges", EditorId, Owner);
    }

    /**
     * Static hub for editor/model lifecycle events that have no per-instance .NET callback.
     * JavaScript registers persistent global listeners (see `Global.RegisterLifecycleListeners`)
     * that call back into `OnLifecycleEvent`.
     */
    public class MonacoLifecycle
    {
        private static MonacoLifecycle _instance;
        private static DotNetObjectReference<MonacoLifecycle> _ref;

        public static event Action<string, string> LifecycleEvent;

        [JSInvokable]
        public void OnLifecycleEvent(string eventName, string payloadJson)
            => LifecycleEvent?.Invoke(eventName, payloadJson);

        public static MonacoLifecycle GetInstance()
        {
            if (_instance == null)
                _instance = new MonacoLifecycle();
            return _instance;
        }

        internal static DotNetObjectReference<MonacoLifecycle> GetRef()
        {
            if (_ref == null)
                _ref = DotNetObjectReference.Create(GetInstance());
            return _ref;
        }
    }


    /**
     * A content widget added to an editor.
     */
    public class ContentWidget
    {
        public string Id { get; set; }
        public string DomNodeId { get; set; }
        public bool? AllowEditorOverflow { get; set; }
        public bool? UseDisplayNone { get; set; }
        public bool? SuppressMouseDown { get; set; }
        public ContentWidgetPosition Position { get; set; }
    }

    /**
     * The placement of a content widget.
     */
    public class ContentWidgetPosition
    {
        public Position Position { get; set; }
        public Position SecondaryPosition { get; set; }
        public ContentWidgetPositionPreference[] Preference { get; set; }
    }

    /**
     * An overlay widget added to an editor.
     */
    public class OverlayWidget
    {
        public string Id { get; set; }
        public string DomNodeId { get; set; }
        public bool? AllowEditorOverflow { get; set; }
        public object Position { get; set; }
    }

    /**
     * A glyph margin widget added to an editor.
     */
    public class GlyphMarginWidget
    {
        public string Id { get; set; }
        public string DomNodeId { get; set; }
        public GlyphMarginWidgetPosition Position { get; set; }
    }

    /**
     * The placement of a glyph margin widget.
     */
    public class GlyphMarginWidgetPosition
    {
        public int LineNumber { get; set; }
        public int Column { get; set; }
    }

    /**
     * A view zone added to an editor via `Editor.AddViewZone`.
     */
    public class ViewZone
    {
        public int AfterLineNumber { get; set; }
        public int? AfterColumn { get; set; }
        public bool? ShowInHiddenAreas { get; set; }
        public int? Ordinal { get; set; }
        public bool? SuppressMouseDown { get; set; }
        public int? HeightInLines { get; set; }
        public int? HeightInPx { get; set; }
        public int? MinWidthInPx { get; set; }
        public string DomNodeId { get; set; }
        public string MarginDomNodeId { get; set; }
    }

    /**
     * An accessor used to add/remove/layout view zones. Mirrors
     * `monaco.editor.IViewZoneChangeAccessor`.
     */
    public interface IViewZoneChangeAccessor
    {
        Task<string> AddZone(ViewZone zone);
        Task RemoveZone(string id);
        Task LayoutZone(string id);
    }

    /**
     * Bridge implementation of `IViewZoneChangeAccessor` bound to a specific editor.
     */
    public class ViewZoneChangeAccessor : IViewZoneChangeAccessor
    {
        internal IJSRuntime JsRuntime { get; set; }
        internal string EditorId { get; set; }

        public Task<string> AddZone(ViewZone zone)
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.addViewZone", EditorId, zone);

        public Task RemoveZone(string id)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeViewZone", EditorId, id);

        public Task LayoutZone(string id)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.layoutViewZone", EditorId, id);

        public ViewZoneChangeAccessor() { }

        internal ViewZoneChangeAccessor(IJSRuntime jsRuntime, string editorId)
        {
            JsRuntime = jsRuntime;
            EditorId = editorId;
        }
    }

    public partial class Global
    {
        /**
         * Create a new editor under `domElement`.
         * `domElement` should be empty (not contain other dom nodes).
         * The editor will read the size of `domElement`.
         */
        internal static async Task Create(
            IJSRuntime jsRuntime,
            string domElementId,
            StandaloneEditorConstructionOptions options,
            EditorOverrideServices overrideServices,
            DotNetObjectReference<Editor> dotnetObjectRef)
        {
            options = options ?? new StandaloneEditorConstructionOptions();

            // Convert to JsonElement to remove the properties with null values
            var optionsJson = JsonElementExt.FromObject(options);

            // Create the editor
#if NET5_0_OR_GREATER
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setWasm", OperatingSystem.IsBrowser());
#else
            var isBrowser = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Create("BROWSER"));
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setWasm", isBrowser);
#endif
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.create", domElementId, optionsJson, overrideServices, dotnetObjectRef);
        }


        /**
         * Create a new diff editor under `domElement`.
         * `domElement` should be empty (not contain other dom nodes).
         * The editor will read the size of `domElement`.
         */
        internal static async Task CreateDiffEditor(
            IJSRuntime jsRuntime,
            string domElementId,
            StandaloneDiffEditorConstructionOptions options,
            EditorOverrideServices overrideServices,
            DotNetObjectReference<Editor> dotnetObjectRef,
            DotNetObjectReference<Editor> dotnetObjectRefOriginal,
            DotNetObjectReference<Editor> dotnetObjectRefModified)
        {
            options = options ?? new StandaloneDiffEditorConstructionOptions();

            // Convert to JsonElement to remove the properties with null values
            var optionsJson = JsonElementExt.FromObject(options);

            // Create the editor
#if NET5_0_OR_GREATER
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setWasm", OperatingSystem.IsBrowser());
#else
            var isBrowser = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Create("BROWSER"));
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setWasm", isBrowser);
#endif
            await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.createDiffEditor", domElementId, optionsJson, overrideServices, dotnetObjectRef, dotnetObjectRefOriginal, dotnetObjectRefModified);
        }


        /**
         * Create a new editor model.
         * You can specify the language that should be set for this model or let the language be inferred from the `uri`.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<TextModel> CreateModel(string value, string language = null, string uri = null)
            => CreateModel(null, value, language, uri);

        public static async Task<TextModel> CreateModel(IJSRuntime jsRuntime, string value, string language = null, string uri = null)
        {
            var textModel = await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<TextModel>("blazorMonaco.editor.createModel", value, language, uri);
            if (textModel != null)
                textModel.JsRuntime = JsRuntimeExt.UpdateRuntime(jsRuntime);
            return textModel;
        }

        /**
         * Change the language for a model.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task SetModelLanguage(TextModel model, string languageId)
            => SetModelLanguage(null, model, languageId);

        public static Task SetModelLanguage(IJSRuntime jsRuntime, TextModel model, string mimeTypeOrLanguageId)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setModelLanguage", model.Uri, mimeTypeOrLanguageId);

        /**
         * Set the markers for a model.
         */
        public static Task SetModelMarkers(IJSRuntime jsRuntime, TextModel model, string owner, List<MarkerData> markers)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setModelMarkers", model.Uri, owner, markers);

        /**
         * Remove all markers of an owner.
         */
        public static Task RemoveAllMarkers(IJSRuntime jsRuntime, string owner)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.removeAllMarkers", owner);

        /**
         * Get markers for owner and/or resource
         *
         * @returns list of markers
         */
        public static Task<List<Marker>> GetModelMarkers(IJSRuntime jsRuntime, GetModelMarkersFilter filter)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<List<Marker>>("blazorMonaco.editor.getModelMarkers", filter);

        public class GetModelMarkersFilter
        {
            public string Owner { get; set; }
            public string Resource { get; set; }
            public int? Take { get; set; }
        }


        /**
         * Get the model that has `uri` if it exists.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<TextModel> GetModel(string uri)
            => GetModel(null, uri);

        public static async Task<TextModel> GetModel(IJSRuntime jsRuntime, string uri)
        {
            var textModel = await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<TextModel>("blazorMonaco.editor.getModel", uri);
            if (textModel != null)
                textModel.JsRuntime = JsRuntimeExt.UpdateRuntime(jsRuntime);
            return textModel;
        }

        /**
         * Get all the created models.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<List<TextModel>> GetModels()
            => GetModels(null);

        public static async Task<List<TextModel>> GetModels(IJSRuntime jsRuntime)
        {
            var result = await JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<List<TextModel>>("blazorMonaco.editor.getModels");
            result.ForEach(t =>
            {
                if (t != null)
                    t.JsRuntime = JsRuntimeExt.UpdateRuntime(jsRuntime);
            });
            return result;
        }


        /**
         * Colorize the contents of `domNode` using attribute `data-lang`.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task ColorizeElement(string domNodeId, ColorizerElementOptions options)
            => ColorizeElement(null, domNodeId, options);
        public static Task ColorizeElement(IJSRuntime jsRuntime, string domNodeId, ColorizerElementOptions options)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.colorizeElement", domNodeId, options);

        /**
         * Colorize `text` using language `languageId`.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<string> Colorize(string text, string languageId, ColorizerOptions options)
            => Colorize(null, text, languageId, options);

        public static Task<string> Colorize(IJSRuntime jsRuntime, string text, string languageId, ColorizerOptions options)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<string>("blazorMonaco.editor.colorize", text, languageId, options);

        /**
         * Colorize a line in a model.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<string> ColorizeModelLine(TextModel model, int lineNumber, int? tabSize = null)
            => ColorizeModelLine(null, model, lineNumber, tabSize);

        public static Task<string> ColorizeModelLine(IJSRuntime jsRuntime, TextModel model, int lineNumber, int? tabSize = null)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<string>("blazorMonaco.editor.colorizeModelLine", model.Uri, lineNumber, tabSize);


        /**
         * Define a new theme or update an existing theme.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task DefineTheme(string themeName, StandaloneThemeData themeData)
            => DefineTheme(null, themeName, themeData);

        public static Task DefineTheme(IJSRuntime jsRuntime, string themeName, StandaloneThemeData themeData)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.defineTheme", themeName, themeData);

        /**
         * Switches to a theme.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task SetTheme(string themeName)
            => SetTheme(null, themeName);

        public static Task SetTheme(IJSRuntime jsRuntime, string themeName)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.setTheme", themeName);

        /**
         * Clears all cached font measurements and triggers re-measurement.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RemeasureFonts()
            => RemeasureFonts(null);

        public static Task RemeasureFonts(IJSRuntime jsRuntime)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.remeasureFonts");

        /**
         * Returns the ids of all created code editors.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<List<string>> GetEditors()
            => GetEditors(null);

        public static Task<List<string>> GetEditors(IJSRuntime jsRuntime)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<List<string>>("blazorMonaco.editor.getEditors");

        /**
         * Returns the ids of all created diff editors.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<List<string>> GetDiffEditors()
            => GetDiffEditors(null);

        public static Task<List<string>> GetDiffEditors(IJSRuntime jsRuntime)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<List<string>>("blazorMonaco.editor.getDiffEditors");

        /**
         * Tokenize a string. Returns the tokens grouped by line.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<List<List<Token>>> Tokenize(string text, string languageId)
            => Tokenize(null, text, languageId);

        public static Task<List<List<Token>>> Tokenize(IJSRuntime jsRuntime, string text, string languageId)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<List<List<Token>>>("blazorMonaco.editor.tokenize", text, languageId);

        /**
         * Register a command. The command handler is invoked (via JavaScript) when the
         * command is executed, e.g. through `editor.executeCommand(...)` or a keybinding.
         */
        private static readonly Dictionary<string, Action<object[]>> _commands = new Dictionary<string, Action<object[]>>();
        private static DotNetObjectReference<CommandHandler> _commandHandlerRef;

        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterCommand(string commandId, Action<object[]> handler)
            => RegisterCommand(null, commandId, handler);

        public static Task RegisterCommand(IJSRuntime jsRuntime, string commandId, Action<object[]> handler)
        {
            _commands[commandId] = handler;
            if (_commandHandlerRef == null)
                _commandHandlerRef = DotNetObjectReference.Create(new CommandHandler());
            return JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.registerCommand", commandId, _commandHandlerRef);
        }

        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task AddCommand(object descriptor)
            => AddCommand(null, descriptor);

        public static Task AddCommand(IJSRuntime jsRuntime, object descriptor)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.addCommandDescriptor", descriptor);

        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task AddEditorAction(object descriptor)
            => AddEditorAction(null, descriptor);

        public static Task AddEditorAction(IJSRuntime jsRuntime, object descriptor)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.addEditorAction", descriptor);

        /// <summary>
        /// .NET callback target invoked by JavaScript when a registered command runs.
        /// </summary>
        internal class CommandHandler
        {
            [JSInvokable]
            public void Invoke(string commandId, object[] args)
            {
                if (_commands.TryGetValue(commandId, out var handler))
                    handler?.Invoke(args);
            }
        }


        /**
         * Register a handler that is called whenever a link is opened in an editor.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterLinkOpener(LinkOpener linkOpener)
            => RegisterLinkOpener(null, linkOpener);

        public static Task RegisterLinkOpener(IJSRuntime jsRuntime, LinkOpener linkOpener)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.registerLinkOpener", DotNetObjectReference.Create(linkOpener));

        /**
         * Register a handler that is called whenever a resource other than the current
         * model is opened in an editor (e.g. "go to definition").
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterEditorOpener(CodeEditorOpener codeEditorOpener)
            => RegisterEditorOpener(null, codeEditorOpener);

        public static Task RegisterEditorOpener(IJSRuntime jsRuntime, CodeEditorOpener codeEditorOpener)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.registerEditorOpener", DotNetObjectReference.Create(codeEditorOpener));

        /**
         * Add a single global keybinding rule.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterKeybindingRule(KeybindingRule rule)
            => RegisterKeybindingRule(null, rule);

        public static Task RegisterKeybindingRule(IJSRuntime jsRuntime, KeybindingRule rule)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.addKeybindingRule", rule);

        /**
         * Add multiple global keybinding rules at once.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterKeybindingRules(List<KeybindingRule> rules)
            => RegisterKeybindingRules(null, rules);

        public static Task RegisterKeybindingRules(IJSRuntime jsRuntime, List<KeybindingRule> rules)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.addKeybindingRules", rules);

        /**
         * Register persistent global lifecycle listeners (onDidCreateEditor(s),
         * onDidChangeMarkers, onDidChangeModelLanguage, onDidCreateModel, onWillDisposeModel)
         * that fire the `MonacoLifecycle` hub. Call once; subsequent calls are no-ops.
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task RegisterLifecycleListeners()
            => RegisterLifecycleListeners(null);

        public static Task RegisterLifecycleListeners(IJSRuntime jsRuntime)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.editor.registerLifecycleListeners", MonacoLifecycle.GetRef());


        /**
         * Create a new web worker (see `monaco.editor.createWebWorker`).
         */
        [Obsolete("This method is deprecated as it's WASM only. Use the overload that takes an IJSRuntime parameter.")]
        public static Task<MonacoWebWorker<T>> CreateWebWorker<T>(WebWorkerOptions options) where T : class
            => CreateWebWorker<T>(null, options);

        public static async Task<MonacoWebWorker<T>> CreateWebWorker<T>(IJSRuntime jsRuntime, WebWorkerOptions options) where T : class
        {
            var runtime = JsRuntimeExt.UpdateRuntime(jsRuntime);
            var id = await runtime.SafeInvokeAsync<string>("blazorMonaco.editor.createWebWorker", options);
            return new MonacoWebWorker<T> { Id = id, JsRuntime = runtime };
        }

    }
    public class BuiltinTheme
    {
        public const string Vs = "vs";
        public const string VsDark = "vs-dark";
        public const string HcBlack = "hc-black";
        public const string HcLight = "hc-light";
    }

    public class StandaloneThemeData
    {
        public string Base { get; set; }
        public bool Inherit { get; set; }
        public List<TokenThemeRule> Rules { get; set; }
        public List<string> EncodedTokensColors { get; set; }
        public Dictionary<string, string> Colors { get; set; }
    }

    public class TokenThemeRule
    {
        public string Token { get; set; }
        public string Foreground { get; set; }
        public string Background { get; set; }
        public string FontStyle { get; set; }
    }

    /**
     * A web worker that can provide a proxy to an arbitrary file.
     */


    /**
     * Options passed to `Global.CreateWebWorker`.
     */
    public class WebWorkerOptions
    {
        /**
         * URL used to construct the underlying JavaScript `Worker`.
         * monaco's `IInternalWebWorkerOptions.worker` is a `Worker`; from C# a URL string is the
         * constructible equivalent, so JavaScript creates `new Worker(workerUrl)` for you.
         */
        public string WorkerUrl { get; set; }
        /**
         * An object that can be used by the web worker to make calls back to the main thread.
         */
        public object Host { get; set; }
        /**
         * Keep idle models. Defaults to false, which means that idle models will stop syncing after a while.
         */
        public bool? KeepIdleModels { get; set; }
    }

    /**
     * A web worker that can provide a proxy to an arbitrary loaded file. Mirrors
     * `monaco.editor.createWebWorker<T>` and `monaco.editor.MonacoWebWorker<T>`.
     */
    public class MonacoWebWorker<T> where T : class
    {
        internal IJSRuntime JsRuntime { get; set; }
        internal string Id { get; set; }

        /** Terminate the web worker, thus invalidating the returned proxy. */
        public Task Dispose()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.worker.dispose", Id);

        /** Get a proxy to the arbitrary loaded code. */
        public Task<T> GetProxy()
            => JsRuntime.SafeInvokeAsync<T>("blazorMonaco.worker.getProxy", Id);

        /** Synchronize (send) the models at `resources` (uris) to the web worker. */
        public Task<T> WithSyncedResources(List<string> resources)
            => JsRuntime.SafeInvokeAsync<T>("blazorMonaco.worker.withSyncedResources", Id, resources);
    }

    /**
     * Editor zoom control. Mirrors `monaco.editor.EditorZoom`.
     * Provides global zoom level get/set for all editor instances.
     *
     * To use with DI inBlazor Server: Inject EditorZoom and call methods.
     * To use explicitly: pass IJSRuntime to each method call.
     */
    public class EditorZoom
    {
        private readonly IJSRuntime _jsRuntime;

        public EditorZoom(IJSRuntime jsRuntime)
            => _jsRuntime = JsRuntimeExt.UpdateRuntime(jsRuntime);

        /**
         * Gets the current zoom level.
         */
        public Task<double> GetZoomLevel()
            => _jsRuntime.SafeInvokeAsync<double>("blazorMonaco.editor.getZoomLevel");

        /**
         * Sets the zoom level for all editor instances.
         * @param zoomLevel The zoom level (1.0 = 100%).
         */
        public Task SetZoomLevel(double zoomLevel)
            => _jsRuntime.SafeInvokeAsync("blazorMonaco.editor.setZoomLevel", null, zoomLevel);
    }

    /**
     * Description of an action contribution
     */
    public class ActionDescriptor
    {
        /**
         * An unique identifier of the contributed action.
         */
        public string Id { get; set; }
        /**
         * A label of the action that will be presented to the user.
         */
        public string Label { get; set; }
        /**
         * Precondition rule. The value should be a [context key expression](https://code.visualstudio.com/docs/getstarted/keybindings#_when-clause-contexts).
         */
        public string Precondition { get; set; }
        /**
         * An array of keybindings for the action.
         */
        public int[] Keybindings { get; set; }
        /**
         * The keybinding rule (condition on top of precondition).
         */
        public string KeybindingContext { get; set; }
        /**
         * Control if the action should show up in the context menu and where.
         * The context menu of the editor has these default:
         *   navigation - The navigation group comes first in all cases.
         *   1_modification - This group comes next and contains commands that modify your code.
         *   9_cutcopypaste - The last default group with the basic editing commands.
         * You can also create your own group.
         * Defaults to null (don't show in context menu).
         */
        public string ContextMenuGroupId { get; set; }
        /**
         * Control the order in the context menu group.
         */
        public float ContextMenuOrder { get; set; }
        /**
         * Method that will be executed when the action is triggered.
         * @param editor The editor instance is passed in as a convenience
         */
        [JsonIgnore]
        public Action<CodeEditor> Run { get; set; }
    }

    /**
     * Options which apply for all editors.
     */
    public interface IGlobalEditorOptions
    {
        /**
         * The number of spaces a tab is equal to.
         * This setting is overridden based on the file contents when `detectIndentation` is on.
         * Defaults to 4.
         */
        int? TabSize { get; set; }
        /**
         * Insert spaces when pressing `Tab`.
         * This setting is overridden based on the file contents when `detectIndentation` is on.
         * Defaults to true.
         */
        bool? InsertSpaces { get; set; }
        /**
         * Controls whether `tabSize` and `insertSpaces` will be automatically detected when a file is opened based on the file contents.
         * Defaults to true.
         */
        bool? DetectIndentation { get; set; }
        /**
         * Remove trailing auto inserted whitespace.
         * Defaults to true.
         */
        bool? TrimAutoWhitespace { get; set; }
        /**
         * Special handling for large files to disable certain memory intensive features.
         * Defaults to true.
         */
        bool? LargeFileOptimizations { get; set; }
        /**
         * Controls whether completions should be computed based on words in the document.
         * Defaults to true.
         */
        object WordBasedSuggestions { get; set; }
        /**
         * Controls whether word based completions should be included from opened documents of the same language or any language.
         */
        bool? WordBasedSuggestionsOnlySameLanguage { get; set; }
        /**
         * Controls whether the semanticHighlighting is shown for the languages that support it.
         * true: semanticHighlighting is enabled for all themes
         * false: semanticHighlighting is disabled for all themes
         * 'configuredByTheme': semanticHighlighting is controlled by the current color theme's semanticHighlighting setting.
         * Defaults to 'byTheme'.
         */
        [JsonPropertyName("semanticHighlighting.enabled")]
        object SemanticHighlightingEnabled { get; set; }
        /**
         * Keep peek editors open even when double-clicking their content or when hitting `Escape`.
         * Defaults to false.
         */
        bool? StablePeek { get; set; }
        /**
         * Lines above this length will not be tokenized for performance reasons.
         * Defaults to 20000.
         */
        int? MaxTokenizationLineLength { get; set; }
        /**
         * Theme to be used for rendering.
         * The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light'.
         * You can create custom themes via `monaco.editor.defineTheme`.
         * To switch a theme, use `monaco.editor.setTheme`.
         * **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless `autoDetectHighContrast` is set to false.
         */
        string Theme { get; set; }
        /**
         * If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
         * Defaults to true.
         */
        bool? AutoDetectHighContrast { get; set; }
    }

    public class EditorUpdateOptions : EditorOptions, IGlobalEditorOptions
    {
        public int? TabSize { get; set; }
        public bool? InsertSpaces { get; set; }
        public bool? DetectIndentation { get; set; }
        public bool? TrimAutoWhitespace { get; set; }
        public bool? LargeFileOptimizations { get; set; }
        public object WordBasedSuggestions { get; set; }
        public bool? WordBasedSuggestionsOnlySameLanguage { get; set; }
        [JsonPropertyName("semanticHighlighting.enabled")]
        public object SemanticHighlightingEnabled { get; set; }
        public bool? StablePeek { get; set; }
        public int? MaxTokenizationLineLength { get; set; }
        public string Theme { get; set; }
        public bool? AutoDetectHighContrast { get; set; }
    }

    /**
     * The options to create an editor.
     */
    public class StandaloneEditorConstructionOptions : EditorConstructionOptions, IGlobalEditorOptions
    {
        /**
         * The initial model associated with this code editor.
         */
        public TextModel Model { get; set; }
        /**
         * The initial value of the auto created model in the editor.
         * To not automatically create a model, use `model: null`.
         */
        public string Value { get; set; }
        /**
         * The initial language of the auto created model in the editor.
         * To not automatically create a model, use `model: null`.
         */
        public string Language { get; set; }
        /**
         * Initial theme to be used for rendering.
         * The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light'.
         * You can create custom themes via `monaco.editor.defineTheme`.
         * To switch a theme, use `monaco.editor.setTheme`.
         * **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless `autoDetectHighContrast` is set to false.
         */
        public string Theme { get; set; }
        /**
         * If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
         * Defaults to true.
         */
        public bool? AutoDetectHighContrast { get; set; }
        /**
         * An URL to open when Ctrl+H (Windows and Linux) or Cmd+H (OSX) is pressed in
         * the accessibility help dialog in the editor.
         *
         * Defaults to "https://go.microsoft.com/fwlink/?linkid=852450"
         */
        public string AccessibilityHelpUrl { get; set; }

        /**
        * Container element to use for ARIA messages.
        * Defaults to document.body.
        */
        //ariaContainerElement?: HTMLElement;

        public int? TabSize { get; set; }
        public bool? InsertSpaces { get; set; }
        public bool? DetectIndentation { get; set; }
        public bool? TrimAutoWhitespace { get; set; }
        public bool? LargeFileOptimizations { get; set; }
        public object WordBasedSuggestions { get; set; }
        public bool? WordBasedSuggestionsOnlySameLanguage { get; set; }
        [JsonPropertyName("semanticHighlighting.enabled")]
        public object SemanticHighlightingEnabled { get; set; }
        public bool? StablePeek { get; set; }
        public int? MaxTokenizationLineLength { get; set; }
    }

    /**
     * The options to create a diff editor.
     */
    public class StandaloneDiffEditorConstructionOptions : DiffEditorConstructionOptions
    {
        /**
         * Initial theme to be used for rendering.
         * The current out-of-the-box available themes are: 'vs' (default), 'vs-dark', 'hc-black', 'hc-light'.
         * You can create custom themes via `monaco.editor.defineTheme`.
         * To switch a theme, use `monaco.editor.setTheme`.
         * **NOTE**: The theme might be overwritten if the OS is in high contrast mode, unless `autoDetectHighContrast` is set to false.
         */
        public string Theme { get; set; }
        /**
         * If enabled, will automatically change to high contrast theme if the OS is using a high contrast theme.
         * Defaults to true.
         */
        public bool? AutoDetectHighContrast { get; set; }
    }


    public delegate void CommandHandler(params object[] args);

    public class EditorOverrideServices : Dictionary<string, object>
    { }

    public class Marker
    {
        public string Owner { get; set; }
        [JsonPropertyName("resource")]
        public string ResourceUri { get; set; }
        public MarkerSeverity Severity { get; set; }
        public JsonElement? Code { get; set; }
        [JsonIgnore]
        public string CodeAsString
        {
            get => Code?.AsString();
            set => Code = JsonElementExt.FromObject(value);
        }
        [JsonIgnore]
        public MarkerCode CodeAsObject
        {
            get => Code?.AsObject<MarkerCode>();
            set => Code = JsonElementExt.FromObject(value);
        }
        public string Message { get; set; }
        public string Source { get; set; }
        public int StartLineNumber { get; set; }
        public int StartColumn { get; set; }
        public int EndLineNumber { get; set; }
        public int EndColumn { get; set; }
        public int ModelVersionId { get; set; }
        public List<RelatedInformation> RelatedInformation { get; set; }
        public List<MarkerTag> Tags { get; set; }
        public string Origin { get; set; }
    }

    public class MarkerCode
    {
        public string Value { get; set; }
        [JsonPropertyName("target")]
        public string TargetUri { get; set; }
    }

    /**
     * A structure defining a problem/warning/etc.
     */
    public class MarkerData
    {
        public JsonElement? Code { get; set; }
        [JsonIgnore]
        public string CodeAsString
        {
            get => Code?.AsString();
            set => Code = JsonElementExt.FromObject(value);
        }
        [JsonIgnore]
        public MarkerCode CodeAsObject
        {
            get => Code?.AsObject<MarkerCode>();
            set => Code = JsonElementExt.FromObject(value);
        }
        public MarkerSeverity Severity { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public int StartLineNumber { get; set; }
        public int StartColumn { get; set; }
        public int EndLineNumber { get; set; }
        public int EndColumn { get; set; }
        public int ModelVersionId { get; set; }
        public List<RelatedInformation> RelatedInformation { get; set; }
        public List<MarkerTag> Tags { get; set; }
        public string Origin { get; set; }
    }

    /**
     *
     */
    public class RelatedInformation
    {
        [JsonPropertyName("resource")]
        public string ResourceUri { get; set; }
        public string Message { get; set; }
        public int StartLineNumber { get; set; }
        public int StartColumn { get; set; }
        public int EndLineNumber { get; set; }
        public int EndColumn { get; set; }
    }

    public class ColorizerOptions
    {
        public int? TabSize { get; set; }
    }

    public class ColorizerElementOptions : ColorizerOptions
    {
        public string Theme { get; set; }
        public string MimeType { get; set; }
    }

    public enum ScrollbarVisibility
    {
        Auto = 1,
        Hidden = 2,
        Visible = 3
    }

    public class ThemeColor
    {
        public string Id { get; set; }
    }

    public class ThemeIcon
    {
        public string Id { get; set; }
        public ThemeColor Color { get; set; }
    }

    /**
     * A single edit operation, that acts as a simple replace.
     * i.e. Replace text at `range` with `text` in model.
     */
    public class SingleEditOperation
    {
        /**
         * The range to replace. This can be empty to emulate a simple insert.
         */
        public Range Range { get; set; }
        /**
         * The text to replace with. This can be null to emulate a simple delete.
         */
        public string Text { get; set; }
        /**
         * This indicates that this operation has "insert" semantics.
         * i.e. forceMoveMarkers = true => if `range` is collapsed, all markers at the position will be moved.
         */
        public bool? ForceMoveMarkers { get; set; }
    }

    /**
     * Word inside a model.
     */
    public class WordAtPosition
    {
        /**
         * The word.
         */
        public string Word { get; set; }
        /**
         * The column where the word starts.
         */
        public int StartColumn { get; set; }
        /**
         * The column where the word ends.
         */
        public int EndColumn { get; set; }
    }

    /**
     * Vertical Lane in the overview ruler of the editor.
     */
    public enum OverviewRulerLane
    {
        Left = 1,
        Center = 2,
        Right = 4,
        Full = 7
    }

    /**
     * Vertical Lane in the glyph margin of the editor.
     */
    public enum GlyphMarginLane
    {
        Left = 1,
        Center = 2,
        Right = 3
    }


    /**
     * Position in the minimap to render the decoration.
     */
    public enum MinimapPosition
    {
        Inline = 1,
        Gutter = 2
    }

    /**
     * Section header style.
     */
    public enum MinimapSectionHeaderStyle
    {
        Normal = 1,
        Underlined = 2
    }

    public class DecorationOptions
    {
        /**
         * CSS color to render.
         * e.g.: rgba(100, 100, 100, 0.5) or a color from the color registry
         */
        public string Color { get; set; }
        /**
         * CSS color to render.
         * e.g.: rgba(100, 100, 100, 0.5) or a color from the color registry
         */
        public string DarkColor { get; set; }
    }

    public class ModelDecorationGlyphMarginOptions
    {
        /**
         * The position in the glyph margin.
         */
        public GlyphMarginLane Position { get; set; }
        /**
         * Whether the glyph margin lane in {@link position} should be rendered even
         * outside of this decoration's range.
         */
        public bool? PersistLane { get; set; }
    }

    /**
     * Options for rendering a model decoration in the overview ruler.
     */
    public class ModelDecorationOverviewRulerOptions : DecorationOptions
    {
        /**
         * The position in the overview ruler.
         */
        public OverviewRulerLane Position { get; set; }
    }

    /**
     * Options for rendering a model decoration in the minimap.
     */
    public class ModelDecorationMinimapOptions : DecorationOptions
    {
        /**
         * The position in the minimap.
         */
        public MinimapPosition Position { get; set; }
        /**
         * If the decoration is for a section header, which header style.
         */
        public MinimapSectionHeaderStyle? SectionHeaderStyle { get; set; }
        /**
         * If the decoration is for a section header, the header text.
         */
        public string SectionHeaderText { get; set; }
    }

    /**
     * Options for a model decoration.
     */
    public class ModelDecorationOptions
    {
        /**
         * Customize the growing behavior of the decoration when typing at the edges of the decoration.
         * Defaults to TrackedRangeStickiness.AlwaysGrowsWhenTypingAtEdges
         */
        public TrackedRangeStickiness? Stickiness { get; set; }
        /**
         * CSS class name describing the decoration.
         */
        public string ClassName { get; set; }
        /**
         * Indicates whether the decoration should span across the entire line when it continues onto the next line.
         */
        public bool? ShouldFillLineOnLineBreak { get; set; }
        public string BlockClassName { get; set; }
        /**
         * Indicates if this block should be rendered after the last line.
         * In this case, the range must be empty and set to the last line.
         */
        public bool? BlockIsAfterEnd { get; set; }
        public bool? BlockDoesNotCollapse { get; set; }
        //blockPadding?: [top: number, right: number, bottom: number, left: number] | null;
        /**
         * Message to be rendered when hovering over the glyph margin decoration.
         */
        public MarkdownString[] GlyphMarginHoverMessage { get; set; }
        /**
         * Array of MarkdownString to render as the decoration message.
         */
        public MarkdownString[] HoverMessage { get; set; }
        /**
         * Array of MarkdownString to render as the line number message.
         */
        public MarkdownString[] LineNumberHoverMessage { get; set; }
        /**
         * Should the decoration expand to encompass a whole line.
         */
        public bool? IsWholeLine { get; set; }
        /**
         * Always render the decoration (even when the range it encompasses is collapsed).
         */
        public bool? ShowIfCollapsed { get; set; }
        /**
         * Specifies the stack order of a decoration.
         * A decoration with greater stack order is always in front of a decoration with
         * a lower stack order when the decorations are on the same line.
         */
        public int? ZIndex { get; set; }
        /**
         * If set, render this decoration in the overview ruler.
         */
        public ModelDecorationOverviewRulerOptions OverviewRuler { get; set; }
        /**
         * If set, render this decoration in the minimap.
         */
        public ModelDecorationMinimapOptions Minimap { get; set; }
        /**
         * If set, the decoration will be rendered in the glyph margin with this CSS class name.
         */
        public string GlyphMarginClassName { get; set; }
        /**
         * If set and the decoration has {@link glyphMarginClassName} set, render this decoration
         * with the specified {@link IModelDecorationGlyphMarginOptions} in the glyph margin.
         */
        public ModelDecorationGlyphMarginOptions GlyphMargin { get; set; }
        /**
         * If set, the decoration will override the line height of the lines it spans. This value is a multiplier to the default line height.
         */
        public int? LineHeight { get; set; }
        /**
         * Font family
         */
        public string FontFamily { get; set; }
        /**
         * Font size
         */
        public string FontSize { get; set; }
        /**
         * Font weight
         */
        public string FontWeight { get; set; }
        /**
         * Font style
         */
        public string FontStyle { get; set; }
        /**
         * If set, the decoration will be rendered in the lines decorations with this CSS class name.
         */
        public string LinesDecorationsClassName { get; set; }
        /**
         * Controls the tooltip text of the line decoration.
         */
        public string LinesDecorationsTooltip { get; set; }
        /**
         * If set, the decoration will be rendered on the line number.
         */
        public string LineNumberClassName { get; set; }
        /**
         * If set, the decoration will be rendered in the lines decorations with this CSS class name, but only for the first line in case of line wrapping.
         */
        public string FirstLineDecorationClassName { get; set; }
        /**
         * If set, the decoration will be rendered in the margin (covering its full width) with this CSS class name.
         */
        public string MarginClassName { get; set; }
        /**
         * If set, the decoration will be rendered inline with the text with this CSS class name.
         * Please use this only for CSS rules that must impact the text. For example, use `className`
         * to have a background color decoration.
         */
        public string InlineClassName { get; set; }
        /**
         * If there is an `inlineClassName` which affects letter spacing.
         */
        public bool? InlineClassNameAffectsLetterSpacing { get; set; }
        /**
         * If set, the decoration will be rendered before the text with this CSS class name.
         */
        public string BeforeContentClassName { get; set; }
        /**
         * If set, the decoration will be rendered after the text with this CSS class name.
         */
        public string AfterContentClassName { get; set; }
        /**
         * If set, text will be injected in the view after the range.
         */
        public InjectedTextOptions After { get; set; }
        /**
         * If set, text will be injected in the view before the range.
         */
        public InjectedTextOptions Before { get; set; }
        /**
         * The text direction of the decoration.
         */
        public TextDirection? TextDirection { get; set; }
    }

    /**
     * Text Direction for a decoration.
     */
    public enum TextDirection
    {
        LTR = 0,
        RTL = 1
    }

    /**
     * Configures text that is injected into the view without changing the underlying document.
    */
    public class InjectedTextOptions
    {
        /**
         * Sets the text to inject. Must be a single line.
         */
        public string Content { get; set; }
        /**
         * If set, the decoration will be rendered inline with the text with this CSS class name.
         */
        public string InlineClassName { get; set; }
        /**
         * If there is an `inlineClassName` which affects letter spacing.
         */
        public bool? InlineClassNameAffectsLetterSpacing { get; set; }
        /**
         * This field allows to attach data to this injected text.
         * The data can be read when injected texts at a given position are queried.
         */
        //readonly attachedData?: unknown;
        /**
         * Configures cursor stops around injected text.
         * Defaults to {@link InjectedTextCursorStops.Both}.
        */
        public InjectedTextCursorStops? CursorStops { get; set; }
    }

    public enum InjectedTextCursorStops
    {
        Both = 0,
        Right = 1,
        Left = 2,
        None = 3
    }

    /**
     * New model decorations.
     */
    public class ModelDeltaDecoration
    {
        /**
         * Range that this decoration covers.
         */
        public Range Range { get; set; }
        /**
         * Options associated with this decoration.
         */
        public ModelDecorationOptions Options { get; set; }
    }

    /**
     * A decoration in the model.
     */
    public class ModelDecoration
    {
        /**
         * Identifier for a decoration.
         */
        public string Id { get; set; }
        /**
         * Identifier for a decoration's owner.
         */
        public int OwnerId { get; set; }
        /**
         * Range that this decoration covers.
         */
        public Range Range { get; set; }
        /**
         * Options associated with this decoration.
         */
        public ModelDecorationOptions Options { get; set; }
    }

    /**
     * End of line character preference.
     */
    public enum EndOfLinePreference
    {
        /**
         * Use the end of line character identified in the text buffer.
         */
        TextDefined = 0,
        /**
         * Use line feed (\n) as the end of line character.
         */
        LF = 1,
        /**
         * Use carriage return and line feed (\r\n) as the end of line character.
         */
        CRLF = 2
    }

    /**
     * The default end of line to use when instantiating models.
     */
    public enum DefaultEndOfLine
    {
        /**
         * Use line feed (\n) as the end of line character.
         */
        LF = 1,
        /**
         * Use carriage return and line feed (\r\n) as the end of line character.
         */
        CRLF = 2
    }

    /**
     * End of line character preference.
     */
    public enum EndOfLineSequence
    {
        /**
         * Use line feed (\n) as the end of line character.
         */
        LF = 0,
        /**
         * Use carriage return and line feed (\r\n) as the end of line character.
         */
        CRLF = 1
    }

    /**
     * A single edit operation, that has an identifier.
     */
    public class IdentifiedSingleEditOperation : SingleEditOperation
    {
    }

    public class ValidEditOperation
    {
        /**
         * The range to replace. This can be empty to emulate a simple insert.
         */
        public Range Range { get; set; }
        /**
         * The text to replace with. This can be empty to emulate a simple delete.
         */
        public string Text { get; set; }
    }

    /**
     * A callback that can compute the cursor state after applying a series of edit operations.
     */
    public delegate List<Selection> CursorStateComputer(List<ValidEditOperation> inverseEditOperations);

    public class TextModelResolvedOptions
    {
        public int TabSize { get; set; }
        public int IndentSize { get; set; }
        public bool InsertSpaces { get; set; }
        public DefaultEndOfLine DefaultEOL { get; set; }
        public bool TrimAutoWhitespace { get; set; }
        public BracketPairColorizationOptions BracketPairColorizationOptions { get; set; }
    }


    public class TextModelUpdateOptions
    {
        public int? TabSize { get; set; }
        public object IndentSize { get; set; }
        public bool? InsertSpaces { get; set; }
        public bool? TrimAutoWhitespace { get; set; }
        public BracketPairColorizationOptions BracketColorizationOptions { get; set; }
    }

    public class FindMatch
    {
        public Range Range { get; set; }
        public string[] Matches { get; set; }
    }

    /**
     * Describes the behavior of decorations when typing/editing near their edges.
     * Note: Please do not edit the values, as they very carefully match `DecorationRangeBehavior`
     */
    public enum TrackedRangeStickiness
    {
        AlwaysGrowsWhenTypingAtEdges = 0,
        NeverGrowsWhenTypingAtEdges = 1,
        GrowsOnlyWhenTypingBefore = 2,
        GrowsOnlyWhenTypingAfter = 3
    }

    /**
     * Text snapshot that works like an iterator.
     * Will try to return chunks of roughly ~64KB size.
     * Will return null when finished.
     */
    public interface ITextSnapshot
    {
        string Read();
    }


    public enum PositionAffinity
    {
        /**
         * Prefers the left most position.
         */
        Left = 0,
        /**
         * Prefers the right most position.
         */
        Right = 1,
        /**
         * No preference.
         */
        None = 2,
        /**
         * If the given position is on injected text, prefers the position left of it.
         */
        LeftOfInjectedText = 3,
        /**
         * If the given position is on injected text, prefers the position right of it.
         */
        RightOfInjectedText = 4
    }

    public class Dimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class DiffEditorModel
    {
        /**
         * Original model.
         */
        public TextModel Original { get; set; }
        /**
         * Modified model.
         */
        public TextModel Modified { get; set; }
    }

    /**
     * An event describing that an editor has had its model reset (i.e. `editor.setModel()`).
     */
    public class ModelChangedEvent
    {
        /**
         * The `uri` of the previous model or null.
         */
        public string OldModelUrl { get; set; }
        /**
         * The `uri` of the new model or null.
         */
        public string NewModelUrl { get; set; }
    }

    public class ContentSizeChangedEvent
    {
        public double ContentWidth { get; set; }
        public double ContentHeight { get; set; }
        public bool ContentWidthChanged { get; set; }
        public bool ContentHeightChanged { get; set; }
    }

    public class NewScrollPosition
    {
        public double ScrollLeft { get; set; }
        public double ScrollTop { get; set; }
    }

    public class EditorAction
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Alias { get; set; }
        public CommandMetadata Metadata { get; set; }
        public bool IsSupported() => true;
        public Task Run(object args = null) => Task.CompletedTask;
    }

    public class CommandMetadata
    {
        public string Description { get; set; }
    }


    /**
     * A (serializable) state of the cursors.
     */
    public class CursorState
    {
        public bool InSelectionMode { get; set; }
        public Position SelectionStart { get; set; }
        public Position Position { get; set; }
    }

    /**
     * A (serializable) state of the view.
     */
    public class ViewState
    {
        /** written by previous versions */
        public int? ScrollTop { get; set; }
        /** written by previous versions */
        public int? ScrollTopWithoutViewZones { get; set; }
        public int ScrollLeft { get; set; }
        public Position FirstPosition { get; set; }
        public int FirstPositionDeltaTop { get; set; }
    }

    /**
     * A (serializable) state of the code editor.
     */
    public class CodeEditorViewState
    {
        public List<CursorState> CursorState { get; set; }
        public ViewState ViewState { get; set; }
        public Dictionary<string, object> ContributionsState { get; set; }
    }

    /**
     * (Serializable) View state for the diff editor.
     */
    public class DiffEditorViewState
    {
        public CodeEditorViewState Original { get; set; }
        public CodeEditorViewState Modified { get; set; }
        public object ModelState { get; set; }
    }

    /**
     * An editor view state.
     */

    public enum ScrollType
    {
        Smooth = 0,
        Immediate = 1
    }

    /**
     * An editor.
     */

    /**
     * A collection of decorations
     */

    /**
     * An editor contribution that gets created every time a new editor gets created and gets disposed when the editor gets disposed.
     */

    /**
     * The type of the `IEditor`.
     */

    /**
     * An event describing that the current language associated with a model has changed.
     */
    public class ModelLanguageChangedEvent
    {
        /**
         * Previous language
         */
        public string OldLanguage { get; set; }
        /**
         * New language
         */
        public string NewLanguage { get; set; }
        /**
         * Source of the call that caused the event.
         */
        public string Source { get; set; }
    }

    /**
     * An event describing that the language configuration associated with a model has changed.
     */
    public class ModelLanguageConfigurationChangedEvent
    {
    }

    /**
     * An event describing a change in the text of a model.
     */
    public class ModelContentChangedEvent
    {
        /**
         * The changes are ordered from the end of the document to the beginning, so they should be safe to apply in sequence.
         */
        public List<ModelContentChange> Changes { get; set; }
        /**
         * The (new) end-of-line character.
         */
        public string Eol { get; set; }
        /**
         * The new version id the model has transitioned to.
         */
        public int VersionId { get; set; }
        /**
         * Flag that indicates that this event was generated while undoing.
         */
        public bool IsUndoing { get; set; }
        /**
         * Flag that indicates that this event was generated while redoing.
         */
        public bool IsRedoing { get; set; }
        /**
         * Flag that indicates that all decorations were lost with this edit.
         * The model has been reset to a new value.
         */
        public bool IsFlush { get; set; }
        /**
         * Flag that indicates that this event describes an eol change.
         */
        public bool IsEolChange { get; set; }
        /**
         * The sum of these lengths equals changes.length.
         * The length of this array must equal the length of detailedReasons.
        */
        public List<int> DetailedReasonsChangeLengths { get; set; }
    }

    public class SerializedModelContentChangedEvent
    {
        /**
         * The changes are ordered from the end of the document to the beginning, so they should be safe to apply in sequence.
         */
        public List<ModelContentChange> Changes { get; set; }
        /**
         * The (new) end-of-line character.
         */
        public string Eol { get; set; }
        /**
         * The new version id the model has transitioned to.
         */
        public int VersionId { get; set; }
        /**
         * Flag that indicates that this event was generated while undoing.
         */
        public bool IsUndoing { get; set; }
        /**
         * Flag that indicates that this event was generated while redoing.
         */
        public bool IsRedoing { get; set; }
        /**
         * Flag that indicates that all decorations were lost with this edit.
         * The model has been reset to a new value.
         */
        public bool IsFlush { get; set; }
        /**
         * Flag that indicates that this event describes an eol change.
         */
        public bool IsEolChange { get; set; }
    }

    /**
     * An event describing that model decorations have changed.
     */
    public class ModelDecorationsChangedEvent
    {
        public bool AffectsMinimap { get; set; }
        public bool AffectsOverviewRuler { get; set; }
        public bool AffectsGlyphMargin { get; set; }
        public bool AffectsLineNumber { get; set; }
    }

    public class ModelOptionsChangedEvent
    {
        public bool TabSize { get; set; }
        public bool IndentSize { get; set; }
        public bool InsertSpaces { get; set; }
        public bool TrimAutoWhitespace { get; set; }
    }

    public class ModelContentChange
    {
        /**
         * The old range that got replaced.
         */
        public Range Range { get; set; }
        /**
         * The offset of the range that got replaced.
         */
        public int RangeOffset { get; set; }
        /**
         * The length of the range that got replaced.
         */
        public int RangeLength { get; set; }
        /**
         * The new text for the range.
         */
        public string Text { get; set; }
    }

    /**
     * Describes the reason the cursor has changed its position.
     */
    public enum CursorChangeReason
    {
        /**
         * Unknown or not set.
         */
        NotSet = 0,
        /**
         * A `model.setValue()` was called.
         */
        ContentFlush = 1,
        /**
         * The `model` has been changed outside of this cursor and the cursor recovers its position from associated markers.
         */
        RecoverFromMarkers = 2,
        /**
         * There was an explicit user gesture.
         */
        Explicit = 3,
        /**
         * There was a Paste.
         */
        Paste = 4,
        /**
         * There was an Undo.
         */
        Undo = 5,
        /**
         * There was a Redo.
         */
        Redo = 6
    }

    /**
     * An event describing that the cursor position has changed.
     */
    public class CursorPositionChangedEvent
    {
        /**
         * Primary cursor's position.
         */
        public Position Position { get; set; }
        /**
         * Secondary cursors' position.
         */
        public List<Position> SecondaryPositions { get; set; }
        /**
         * Reason.
         */
        public CursorChangeReason Reason { get; set; }
        /**
         * Source of the call that caused the event. Possible sources include `'mouse'`, `'keyboard'`, and `'api'`.
         */
        public string Source { get; set; }
    }

    /**
     * An event describing that the cursor selection has changed.
     */
    public class CursorSelectionChangedEvent
    {
        /**
         * The primary selection.
         */
        public Selection Selection { get; set; }
        /**
         * The secondary selections.
         */
        public List<Selection> SecondarySelections { get; set; }
        /**
         * The model version id.
         */
        public int ModelVersionId { get; set; }
        /**
         * The old selections.
         */
        public List<Selection> OldSelections { get; set; }
        /**
         * The model version id the that `oldSelections` refer to.
         */
        public int OldModelVersionId { get; set; }
        /**
         * Source of the call that caused the event. Possible sources include `'mouse'`, `'keyboard'`, and `'api'`.
         */
        public string Source { get; set; }
        /**
         * Reason.
         */
        public CursorChangeReason Reason { get; set; }
    }

    public enum AccessibilitySupport
    {
        /**
         * This should be the browser case where it is not known if a screen reader is attached or no.
         */
        Unknown = 0,
        Disabled = 1,
        Enabled = 2
    }

    /**
     * Configuration options for auto closing quotes and brackets
     */

    /**
     * Configuration options for auto wrapping quotes and brackets
     */

    /**
     * Configuration options for typing over closing quotes or brackets
     */

    /**
     * Configuration options for auto indentation in the editor
     */
    public enum EditorAutoIndentStrategy
    {
        None = 0,
        Keep = 1,
        Brackets = 2,
        Advanced = 3,
        Full = 4
    }

    /**
     * Configuration options for the editor.
     */
    public class EditorOptions
    {
        /**
         * This editor is used inside a diff editor.
         */
        public bool? InDiffEditor { get; set; }
        /**
         * This editor is allowed to use variable line heights.
         */
        public bool? AllowVariableLineHeights { get; set; }
        /**
         * This editor is allowed to use variable font-sizes and font-families
         */
        public bool? AllowVariableFonts { get; set; }
        /**
         * This editor is allowed to use variable font-sizes and font-families in accessibility mode
         */
        public bool? AllowVariableFontsInAccessibilityMode { get; set; }
        /**
         * The aria label for the editor's textarea (when it is focused).
         */
        public string AriaLabel { get; set; }
        /**
         * Whether the aria-required attribute should be set on the editors textarea.
         */
        public bool? AriaRequired { get; set; }
        /**
         * Control whether a screen reader announces inline suggestion content immediately.
         */
        public bool? ScreenReaderAnnounceInlineSuggestion { get; set; }
        /**
         * The `tabindex` property of the editor's textarea
         */
        public int? TabIndex { get; set; }
        /**
         * Render vertical lines at the specified columns.
         * Defaults to empty array.
         */
        public int[] Rulers { get; set; }
        /**
         * Locales used for segmenting lines into words when doing word related navigations or operations.
         *
         * Specify the BCP 47 language tag of the word you wish to recognize (e.g., ja, zh-CN, zh-Hant-TW, etc.).
         * Defaults to empty array
         */
        public List<string> WordSegmenterLocales { get; set; }
        /**
         * A string containing the word separators used when doing word navigation.
         * Defaults to `~!@#$%^&*()-=+[{]}\\|;:\'",.<>/?
         */
        public string WordSeparators { get; set; }
        /**
         * Enable Linux primary clipboard.
         * Defaults to true.
         */
        public bool? SelectionClipboard { get; set; }
        /**
         * Control the rendering of line numbers.
         * If it is a function, it will be invoked when rendering a line number and the return value will be rendered.
         * Otherwise, if it is a truthy, line numbers will be rendered normally (equivalent of using an identity function).
         * Otherwise, line numbers will not be rendered.
         * Defaults to `on`.
         */
        public string LineNumbers { get; set; }
        public Func<int, string> LineNumbersLambda { get; set; }
        /**
         * Controls the minimal number of visible leading and trailing lines surrounding the cursor.
         * Defaults to 0.
        */
        public int? CursorSurroundingLines { get; set; }
        /**
         * Controls when `cursorSurroundingLines` should be enforced
         * Defaults to `default`, `cursorSurroundingLines` is not enforced when cursor position is changed
         * by mouse.
        */
        public string CursorSurroundingLinesStyle { get; set; }
        /**
         * Render last line number when the file ends with a newline.
         * Defaults to 'on' for Windows and macOS and 'dimmed' for Linux.
        */
        public string RenderFinalNewline { get; set; }
        /**
         * Remove unusual line terminators like LINE SEPARATOR (LS), PARAGRAPH SEPARATOR (PS).
         * Defaults to 'prompt'.
         */
        public string UnusualLineTerminators { get; set; }
        /**
         * Should the corresponding line be selected when clicking on the line number?
         * Defaults to true.
         */
        public bool? SelectOnLineNumbers { get; set; }
        /**
         * Control the width of line numbers, by reserving horizontal space for rendering at least an amount of digits.
         * Defaults to 5.
         */
        public int? LineNumbersMinChars { get; set; }
        /**
         * Enable the rendering of the glyph margin.
         * Defaults to true in vscode and to false in monaco-editor.
         */
        public bool? GlyphMargin { get; set; }
        /**
         * The width reserved for line decorations (in px).
         * Line decorations are placed between line numbers and the editor content.
         * You can pass in a string in the format floating point followed by "ch". e.g. 1.3ch.
         * Defaults to 10.
         */
        public int? LineDecorationsWidth { get; set; }
        public string LineDecorationsWidthString { get; set; }
        /**
         * When revealing the cursor, a virtual padding (px) is added to the cursor, turning it into a rectangle.
         * This virtual padding ensures that the cursor gets revealed before hitting the edge of the viewport.
         * Defaults to 30 (px).
         */
        public int? RevealHorizontalRightPadding { get; set; }
        /**
         * Render the editor selection with rounded borders.
         * Defaults to true.
         */
        public bool? RoundedSelection { get; set; }
        /**
         * Class name to be added to the editor.
         */
        public string ExtraEditorClassName { get; set; }
        /**
         * Should the editor be read only. See also `domReadOnly`.
         * Defaults to false.
         */
        public bool? ReadOnly { get; set; }
        /**
         * The message to display when the editor is readonly.
         */
        public MarkdownString ReadOnlyMessage { get; set; }
        /**
         * Should the textarea used for input use the DOM `readonly` attribute.
         * Defaults to false.
         */
        public bool? DomReadOnly { get; set; }
        /**
         * Enable linked editing.
         * Defaults to false.
         */
        public bool? LinkedEditing { get; set; }
        /**
         * deprecated, use linkedEditing instead
         */
        public bool? RenameOnType { get; set; }
        /**
         * Should the editor render validation decorations.
         * Defaults to editable.
         */
        public string RenderValidationDecorations { get; set; }
        /**
         * Control the behavior and rendering of the scrollbars.
         */
        public EditorScrollbarOptions Scrollbar { get; set; }
        /**
         * Control the behavior of sticky scroll options
         */
        public EditorStickyScrollOptions StickyScroll { get; set; }
        /**
         * Control the behavior and rendering of the minimap.
         */
        public EditorMinimapOptions Minimap { get; set; }
        /**
         * Control the behavior of the find widget.
         */
        public EditorFindOptions Find { get; set; }
        /**
         * Display overflow widgets as `fixed`.
         * Defaults to `false`.
         */
        public bool? FixedOverflowWidgets { get; set; }
        /**
         * Allow content widgets and overflow widgets to overflow the editor viewport.
         * Defaults to `true`.
         */
        public bool? AllowOverflow { get; set; }
        /**
         * The number of vertical lanes the overview ruler should render.
         * Defaults to 3.
         */
        public int? OverviewRulerLanes { get; set; }
        /**
         * Controls if a border should be drawn around the overview ruler.
         * Defaults to `true`.
         */
        public bool? OverviewRulerBorder { get; set; }
        /**
         * Control the cursor animation style, possible values are 'blink', 'smooth', 'phase', 'expand' and 'solid'.
         * Defaults to 'blink'.
         */
        public string CursorBlinking { get; set; }
        /**
         * Zoom the font in the editor when using the mouse wheel in combination with holding Ctrl.
         * Defaults to false.
         */
        public bool? MouseWheelZoom { get; set; }
        /**
         * Control the mouse pointer style, either 'text' or 'default' or 'copy'
         * Defaults to 'text'
         */
        public string MouseStyle { get; set; }
        /**
         * Enable smooth caret animation.
         * Defaults to 'off'.
         */
        public string CursorSmoothCaretAnimation { get; set; }
        /**
         * Control the cursor style in insert mode.
         * Defaults to 'line'.
         */
        public string CursorStyle { get; set; }
        /**
         * Control the cursor style in overtype mode.
         * Defaults to 'block'.
         */
        public string OvertypeCursorStyle { get; set; }
        /**
         *  Controls whether paste in overtype mode should overwrite or insert.
         */
        public bool? OvertypeOnPaste { get; set; }
        /**
         * Control the width of the cursor when cursorStyle is set to 'line'
         */
        public int? CursorWidth { get; set; }
        /**
         * Control the height of the cursor when cursorStyle is set to 'line'
         */
        public int? CursorHeight { get; set; }
        /**
         * Enable font ligatures.
         * Defaults to false.
         */
        public object FontLigatures { get; set; }
        /**
         * Enable font variations.
         * Defaults to false.
         */
        public object FontVariations { get; set; }
        /**
         * Controls whether to use default color decorations or not using the default document color provider
         */
        public string DefaultColorDecorators { get; set; }
        /**
         * Disable the use of `transform: translate3d(0px, 0px, 0px)` for the editor margin and lines layers.
         * The usage of `transform: translate3d(0px, 0px, 0px)` acts as a hint for browsers to create an extra layer.
         * Defaults to false.
         */
        public bool? DisableLayerHinting { get; set; }
        /**
         * Disable the optimizations for monospace fonts.
         * Defaults to false.
         */
        public bool? DisableMonospaceOptimizations { get; set; }
        /**
         * Should the cursor be hidden in the overview ruler.
         * Defaults to false.
         */
        public bool? HideCursorInOverviewRuler { get; set; }
        /**
         * Enable that scrolling can go one screen size after the last line.
         * Defaults to true.
         */
        public bool? ScrollBeyondLastLine { get; set; }
        /**
         * Scroll editor on middle click
         */
        public bool? ScrollOnMiddleClick { get; set; }
        /**
         * Enable that scrolling can go beyond the last column by a number of columns.
         * Defaults to 5.
         */
        public int? ScrollBeyondLastColumn { get; set; }
        /**
         * Enable that the editor animates scrolling to a position.
         * Defaults to false.
         */
        public bool? SmoothScrolling { get; set; }
        /**
         * Enable that the editor will install a ResizeObserver to check if its container dom node size has changed.
         * Defaults to false.
         */
        public bool? AutomaticLayout { get; set; }
        /**
         * Control the wrapping of the editor.
         * When `wordWrap` = "off", the lines will never wrap.
         * When `wordWrap` = "on", the lines will wrap at the viewport width.
         * When `wordWrap` = "wordWrapColumn", the lines will wrap at `wordWrapColumn`.
         * When `wordWrap` = "bounded", the lines will wrap at min(viewport width, wordWrapColumn).
         * Defaults to "off".
         */
        public string WordWrap { get; set; }
        /**
         * Control whether an indicator is rendered at the wrapping column of soft wrapped lines.
         * Defaults to false.
         */
        public bool? WordWrapIndicator { get; set; }
        /**
         * Override the `wordWrap` setting.
         */
        public string WordWrapOverride1 { get; set; }
        /**
         * Override the `wordWrapOverride1` setting.
         */
        public string WordWrapOverride2 { get; set; }
        /**
         * Control the wrapping of the editor.
         * When `wordWrap` = "off", the lines will never wrap.
         * When `wordWrap` = "on", the lines will wrap at the viewport width.
         * When `wordWrap` = "wordWrapColumn", the lines will wrap at `wordWrapColumn`.
         * When `wordWrap` = "bounded", the lines will wrap at min(viewport width, wordWrapColumn).
         * Defaults to 80.
         */
        public int? WordWrapColumn { get; set; }
        /**
         * Control indentation of wrapped lines. Can be: 'none', 'same', 'indent' or 'deepIndent'.
         * Defaults to 'same' in vscode and to 'none' in monaco-editor.
         */
        public string WrappingIndent { get; set; }
        /**
         * Controls the wrapping strategy to use.
         * Defaults to 'simple'.
         */
        public string WrappingStrategy { get; set; }
        /**
         * Create a softwrap on every quoted "\n" literal.
         * Defaults to false.
         */
        public bool? WrapOnEscapedLineFeeds { get; set; }
        /**
         * Configure word wrapping characters. A break will be introduced before these characters.
         */
        public string WordWrapBreakBeforeCharacters { get; set; }
        /**
         * Configure word wrapping characters. A break will be introduced after these characters.
         */
        public string WordWrapBreakAfterCharacters { get; set; }
        /**
         * Sets whether line breaks appear wherever the text would otherwise overflow its content box.
         * When wordBreak = 'normal', Use the default line break rule.
         * When wordBreak = 'keepAll', Word breaks should not be used for Chinese/Japanese/Korean (CJK) text. Non-CJK text behavior is the same as for normal.
         */
        public string WordBreak { get; set; }
        /**
         * Performance guard: Stop rendering a line after x characters.
         * Defaults to 10000.
         * Use -1 to never stop rendering
         */
        public int? StopRenderingLineAfter { get; set; }
        /**
         * Configure the editor's hover.
         */
        public EditorHoverOptions Hover { get; set; }
        /**
         * Enable detecting links and making them clickable.
         * Defaults to true.
         */
        public bool? Links { get; set; }
        /**
         * Enable inline color decorators and color picker rendering.
         */
        public bool? ColorDecorators { get; set; }
        /**
         * Controls what is the condition to spawn a color picker from a color dectorator
         */
        public string ColorDecoratorsActivatedOn { get; set; }
        /**
         * Controls the max number of color decorators that can be rendered in an editor at once.
         */
        public int? ColorDecoratorsLimit { get; set; }
        /**
         * Control the behaviour of comments in the editor.
         */
        public EditorCommentsOptions Comments { get; set; }
        /**
         * Enable custom contextmenu.
         * Defaults to true.
         */
        public bool? Contextmenu { get; set; }
        /**
         * A multiplier to be used on the `deltaX` and `deltaY` of mouse wheel scroll events.
         * Defaults to 1.
         */
        public int? MouseWheelScrollSensitivity { get; set; }
        /**
         * FastScrolling mulitplier speed when pressing `Alt`
         * Defaults to 5.
         */
        public int? FastScrollSensitivity { get; set; }
        /**
         * Enable that the editor scrolls only the predominant axis. Prevents horizontal drift when scrolling vertically on a trackpad.
         * Defaults to true.
         */
        public bool? ScrollPredominantAxis { get; set; }
        /**
         * Make scrolling inertial - mostly useful with touchpad on linux.
         */
        public bool? InertialScroll { get; set; }
        /**
         * Enable that the selection with the mouse and keys is doing column selection.
         * Defaults to false.
         */
        public bool? ColumnSelection { get; set; }
        /**
         * The modifier to be used to add multiple cursors with the mouse.
         * Defaults to 'alt'
         */
        public string MultiCursorModifier { get; set; }
        /**
         * Merge overlapping selections.
         * Defaults to true
         */
        public bool? MultiCursorMergeOverlapping { get; set; }
        /**
         * Configure the behaviour when pasting a text with the line count equal to the cursor count.
         * Defaults to 'spread'.
         */
        public string MultiCursorPaste { get; set; }
        /**
         * Controls the max number of text cursors that can be in an active editor at once.
         */
        public int? MultiCursorLimit { get; set; }
        /**
         * Enables middle mouse button to open links and Go To Definition
         */
        public string MouseMiddleClickAction { get; set; }
        /**
         * Configure the editor's accessibility support.
         * Defaults to 'auto'. It is best to leave this to 'auto'.
         */
        public string AccessibilitySupport { get; set; }
        /**
         * Controls the number of lines in the editor that can be read out by a screen reader
         */
        public int? AccessibilityPageSize { get; set; }
        /**
         * Suggest options.
         */
        public SuggestOptions Suggest { get; set; }
        public InlineSuggestOptions InlineSuggest { get; set; }
        /**
         * Smart select options.
         */
        public SmartSelectOptions SmartSelect { get; set; }
        /**
         *
         */
        public GotoLocationOptions GotoLocation { get; set; }
        /**
         * Enable quick suggestions (shadow suggestions)
         * Defaults to true.
         */
        public object QuickSuggestions { get; set; }
        /**
         * Quick suggestions show delay (in ms)
         * Defaults to 10 (ms)
         */
        public int? QuickSuggestionsDelay { get; set; }
        /**
         * Controls the spacing around the editor.
         */
        public EditorPaddingOptions Padding { get; set; }
        /**
         * Parameter hint options.
         */
        public EditorParameterHintOptions ParameterHints { get; set; }
        /**
         * Options for auto closing brackets.
         * Defaults to language defined behavior.
         */
        public string AutoClosingBrackets { get; set; }
        /**
         * Options for auto closing comments.
         * Defaults to language defined behavior.
         */
        public string AutoClosingComments { get; set; }
        /**
         * Options for auto closing quotes.
         * Defaults to language defined behavior.
         */
        public string AutoClosingQuotes { get; set; }
        /**
         * Options for pressing backspace near quotes or bracket pairs.
         */
        public string AutoClosingDelete { get; set; }
        /**
         * Options for typing over closing quotes or brackets.
         */
        public string AutoClosingOvertype { get; set; }
        /**
         * Options for auto surrounding.
         * Defaults to always allowing auto surrounding.
         */
        public string AutoSurround { get; set; }
        /**
         * Controls whether the editor should automatically adjust the indentation when users type, paste, move or indent lines.
         * Defaults to advanced.
         */
        public string AutoIndent { get; set; }
        /**
         * Boolean which controls whether to autoindent on paste
         */
        public bool? AutoIndentOnPaste { get; set; }
        /**
         * Boolean which controls whether to autoindent on paste within a string when autoIndentOnPaste is enabled.
         */
        public bool? AutoIndentOnPasteWithinString { get; set; }
        /**
         * Emulate selection behaviour of tab characters when using spaces for indentation.
         * This means selection will stick to tab stops.
         */
        public bool? StickyTabStops { get; set; }
        /**
         * Enable format on type.
         * Defaults to false.
         */
        public bool? FormatOnType { get; set; }
        /**
         * Enable format on paste.
         * Defaults to false.
         */
        public bool? FormatOnPaste { get; set; }
        /**
         * Controls the width used to render full-width characters.
         * Defaults to 'font'.
         */
        public string FullwidthCharacterWidth { get; set; }
        /**
         * Controls whether double-clicking next to a bracket or quote selects the content inside.
         * Defaults to true.
         */
        public bool? DoubleClickSelectsBlock { get; set; }
        /**
         * Controls if the editor should allow to move selections via drag and drop.
         * Defaults to false.
         */
        public bool? DragAndDrop { get; set; }
        /**
         * Enable the suggestion box to pop-up on trigger characters.
         * Defaults to true.
         */
        public bool? SuggestOnTriggerCharacters { get; set; }
        /**
         * Accept suggestions on ENTER.
         * Defaults to 'on'.
         */
        public string AcceptSuggestionOnEnter { get; set; }
        /**
         * Accept suggestions on provider defined characters.
         * Defaults to true.
         */
        public bool? AcceptSuggestionOnCommitCharacter { get; set; }
        /**
         * Enable snippet suggestions. Default to 'true'.
         */
        public string SnippetSuggestions { get; set; }
        /**
         * Copying without a selection copies the current line.
         */
        public bool? EmptySelectionClipboard { get; set; }
        /**
         * Syntax highlighting is copied.
         */
        public bool? CopyWithSyntaxHighlighting { get; set; }
        /**
         * The history mode for suggestions.
         */
        public string SuggestSelection { get; set; }
        /**
         * The font size for the suggest widget.
         * Defaults to the editor font size.
         */
        public int? SuggestFontSize { get; set; }
        /**
         * The line height for the suggest widget.
         * Defaults to the editor line height.
         */
        public int? SuggestLineHeight { get; set; }
        /**
         * Enable tab completion.
         */
        public string TabCompletion { get; set; }
        /**
         * Enable selection highlight.
         * Defaults to true.
         */
        public bool? SelectionHighlight { get; set; }
        /**
         * Enable selection highlight for multiline selections.
         * Defaults to false.
         */
        public bool? SelectionHighlightMultiline { get; set; }
        /**
         * Maximum length (in characters) for selection highlights.
         * Set to 0 to have an unlimited length.
         */
        public int? SelectionHighlightMaxLength { get; set; }
        /**
         * Enable semantic occurrences highlight.
         * Defaults to 'singleFile'.
         * 'off' disables occurrence highlighting
         * 'singleFile' triggers occurrence highlighting in the current document
         * 'multiFile'  triggers occurrence highlighting across valid open documents
         */
        public string OccurrencesHighlight { get; set; }
        /**
         * Controls delay for occurrences highlighting
         * Defaults to 250.
         * Minimum value is 0
         * Maximum value is 2000
         */
        public int? OccurrencesHighlightDelay { get; set; }
        /**
         * Show code lens
         * Defaults to true.
         */
        public bool? CodeLens { get; set; }
        /**
         * Code lens font family. Defaults to editor font family.
         */
        public string CodeLensFontFamily { get; set; }
        /**
         * Code lens font size. Default to 90% of the editor font size
         */
        public int? CodeLensFontSize { get; set; }
        /**
         * Control the behavior and rendering of the code action lightbulb.
         */
        public EditorLightbulbOptions Lightbulb { get; set; }
        /**
         * Timeout for running code actions on save.
         */
        public int? CodeActionsOnSaveTimeout { get; set; }
        /**
         * Enable code folding.
         * Defaults to true.
         */
        public bool? Folding { get; set; }
        /**
         * Selects the folding strategy. 'auto' uses the strategies contributed for the current document, 'indentation' uses the indentation based folding strategy.
         * Defaults to 'auto'.
         */
        public string FoldingStrategy { get; set; }
        /**
         * Enable highlight for folded regions.
         * Defaults to true.
         */
        public bool? FoldingHighlight { get; set; }
        /**
         * Auto fold imports folding regions.
         * Defaults to true.
         */
        public bool? FoldingImportsByDefault { get; set; }
        /**
         * Maximum number of foldable regions.
         * Defaults to 5000.
         */
        public int? FoldingMaximumRegions { get; set; }
        /**
         * Controls whether the fold actions in the gutter stay always visible or hide unless the mouse is over the gutter.
         * Defaults to 'mouseover'.
         */
        public string ShowFoldingControls { get; set; }
        /**
         * Controls whether clicking on the empty content after a folded line will unfold the line.
         * Defaults to false.
         */
        public bool? UnfoldOnClickAfterEndOfLine { get; set; }
        /**
         * Enable highlighting of matching brackets.
         * Defaults to 'always'.
         */
        public string MatchBrackets { get; set; }
        /**
         * Enable experimental rendering using WebGPU.
         * Defaults to 'off'.
         */
        public string ExperimentalGpuAcceleration { get; set; }
        /**
         * Enable experimental whitespace rendering.
         * Defaults to 'svg'.
         */
        public string ExperimentalWhitespaceRendering { get; set; }
        /**
         * Enable rendering of whitespace.
         * Defaults to 'selection'.
         */
        public string RenderWhitespace { get; set; }
        /**
         * Enable rendering of control characters.
         * Defaults to true.
         */
        public bool? RenderControlCharacters { get; set; }
        /**
         * Enable rendering of current line highlight.
         * Defaults to all.
         */
        public string RenderLineHighlight { get; set; }
        /**
         * Control if the current line highlight should be rendered only the editor is focused.
         * Defaults to false.
         */
        public bool? RenderLineHighlightOnlyWhenFocus { get; set; }
        /**
         * Inserting and deleting whitespace follows tab stops.
         */
        public bool? UseTabStops { get; set; }
        /**
         * Controls whether the editor should automatically remove indentation whitespace when joining lines with Delete.
         * Defaults to false.
         */
        public bool? TrimWhitespaceOnDelete { get; set; }
        /**
         * The font family
         */
        public string FontFamily { get; set; }
        /**
         * The font weight
         */
        public string FontWeight { get; set; }
        /**
         * The font size
         */
        public int? FontSize { get; set; }
        /**
         * The line height
         */
        public int? LineHeight { get; set; }
        /**
         * The letter spacing
         */
        public int? LetterSpacing { get; set; }
        /**
         * Controls fading out of unused variables.
         */
        public bool? ShowUnused { get; set; }
        /**
         * Controls whether to focus the inline editor in the peek widget by default.
         * Defaults to false.
         */
        public string PeekWidgetDefaultFocus { get; set; }
        /**
         * Sets a placeholder for the editor.
         * If set, the placeholder is shown if the editor is empty.
        */
        public string Placeholder { get; set; }
        /**
         * Controls whether the definition link opens element in the peek widget.
         * Defaults to false.
         */
        public bool? DefinitionLinkOpensInPeek { get; set; }
        /**
         * Controls strikethrough deprecated variables.
         */
        public bool? ShowDeprecated { get; set; }
        /**
         * Controls whether suggestions allow matches in the middle of the word instead of only at the beginning
         */
        public bool? MatchOnWordStartOnly { get; set; }
        /**
         * Control the behavior and rendering of the inline hints.
         */
        public EditorInlayHintsOptions InlayHints { get; set; }
        /**
         * Control if the editor should use shadow DOM.
         */
        public bool? UseShadowDOM { get; set; }
        /**
         * Controls the behavior of editor guides.
        */
        public GuidesOptions Guides { get; set; }
        /**
         * Controls the behavior of the unicode highlight feature
         * (by default, ambiguous and invisible characters are highlighted).
         */
        public UnicodeHighlightOptions UnicodeHighlight { get; set; }
        /**
         * Configures bracket pair colorization (disabled by default).
        */
        public BracketPairColorizationOptions BracketPairColorization { get; set; }
        /**
         * Controls dropping into the editor from an external source.
         *
         * When enabled, this shows a preview of the drop location and triggers an `onDropIntoEditor` event.
         */
        public DropIntoEditorOptions DropIntoEditor { get; set; }
        /**
         * Sets whether the new experimental edit context should be used instead of the text area.
         */
        public bool? EditContext { get; set; }
        /**
         * Controls whether to render rich HTML screen reader content when the EditContext is enabled
         */
        public bool? RenderRichScreenReaderContent { get; set; }
        /**
         * Controls support for changing how content is pasted into the editor.
         */
        public PasteAsOptions PasteAs { get; set; }
        /**
         * Controls whether the editor / terminal receives tabs or defers them to the workbench for navigation.
         */
        public bool? TabFocusMode { get; set; }
        /**
         * Controls whether the accessibility hint should be provided to screen reader users when an inline completion is shown.
         */
        public bool? InlineCompletionsAccessibilityVerbose { get; set; }
    }

    public static class DiffEditorViewMode
    {
        public const string Inline = "inline";
        public const string SideBySide = "sideBySide";
        public const string Automatic = "automatic";
    }

    public interface IDiffEditorBaseOptions
    {
        /**
         * Allow the user to resize the diff editor split view.
         * Defaults to true.
         */
        bool? EnableSplitViewResizing { get; set; }
        /**
         * The default ratio when rendering side-by-side editors.
         * Must be a number between 0 and 1, min sizes apply.
         * Defaults to 0.5
         */
        int? SplitViewDefaultRatio { get; set; }
        /**
         * Render the differences in two side-by-side editors.
         * Defaults to true.
         */
        bool? RenderSideBySide { get; set; }
        /**
         * When `renderSideBySide` is enabled, `useInlineViewWhenSpaceIsLimited` is set,
         * and the diff editor has a width less than `renderSideBySideInlineBreakpoint`, the inline view is used.
         */
        int? RenderSideBySideInlineBreakpoint { get; set; }
        /**
         * When `renderSideBySide` is enabled, `useInlineViewWhenSpaceIsLimited` is set,
         * and the diff editor has a width less than `renderSideBySideInlineBreakpoint`, the inline view is used.
         */
        bool? UseInlineViewWhenSpaceIsLimited { get; set; }
        /**
         * If set, the diff editor is optimized for small views.
         * Defaults to `false`.
        */
        bool? CompactMode { get; set; }
        /**
         * Timeout in milliseconds after which diff computation is cancelled.
         * Defaults to 5000.
         */
        int? MaxComputationTime { get; set; }
        /**
         * Maximum supported file size in MB.
         * Defaults to 50.
         */
        int? MaxFileSize { get; set; }
        /**
         * Compute the diff by ignoring leading/trailing whitespace
         * Defaults to true.
         */
        bool? IgnoreTrimWhitespace { get; set; }
        /**
         * Render +/- indicators for added/deleted changes.
         * Defaults to true.
         */
        bool? RenderIndicators { get; set; }
        /**
         * Shows icons in the glyph margin to revert changes.
         * Default to true.
         */
        bool? RenderMarginRevertIcon { get; set; }
        /**
         * Indicates if the gutter menu should be rendered.
        */
        bool? RenderGutterMenu { get; set; }
        /**
         * Original model should be editable?
         * Defaults to false.
         */
        bool? OriginalEditable { get; set; }
        /**
         * Should the diff editor enable code lens?
         * Defaults to false.
         */
        bool? DiffCodeLens { get; set; }
        /**
         * Is the diff editor should render overview ruler
         * Defaults to true
         */
        bool? RenderOverviewRuler { get; set; }
        /**
         * Control the wrapping of the diff editor.
         */
        string DiffWordWrap { get; set; }
        /**
         * Diff Algorithm
        */
        string DiffAlgorithm { get; set; }
        /**
         * Whether the diff editor aria label should be verbose.
         */
        bool? AccessibilityVerbose { get; set; }
        /*experimental?: {
            /**
             * Defaults to false.
             * /
            showMoves?: boolean;
            showEmptyDecorations?: boolean;
            /**
             * Only applies when `renderSideBySide` is set to false.
            * /
            useTrueInlineView?: boolean;
        };*/
        /**
         * Is the diff editor inside another editor
         * Defaults to false
         */
        bool? IsInEmbeddedEditor { get; set; }
        /**
         * If the diff editor should only show the difference review mode.
         */
        bool? OnlyShowAccessibleDiffViewer { get; set; }
        /*hideUnchangedRegions?: {
            enabled?: boolean;
            revealLineCount?: number;
            minimumLineCount?: number;
            contextLineCount?: number;
        };*/
    }

    /**
     * Configuration options for the diff editor.
     */
    public class DiffEditorOptions : EditorOptions, IDiffEditorBaseOptions
    {
        public bool? EnableSplitViewResizing { get; set; }
        public int? SplitViewDefaultRatio { get; set; }
        public bool? RenderSideBySide { get; set; }
        public int? RenderSideBySideInlineBreakpoint { get; set; }
        public bool? UseInlineViewWhenSpaceIsLimited { get; set; }
        public bool? CompactMode { get; set; }
        public int? MaxComputationTime { get; set; }
        public int? MaxFileSize { get; set; }
        public bool? IgnoreTrimWhitespace { get; set; }
        public bool? RenderIndicators { get; set; }
        public bool? RenderMarginRevertIcon { get; set; }
        public bool? RenderGutterMenu { get; set; }
        public bool? OriginalEditable { get; set; }
        public bool? DiffCodeLens { get; set; }
        public bool? RenderOverviewRuler { get; set; }
        public string DiffWordWrap { get; set; }
        public string DiffAlgorithm { get; set; }
        public bool? AccessibilityVerbose { get; set; }
        public bool? IsInEmbeddedEditor { get; set; }
        public bool? OnlyShowAccessibleDiffViewer { get; set; }
    }

    /**
     * An event describing that the configuration of the editor has changed.
     */
    public class ConfigurationChangedEvent
    {
        private readonly List<bool> _options;
        public ConfigurationChangedEvent(List<bool> options)
        {
            _options = options;
        }
        public bool HasChanged(EditorOption id)
        {
            return _options[(int)id];
        }
    }

    /**
     * All computed editor options.
     */
    public class ComputedEditorOptions
    {
        private readonly List<string> _options;
        public ComputedEditorOptions(List<string> options)
        {
            _options = options;
        }
        public T Get<T>(EditorOption id)
        {
            return JsonSerializer.Deserialize<T>(_options[(int)id]);
        }
    }

    public class IEditorOption<V>
    {
        public EditorOption Id { get; set; }
        public string Name { get; set; }
        public V DefaultValue { get; set; }
        /**
         * Might modify `value`.
        */
    }

    /*public class ApplyUpdateResult<T> {
        public T NewValue { get; }
        public bool DidChange { get; }
        public ApplyUpdateResult(T newValue, bool didChange)
        {
            NewValue = newValue;
            DidChange = didChange;
        }
    }*/

    /**
     * Configuration options for editor comments
     */
    public class EditorCommentsOptions
    {
        /**
         * Insert a space after the line comment token and inside the block comments tokens.
         * Defaults to true.
         */
        public bool? InsertSpace { get; set; }
        /**
         * Ignore empty lines when inserting line comments.
         * Defaults to true.
         */
        public bool? IgnoreEmptyLines { get; set; }
    }

    /**
     * The kind of animation in which the editor's cursor should be rendered.
     */
    public enum TextEditorCursorBlinkingStyle
    {
        /**
         * Hidden
         */
        Hidden = 0,
        /**
         * Blinking
         */
        Blink = 1,
        /**
         * Blinking with smooth fading
         */
        Smooth = 2,
        /**
         * Blinking with prolonged filled state and smooth fading
         */
        Phase = 3,
        /**
         * Expand collapse animation on the y axis
         */
        Expand = 4,
        /**
         * No-Blinking
         */
        Solid = 5
    }

    /**
     * The style in which the editor's cursor should be rendered.
     */
    public enum TextEditorCursorStyle
    {
        /**
         * As a vertical line (sitting between two characters).
         */
        Line = 1,
        /**
         * As a block (sitting on top of a character).
         */
        Block = 2,
        /**
         * As a horizontal line (sitting under a character).
         */
        Underline = 3,
        /**
         * As a thin vertical line (sitting between two characters).
         */
        LineThin = 4,
        /**
         * As an outlined block (sitting on top of a character).
         */
        BlockOutline = 5,
        /**
         * As a thin horizontal line (sitting under a character).
         */
        UnderlineThin = 6
    }

    /**
     * Configuration options for editor find widget
     */
    public class EditorFindOptions
    {
        /**
        * Controls whether the cursor should move to find matches while typing.
        */
        public bool? CursorMoveOnType { get; set; }
        /**
         * Controls whether the find widget should search as you type.
         */
        public bool? FindOnType { get; set; }
        /**
         * Controls if we seed search string in the Find Widget with editor selection.
         */
        public string SeedSearchStringFromSelection { get; set; }
        /**
         * Controls if Find in Selection flag is turned on in the editor.
         */
        public string AutoFindInSelection { get; set; }
        public bool? AddExtraSpaceOnTop { get; set; }
        /**
         * Controls whether the search result and diff result automatically restarts from the beginning (or the end) when no further matches can be found
         */
        public bool? Loop { get; set; }
        /**
         * Controls whether to close the Find Widget after an explicit find navigation command lands on a match.
         */
        public bool? CloseOnResult { get; set; }
    }


    /**
     * Configuration options for go to location
     */
    public class GotoLocationOptions
    {
        public string Multiple { get; set; }
        public string MultipleDefinitions { get; set; }
        public string MultipleTypeDefinitions { get; set; }
        public string MultipleDeclarations { get; set; }
        public string MultipleImplementations { get; set; }
        public string MultipleReferences { get; set; }
        public string MultipleTests { get; set; }
        public string AlternativeDefinitionCommand { get; set; }
        public string AlternativeTypeDefinitionCommand { get; set; }
        public string AlternativeDeclarationCommand { get; set; }
        public string AlternativeImplementationCommand { get; set; }
        public string AlternativeReferenceCommand { get; set; }
        public string AlternativeTestsCommand { get; set; }
    }

    /**
     * Configuration options for editor hover
     */
    public class EditorHoverOptions
    {
        /**
         * Enable the hover.
         * Defaults to 'on'.
         */
        public string Enabled { get; set; }
        /**
         * Delay for showing the hover.
         * Defaults to 300.
         */
        public int? Delay { get; set; }
        /**
         * Is the hover sticky such that it can be clicked and its contents selected?
         * Defaults to true.
         */
        public bool? Sticky { get; set; }
        /**
         * Controls how long the hover is visible after you hovered out of it.
         * Require sticky setting to be true.
         */
        public int? HidingDelay { get; set; }
        /**
         * Should the hover be shown above the line if possible?
         * Defaults to false.
         */
        public bool? Above { get; set; }
        /**
         * Should long line warning hovers be shown (tokenization skipped, rendering paused)?
         * Defaults to true.
         */
        public bool? ShowLongLineWarning { get; set; }
    }

    /**
     * A description for the overview ruler position.
     */
    public class OverviewRulerPosition
    {
        /**
         * Width of the overview ruler
         */
        public double Width { get; set; }
        /**
         * Height of the overview ruler
         */
        public double Height { get; set; }
        /**
         * Top position for the overview ruler
         */
        public double Top { get; set; }
        /**
         * Right position for the overview ruler
         */
        public double Right { get; set; }
    }

    public enum RenderMinimap
    {
        None = 0,
        Text = 1,
        Blocks = 2
    }

    /**
     * The internal layout details of the editor.
     */
    public class EditorLayoutInfo
    {
        /**
         * Full editor width.
         */
        public float Width { get; set; }
        /**
         * Full editor height.
         */
        public float Height { get; set; }
        /**
         * Left position for the glyph margin.
         */
        public float GlyphMarginLeft { get; set; }
        /**
         * The width of the glyph margin.
         */
        public float GlyphMarginWidth { get; set; }
        /**
         * The number of decoration lanes to render in the glyph margin.
         */
        public int GlyphMarginDecorationLaneCount { get; set; }
        /**
         * Left position for the line numbers.
         */
        public float LineNumbersLeft { get; set; }
        /**
         * The width of the line numbers.
         */
        public float LineNumbersWidth { get; set; }
        /**
         * Left position for the line decorations.
         */
        public float DecorationsLeft { get; set; }
        /**
         * The width of the line decorations.
         */
        public float DecorationsWidth { get; set; }
        /**
         * Left position for the content (actual text)
         */
        public float ContentLeft { get; set; }
        /**
         * The width of the content (actual text)
         */
        public float ContentWidth { get; set; }
        /**
         * Layout information for the minimap
         */
        public EditorMinimapLayoutInfo Minimap { get; set; }
        /**
         * The number of columns (of typical characters) fitting on a viewport line.
         */
        public float ViewportColumn { get; set; }
        public bool IsWordWrapMinified { get; set; }
        public bool IsViewportWrapping { get; set; }
        public float WrappingColumn { get; set; }
        /**
         * The width of the vertical scrollbar.
         */
        public float VerticalScrollbarWidth { get; set; }
        /**
         * The height of the horizontal scrollbar.
         */
        public float HorizontalScrollbarHeight { get; set; }
        /**
         * The position of the overview ruler.
         */
        public OverviewRulerPosition OverviewRuler { get; set; }
    }

    /**
     * The internal layout details of the editor.
     */
    public class EditorMinimapLayoutInfo
    {
        public RenderMinimap RenderMinimap { get; set; }
        public float MinimapLeft { get; set; }
        public float MinimapWidth { get; set; }
        public bool MinimapHeightIsEditorHeight { get; set; }
        public bool MinimapIsSampling { get; set; }
        public float MinimapScale { get; set; }
        public float MinimapLineHeight { get; set; }
        public float MinimapCanvasInnerWidth { get; set; }
        public float MinimapCanvasInnerHeight { get; set; }
        public float MinimapCanvasOuterWidth { get; set; }
        public float MinimapCanvasOuterHeight { get; set; }
    }

    public static class ShowLightbulbIconMode
    {
        public const string Off = "off";
        public const string OnCode = "onCode";
        public const string On = "on";
    }

    /**
     * Configuration options for editor lightbulb
     */
    public class EditorLightbulbOptions
    {
        /**
         * Enable the lightbulb code action.
         * The three possible values are `off`, `on` and `onCode` and the default is `onCode`.
         * `off` disables the code action menu.
         * `on` shows the code action menu on code and on empty lines.
         * `onCode` shows the code action menu on code only.
         */
        public string Enabled { get; set; }
    }

    public class EditorStickyScrollOptions
    {
        /**
         * Enable the sticky scroll
         */
        public bool? Enabled { get; set; }
        /**
         * Maximum number of sticky lines to show
         */
        public int? MaxLineCount { get; set; }
        /**
         * Model to choose for sticky scroll by default
         */
        public string DefaultModel { get; set; }
        /**
         * Define whether to scroll sticky scroll with editor horizontal scrollbae
         */
        public bool? ScrollWithEditor { get; set; }
    }

    /**
     * Configuration options for editor inlayHints
     */
    public class EditorInlayHintsOptions
    {
        /**
         * Enable the inline hints.
         * Defaults to true.
         */
        public string Enabled { get; set; }
        /**
         * Font size of inline hints.
         * Default to 90% of the editor font size.
         */
        public float? FontSize { get; set; }
        /**
         * Font family of inline hints.
         * Defaults to editor font family.
         */
        public string FontFamily { get; set; }
        /**
         * Enables the padding around the inlay hint.
         * Defaults to false.
         */
        public bool? Padding { get; set; }
        /**
         * Maximum length for inlay hints per line
         * Set to 0 to have an unlimited length.
         */
        public int? MaximumLength { get; set; }
    }

    /**
     * Configuration options for editor minimap
     */
    public class EditorMinimapOptions
    {
        /**
         * Enable the rendering of the minimap.
         * Defaults to true.
         */
        public bool? Enabled { get; set; }
        /**
         * Control the rendering of minimap.
         */
        public string Autohide { get; set; }
        /**
         * Control the side of the minimap in editor.
         * Defaults to 'right'.
         */
        public string Side { get; set; }
        /**
         * Control the minimap rendering mode.
         * Defaults to 'actual'.
         */
        public string Size { get; set; }
        /**
         * Control the rendering of the minimap slider.
         * Defaults to 'mouseover'.
         */
        public string ShowSlider { get; set; }
        /**
         * Render the actual text on a line (as opposed to color blocks).
         * Defaults to true.
         */
        public bool? RenderCharacters { get; set; }
        /**
         * Limit the width of the minimap to render at most a certain number of columns.
         * Defaults to 120.
         */
        public int? MaxColumn { get; set; }
        /**
         * Relative size of the font in the minimap. Defaults to 1.
         */
        public float? Scale { get; set; }
        /**
         * Whether to show named regions as section headers. Defaults to true.
         */
        public bool? ShowRegionSectionHeaders { get; set; }
        /**
         * Whether to show MARK: comments as section headers. Defaults to true.
         */
        public bool? ShowMarkSectionHeaders { get; set; }
        /**
         * When specified, is used to create a custom section header parser regexp.
         * Must contain a match group named 'label' (written as (?<label>.+)) that encapsulates the section header.
         * Optionally can include another match group named 'separator'.
         * To match multi-line headers like:
         *   // ==========
         *   // My Section
         *   // ==========
         * Use a pattern like: ^={3,}\n^\/\/ *(?<label>[^\n]*?)\n^={3,}$
         */
        public string MarkSectionHeaderRegex { get; set; }
        /**
         * Font size of section headers. Defaults to 9.
         */
        public int? SectionHeaderFontSize { get; set; }
        /**
         * Spacing between the section header characters (in CSS px). Defaults to 1.
         */
        public int? SectionHeaderLetterSpacing { get; set; }
    }

    /**
     * Configuration options for editor padding
     */
    public class EditorPaddingOptions
    {
        /**
         * Spacing between top edge of editor and first line.
         */
        public float? Top { get; set; }
        /**
         * Spacing between bottom edge of editor and last line.
         */
        public float? Bottom { get; set; }
    }

    /**
     * Configuration options for parameter hints
     */
    public class EditorParameterHintOptions
    {
        /**
         * Enable parameter hints.
         * Defaults to true.
         */
        public bool? Enabled { get; set; }
        /**
         * Enable cycling of parameter hints.
         * Defaults to false.
         */
        public bool? Cycle { get; set; }
    }

    public static class QuickSuggestionsValue
    {
        public const string On = "on";
        public const string Inline = "inline";
        public const string Off = "off";
        public const string OffWhenInlineCompletions = "offWhenInlineCompletions";
    }

    /**
     * Configuration options for quick suggestions
     */
    public class QuickSuggestionsOptions
    {
        public string Other { get; set; }
        public string Comments { get; set; }
        public string Strings { get; set; }
    }


    public enum RenderLineNumbersType
    {
        Off = 0,
        On = 1,
        Relative = 2,
        Interval = 3,
        Custom = 4
    }


    /**
     * Configuration options for editor scrollbars
     */
    public class EditorScrollbarOptions
    {
        /**
         * The size of arrows (if displayed).
         * Defaults to 11.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public int? ArrowSize { get; set; }
        /**
         * Render vertical scrollbar.
         * Defaults to 'auto'.
         */
        public string Vertical { get; set; }
        /**
         * Render horizontal scrollbar.
         * Defaults to 'auto'.
         */
        public string Horizontal { get; set; }
        /**
         * Cast horizontal and vertical shadows when the content is scrolled.
         * Defaults to true.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public bool? UseShadows { get; set; }
        /**
         * Render arrows at the top and bottom of the vertical scrollbar.
         * Defaults to false.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public bool? VerticalHasArrows { get; set; }
        /**
         * Render arrows at the left and right of the horizontal scrollbar.
         * Defaults to false.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public bool? HorizontalHasArrows { get; set; }
        /**
         * Listen to mouse wheel events and react to them by scrolling.
         * Defaults to true.
         */
        public bool? HandleMouseWheel { get; set; }
        /**
         * Always consume mouse wheel events (always call preventDefault() and stopPropagation() on the browser events).
         * Defaults to true.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public bool? AlwaysConsumeMouseWheel { get; set; }
        /**
         * Height in pixels for the horizontal scrollbar.
         * Defaults to 12 (px).
         */
        public int? HorizontalScrollbarSize { get; set; }
        /**
         * Width in pixels for the vertical scrollbar.
         * Defaults to 14 (px).
         */
        public int? VerticalScrollbarSize { get; set; }
        /**
         * Width in pixels for the vertical slider.
         * Defaults to `verticalScrollbarSize`.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public int? VerticalSliderSize { get; set; }
        /**
         * Height in pixels for the horizontal slider.
         * Defaults to `horizontalScrollbarSize`.
         * **NOTE**: This option cannot be updated using `updateOptions()`
         */
        public int? HorizontalSliderSize { get; set; }
        /**
         * Scroll gutter clicks move by page vs jump to position.
         * Defaults to false.
         */
        public bool? ScrollByPage { get; set; }
        /**
         * When set, the horizontal scrollbar will not increase content height.
         * Defaults to false.
         */
        public bool? IgnoreHorizontalScrollbarInContentHeight { get; set; }
    }


    /**
     * Configuration options for unicode highlighting.
     */
    public class UnicodeHighlightOptions
    {
        /**
         * Controls whether all non-basic ASCII characters are highlighted. Only characters between U+0020 and U+007E, tab, line-feed and carriage-return are considered basic ASCII.
         */
        public bool? NonBasicASCII { get; set; }
        /**
         * Controls whether characters that just reserve space or have no width at all are highlighted.
         */
        public bool? InvisibleCharacters { get; set; }
        /**
         * Controls whether characters are highlighted that can be confused with basic ASCII characters, except those that are common in the current user locale.
         */
        public bool? AmbiguousCharacters { get; set; }
        /**
         * Controls whether characters in comments should also be subject to unicode highlighting.
         */
        public bool? IncludeComments { get; set; }
        /**
         * Controls whether characters in strings should also be subject to unicode highlighting.
         */
        public bool? IncludeStrings { get; set; }
        /**
         * Defines allowed characters that are not being highlighted.
         */
        /**
         * Unicode characters that are common in allowed locales are not being highlighted.
         */
    }

    public class InlineSuggestOptions
    {
        /**
         * Enable or disable the rendering of automatic inline completions.
        */
        public bool? Enabled { get; set; }
        /**
         * Configures the mode.
         * Use `prefix` to only show ghost text if the text to replace is a prefix of the suggestion text.
         * Use `subword` to only show ghost text if the replace text is a subword of the suggestion text.
         * Use `subwordSmart` to only show ghost text if the replace text is a subword of the suggestion text, but the subword must start after the cursor position.
         * Defaults to `prefix`.
        */
        public string Mode { get; set; }
        public string ShowToolbar { get; set; }
        public bool? SyntaxHighlightingEnabled { get; set; }
        public bool? SuppressSuggestions { get; set; }
        public int? MinShowDelay { get; set; }
        public bool? SuppressInSnippetMode { get; set; }
        /**
         * Does not clear active inline suggestions when the editor loses focus.
         */
        public bool? KeepOnBlur { get; set; }
        /**
         * Font family for inline suggestions.
         */
        public string FontFamily { get; set; }
    }


    public class BracketPairColorizationOptions
    {
        /**
         * Enable or disable bracket pair colorization.
        */
        public bool? Enabled { get; set; }
        /**
         * Use independent color pool per bracket type.
        */
        public bool? IndependentColorPoolPerBracketType { get; set; }
    }

    public class GuidesOptions
    {
        /**
         * Enable rendering of bracket pair guides.
         * Defaults to false.
        */
        public string BracketPairs { get; set; }
        /**
         * Enable rendering of vertical bracket pair guides.
         * Defaults to 'active'.
         */
        public string BracketPairsHorizontal { get; set; }
        /**
         * Enable highlighting of the active bracket pair.
         * Defaults to true.
        */
        public bool? HighlightActiveBracketPair { get; set; }
        /**
         * Enable rendering of indent guides.
         * Defaults to true.
         */
        public bool? Indentation { get; set; }
        /**
         * Enable highlighting of the active indent guide.
         * Defaults to true.
         */
        public object HighlightActiveIndentation { get; set; }
    }

    /**
     * Configuration options for editor suggest widget
     */
    public class SuggestOptions
    {
        /**
         * Overwrite word ends on accept. Default to false.
         */
        public string InsertMode { get; set; }
        /**
         * Enable graceful matching. Defaults to true.
         */
        public bool? FilterGraceful { get; set; }
        /**
         * Prevent quick suggestions when a snippet is active. Defaults to true.
         */
        public bool? SnippetsPreventQuickSuggestions { get; set; }
        /**
         * Favors words that appear close to the cursor.
         */
        public bool? LocalityBonus { get; set; }
        /**
         * Enable using global storage for remembering suggestions.
         */
        public bool? ShareSuggestSelections { get; set; }
        /**
         * Select suggestions when triggered via quick suggest or trigger characters
         */
        public string SelectionMode { get; set; }
        /**
         * Enable or disable icons in suggestions. Defaults to true.
         */
        public bool? ShowIcons { get; set; }
        /**
         * Enable or disable the suggest status bar.
         */
        public bool? ShowStatusBar { get; set; }
        /**
         * Enable or disable the rendering of the suggestion preview.
         */
        public bool? Preview { get; set; }
        /**
         * Configures the mode of the preview.
        */
        public string PreviewModel { get; set; }
        /**
         * Show details inline with the label. Defaults to true.
         */
        public bool? ShowInlineDetails { get; set; }
        /**
         * Show method-suggestions.
         */
        public bool? ShowMethods { get; set; }
        /**
         * Show function-suggestions.
         */
        public bool? ShowFunctions { get; set; }
        /**
         * Show constructor-suggestions.
         */
        public bool? ShowConstructors { get; set; }
        /**
         * Show deprecated-suggestions.
         */
        public bool? ShowDeprecated { get; set; }
        /**
         * Controls whether suggestions allow matches in the middle of the word instead of only at the beginning
         */
        public bool? MatchOnWordStartOnly { get; set; }
        /**
         * Show field-suggestions.
         */
        public bool? ShowFields { get; set; }
        /**
         * Show variable-suggestions.
         */
        public bool? ShowVariables { get; set; }
        /**
         * Show class-suggestions.
         */
        public bool? ShowClasses { get; set; }
        /**
         * Show struct-suggestions.
         */
        public bool? ShowStructs { get; set; }
        /**
         * Show interface-suggestions.
         */
        public bool? ShowInterfaces { get; set; }
        /**
         * Show module-suggestions.
         */
        public bool? ShowModules { get; set; }
        /**
         * Show property-suggestions.
         */
        public bool? ShowProperties { get; set; }
        /**
         * Show event-suggestions.
         */
        public bool? ShowEvents { get; set; }
        /**
         * Show operator-suggestions.
         */
        public bool? ShowOperators { get; set; }
        /**
         * Show unit-suggestions.
         */
        public bool? ShowUnits { get; set; }
        /**
         * Show value-suggestions.
         */
        public bool? ShowValues { get; set; }
        /**
         * Show constant-suggestions.
         */
        public bool? ShowConstants { get; set; }
        /**
         * Show enum-suggestions.
         */
        public bool? ShowEnums { get; set; }
        /**
         * Show enumMember-suggestions.
         */
        public bool? ShowEnumMembers { get; set; }
        /**
         * Show keyword-suggestions.
         */
        public bool? ShowKeywords { get; set; }
        /**
         * Show text-suggestions.
         */
        public bool? ShowWords { get; set; }
        /**
         * Show color-suggestions.
         */
        public bool? ShowColors { get; set; }
        /**
         * Show file-suggestions.
         */
        public bool? ShowFiles { get; set; }
        /**
         * Show reference-suggestions.
         */
        public bool? ShowReferences { get; set; }
        /**
         * Show folder-suggestions.
         */
        public bool? ShowFolders { get; set; }
        /**
         * Show typeParameter-suggestions.
         */
        public bool? ShowTypeParameters { get; set; }
        /**
         * Show issue-suggestions.
         */
        public bool? ShowIssues { get; set; }
        /**
         * Show user-suggestions.
         */
        public bool? ShowUsers { get; set; }
        /**
         * Show snippet-suggestions.
         */
        public bool? ShowSnippets { get; set; }
    }

    public class SmartSelectOptions
    {
        public bool? SelectLeadingAndTrailingWhitespace { get; set; }
        public bool? SelectSubwords { get; set; }
    }

    /**
     * Describes how to indent wrapped lines.
     */
    public enum WrappingIndent
    {
        /**
         * No indentation => wrapped lines begin at column 1.
         */
        None = 0,
        /**
         * Same => wrapped lines get the same indentation as the parent.
         */
        Same = 1,
        /**
         * Indent => wrapped lines get +1 indentation toward the parent.
         */
        Indent = 2,
        /**
         * DeepIndent => wrapped lines get +2 indentation toward the parent.
         */
        DeepIndent = 3
    }


    /**
     * Configuration options for editor drop into behavior
     */
    public class DropIntoEditorOptions
    {
        /**
         * Enable dropping into editor.
         * Defaults to true.
         */
        public bool? Enabled { get; set; }
        /**
         * Controls if a widget is shown after a drop.
         * Defaults to 'afterDrop'.
         */
        public string ShowDropSelector { get; set; }
    }

    /**
     * Configuration options for editor pasting as into behavior
     */
    public class PasteAsOptions
    {
        /**
         * Enable paste as functionality in editors.
         * Defaults to true.
         */
        public bool? Enabled { get; set; }
        /**
         * Controls if a widget is shown after a drop.
         * Defaults to 'afterPaste'.
         */
        public string ShowPasteSelector { get; set; }
    }

    public enum EditorOption
    {
        acceptSuggestionOnCommitCharacter = 0,
        acceptSuggestionOnEnter = 1,
        accessibilitySupport = 2,
        accessibilityPageSize = 3,
        allowOverflow = 4,
        allowVariableLineHeights = 5,
        allowVariableFonts = 6,
        allowVariableFontsInAccessibilityMode = 7,
        ariaLabel = 8,
        ariaRequired = 9,
        autoClosingBrackets = 10,
        autoClosingComments = 11,
        screenReaderAnnounceInlineSuggestion = 12,
        autoClosingDelete = 13,
        autoClosingOvertype = 14,
        autoClosingQuotes = 15,
        autoIndent = 16,
        autoIndentOnPaste = 17,
        autoIndentOnPasteWithinString = 18,
        automaticLayout = 19,
        autoSurround = 20,
        bracketPairColorization = 21,
        guides = 22,
        codeLens = 23,
        codeLensFontFamily = 24,
        codeLensFontSize = 25,
        colorDecorators = 26,
        colorDecoratorsLimit = 27,
        columnSelection = 28,
        comments = 29,
        contextmenu = 30,
        copyWithSyntaxHighlighting = 31,
        cursorBlinking = 32,
        cursorSmoothCaretAnimation = 33,
        cursorStyle = 34,
        cursorSurroundingLines = 35,
        cursorSurroundingLinesStyle = 36,
        cursorWidth = 37,
        cursorHeight = 38,
        disableLayerHinting = 39,
        disableMonospaceOptimizations = 40,
        domReadOnly = 41,
        dragAndDrop = 42,
        dropIntoEditor = 43,
        editContext = 44,
        emptySelectionClipboard = 45,
        experimentalGpuAcceleration = 46,
        experimentalWhitespaceRendering = 47,
        extraEditorClassName = 48,
        fastScrollSensitivity = 49,
        find = 50,
        fixedOverflowWidgets = 51,
        folding = 52,
        foldingStrategy = 53,
        foldingHighlight = 54,
        foldingImportsByDefault = 55,
        foldingMaximumRegions = 56,
        unfoldOnClickAfterEndOfLine = 57,
        fontFamily = 58,
        fontInfo = 59,
        fontLigatures = 60,
        fontSize = 61,
        fontWeight = 62,
        fontVariations = 63,
        formatOnPaste = 64,
        formatOnType = 65,
        glyphMargin = 66,
        gotoLocation = 67,
        hideCursorInOverviewRuler = 68,
        hover = 69,
        inDiffEditor = 70,
        inlineSuggest = 71,
        letterSpacing = 72,
        lightbulb = 73,
        lineDecorationsWidth = 74,
        lineHeight = 75,
        lineNumbers = 76,
        lineNumbersMinChars = 77,
        linkedEditing = 78,
        links = 79,
        matchBrackets = 80,
        minimap = 81,
        mouseStyle = 82,
        mouseWheelScrollSensitivity = 83,
        mouseWheelZoom = 84,
        multiCursorMergeOverlapping = 85,
        multiCursorModifier = 86,
        mouseMiddleClickAction = 87,
        multiCursorPaste = 88,
        multiCursorLimit = 89,
        occurrencesHighlight = 90,
        occurrencesHighlightDelay = 91,
        overtypeCursorStyle = 92,
        overtypeOnPaste = 93,
        overviewRulerBorder = 94,
        overviewRulerLanes = 95,
        padding = 96,
        pasteAs = 97,
        parameterHints = 98,
        peekWidgetDefaultFocus = 99,
        placeholder = 100,
        definitionLinkOpensInPeek = 101,
        quickSuggestions = 102,
        quickSuggestionsDelay = 103,
        readOnly = 104,
        readOnlyMessage = 105,
        renameOnType = 106,
        renderRichScreenReaderContent = 107,
        renderControlCharacters = 108,
        renderFinalNewline = 109,
        renderLineHighlight = 110,
        renderLineHighlightOnlyWhenFocus = 111,
        renderValidationDecorations = 112,
        renderWhitespace = 113,
        revealHorizontalRightPadding = 114,
        roundedSelection = 115,
        rulers = 116,
        scrollbar = 117,
        scrollBeyondLastColumn = 118,
        scrollBeyondLastLine = 119,
        scrollPredominantAxis = 120,
        selectionClipboard = 121,
        selectionHighlight = 122,
        selectionHighlightMaxLength = 123,
        selectionHighlightMultiline = 124,
        selectOnLineNumbers = 125,
        showFoldingControls = 126,
        showUnused = 127,
        snippetSuggestions = 128,
        smartSelect = 129,
        smoothScrolling = 130,
        stickyScroll = 131,
        stickyTabStops = 132,
        stopRenderingLineAfter = 133,
        suggest = 134,
        suggestFontSize = 135,
        suggestLineHeight = 136,
        suggestOnTriggerCharacters = 137,
        suggestSelection = 138,
        tabCompletion = 139,
        tabIndex = 140,
        trimWhitespaceOnDelete = 141,
        unicodeHighlighting = 142,
        unusualLineTerminators = 143,
        useShadowDOM = 144,
        useTabStops = 145,
        wordBreak = 146,
        wordSegmenterLocales = 147,
        wordSeparators = 148,
        wordWrap = 149,
        wordWrapBreakAfterCharacters = 150,
        wordWrapBreakBeforeCharacters = 151,
        wordWrapColumn = 152,
        wordWrapOverride1 = 153,
        wordWrapOverride2 = 154,
        wrappingIndent = 155,
        wrappingStrategy = 156,
        showDeprecated = 157,
        inertialScroll = 158,
        inlayHints = 159,
        wrapOnEscapedLineFeeds = 160,
        wordWrapIndicator = 161,
        effectiveCursorStyle = 162,
        editorClassName = 163,
        pixelRatio = 164,
        tabFocusMode = 165,
        layoutInfo = 166,
        wrappingInfo = 167,
        defaultColorDecorators = 168,
        colorDecoratorsActivatedOn = 169,
        inlineCompletionsAccessibilityVerbose = 170,
        effectiveEditContext = 171,
        scrollOnMiddleClick = 172,
        effectiveAllowVariableFonts = 173,
        doubleClickSelectsBlock = 174,
        fullwidthCharacterWidth = 175,
        effectiveFullwidthCharacterWidth = 176
    }


    public class EditorConstructionOptions : EditorOptions
    {
        /**
         * The initial editor dimension (to avoid measuring the container).
         */
        public Dimension Dimension { get; set; }
        /**
         * Place overflow widgets inside an external DOM node.
         * Defaults to an internal DOM node.
         */
    }

    /**
     * A view zone is a full horizontal rectangle that 'pushes' text down.
     * The editor reserves space for view zones when rendering.
     */

    /**
     * An accessor that allows for zones to be added or removed.
     */

    /**
     * A positioning preference for rendering content widgets.
     */
    public enum ContentWidgetPositionPreference
    {
        /**
         * Place the content widget exactly at a position
         */
        EXACT = 0,
        /**
         * Place the content widget above a position
         */
        ABOVE = 1,
        /**
         * Place the content widget below a position
         */
        BELOW = 2
    }

    /**
     * A position for rendering content widgets.
     */

    /**
     * A content widget renders inline with the text and can be easily placed 'near' an editor position.
     */

    /**
     * Coordinatees passed in {@link IContentWidget.afterRender}
     */

    /**
     * A positioning preference for rendering overlay widgets.
     */
    public enum OverlayWidgetPositionPreference
    {
        /**
         * Position the overlay widget in the top right corner
         */
        TOP_RIGHT_CORNER = 0,
        /**
         * Position the overlay widget in the bottom right corner
         */
        BOTTOM_RIGHT_CORNER = 1,
        /**
         * Position the overlay widget in the top center
         */
        TOP_CENTER = 2
    }

    /**
     * Represents editor-relative coordinates of an overlay widget.
     */

    /**
     * A position for rendering overlay widgets.
     */

    /**
     * An overlay widgets renders on top of the text.
     */

    /**
     * A glyph margin widget renders in the editor glyph margin.
     */

    /**
     * A position for rendering glyph margin widgets.
     */

    /**
     * Type of hit element with the mouse in the editor.
     */
    public enum MouseTargetType
    {
        /**
         * Mouse is on top of an unknown element.
         */
        UNKNOWN = 0,
        /**
         * Mouse is on top of the textarea used for input.
         */
        TEXTAREA = 1,
        /**
         * Mouse is on top of the glyph margin
         */
        GUTTER_GLYPH_MARGIN = 2,
        /**
         * Mouse is on top of the line numbers
         */
        GUTTER_LINE_NUMBERS = 3,
        /**
         * Mouse is on top of the line decorations
         */
        GUTTER_LINE_DECORATIONS = 4,
        /**
         * Mouse is on top of the whitespace left in the gutter by a view zone.
         */
        GUTTER_VIEW_ZONE = 5,
        /**
         * Mouse is on top of text in the content.
         */
        CONTENT_TEXT = 6,
        /**
         * Mouse is on top of empty space in the content (e.g. after line text or below last line)
         */
        CONTENT_EMPTY = 7,
        /**
         * Mouse is on top of a view zone in the content.
         */
        CONTENT_VIEW_ZONE = 8,
        /**
         * Mouse is on top of a content widget.
         */
        CONTENT_WIDGET = 9,
        /**
         * Mouse is on top of the decorations overview ruler.
         */
        OVERVIEW_RULER = 10,
        /**
         * Mouse is on top of a scrollbar.
         */
        SCROLLBAR = 11,
        /**
         * Mouse is on top of an overlay widget.
         */
        OVERLAY_WIDGET = 12,
        /**
         * Mouse is outside of the editor.
         */
        OUTSIDE_EDITOR = 13
    }

    public class BaseMouseTarget
    {
        /**
         * The target element
         */
        public JsonElement? Element { get; set; }
        /**
         * The 'approximate' editor position
         */
        public Position Position { get; set; }
        /**
         * Desired mouse column (e.g. when position.column gets clamped to text length -- clicking after text on a line).
         */
        public int MouseColumn { get; set; }
        /**
         * The 'approximate' editor range
         */
        public Range Range { get; set; }
        public MouseTargetType Type { get; set; }
        public object Detail { get; set; }
    }

    public class MouseTargetUnknown : BaseMouseTarget
    {
    }

    public class MouseTargetTextarea : BaseMouseTarget
    {
    }

    public class MouseTargetMarginData
    {
        public bool IsAfterLines { get; set; }
        public float GlyphMarginLeft { get; set; }
        public float GlyphMarginWidth { get; set; }
        public GlyphMarginLane GlyphMarginLane { get; set; }
        public float LineNumbersWidth { get; set; }
        public float OffsetX { get; set; }
    }

    public class MouseTargetMargin : BaseMouseTarget
    {
        public new MouseTargetMarginData Detail { get; set; }
    }

    public class MouseTargetViewZoneData
    {
        public string ViewZoneId { get; set; }
        public Position PositionBefore { get; set; }
        public Position PositionAfter { get; set; }
        public Position Position { get; set; }
        public int AfterLineNumber { get; set; }
    }

    public class MouseTargetViewZone : BaseMouseTarget
    {
        public new MouseTargetViewZoneData Detail { get; set; }
    }

    public class MouseTargetContentTextData
    {
        public string MightBeForeignElement { get; set; }
    }

    public class MouseTargetContentText : BaseMouseTarget
    {
        public new MouseTargetContentTextData Detail { get; set; }
    }

    public class MouseTargetContentEmptyData
    {
        public bool IsAfterLines { get; set; }
        public float? HorizontalDistanceToText { get; set; }
    }

    public class MouseTargetContentEmpty : BaseMouseTarget
    {
        public new MouseTargetContentEmptyData Detail { get; set; }
    }

    public class MouseTargetContentWidget : BaseMouseTarget
    {
        public new string Detail { get; set; }
    }

    public class MouseTargetOverlayWidget : BaseMouseTarget
    {
        public new string Detail { get; set; }
    }

    public class MouseTargetScrollbar : BaseMouseTarget
    {
    }

    public class MouseTargetOverviewRuler : BaseMouseTarget
    {
    }

    public class MouseTargetOutsideEditor : BaseMouseTarget
    {
    }

    /**
     * Target hit with the mouse in the editor.
     */

    /**
     * A mouse event originating from the editor.
     */
    public class EditorMouseEvent
    {
        public MouseEvent Event { get; set; }
        public BaseMouseTarget Target { get; set; }
    }

    public class PartialEditorMouseEvent
    {
        public MouseEvent Event { get; set; }
        public BaseMouseTarget Target { get; set; }
    }

    /**
     * A paste event originating from the editor.
     */
    public class PasteEvent
    {
        public Range Range { get; set; }
        public string LanguageId { get; set; }
    }

    public class DiffEditorConstructionOptions : DiffEditorOptions
    {
        /**
         * The initial editor dimension (to avoid measuring the container).
         */
        public Dimension Dimension { get; set; }
        /**
         * Place overflow widgets inside an external DOM node.
         * Defaults to an internal DOM node.
         */
        /**
         * Aria label for original editor.
         */
        public string OriginalAriaLabel { get; set; }
        /**
         * Aria label for modified editor.
         */
        public string ModifiedAriaLabel { get; set; }
    }

    /**
     * A rich code editor.
     */

    /**
     * A rich diff editor.
     */


}

namespace BlazorMonaco.Languages
{

    public class RelativePattern
    {
        /**
         * A base file path to which this pattern will be matched against relatively.
         */
        public string Base { get; set; }
        /**
         * A file glob pattern like `*.{ts,js}` that will be matched on file paths
         * relative to the base path.
         *
         * Example: Given a base of `/home/work/folder` and a file path of `/home/work/folder/index.js`,
         * the file glob pattern will match on `index.js`.
         */
        public string Pattern { get; set; }

        public RelativePattern()
        { }

        public RelativePattern(string pattern)
            => Pattern = pattern;

        public static implicit operator RelativePattern(string pattern) => new RelativePattern(pattern);
    }

    public class LanguageSelector : List<LanguageFilter>
    {
        public LanguageSelector()
        { }

        public LanguageSelector(string language)
            => Add(language);

        public LanguageSelector(List<string> languages)
            => AddRange(languages.Select(l => new LanguageFilter(l)));

        public LanguageSelector(LanguageFilter languageFilter)
            => Add(languageFilter);

        public LanguageSelector(List<LanguageFilter> languageFilters)
            => AddRange(languageFilters);

        public static implicit operator LanguageSelector(string language) => new LanguageSelector(language);
        public static implicit operator LanguageSelector(List<string> languages) => new LanguageSelector(languages);
        public static implicit operator LanguageSelector(LanguageFilter language) => new LanguageSelector(language);
    }

    public class LanguageFilter
    {
        public string Language { get; set; }
        public string Scheme { get; set; }
        public RelativePattern Pattern { get; set; }
        public string NotebookType { get; set; }
        /**
         * This provider is implemented in the UI thread.
         */
        public bool? HasAccessToAllModels { get; set; }
        public bool? Exclusive { get; set; }
        /**
         * This provider comes from a builtin extension.
         */
        public bool? IsBuiltin { get; set; }

        public LanguageFilter()
        { }

        public LanguageFilter(string language)
            => Language = language;

        public static implicit operator LanguageFilter(string language) => new LanguageFilter(language);
    }

    public partial class Global
    {


    }

    /**
     * A token.
     */

    /**
     * The result of a line tokenization.
     */

    /**
     * The result of a line tokenization.
     */

    /**
     * A factory for token providers.
     */

    /**
     * A "manual" provider of tokens.
     */

    /**
     * A "manual" provider of tokens, returning tokens in a binary form.
     */

    public partial class Global
    {


        /**
         * Register a hover provider (used by e.g. editor hover).
         */
        public static Task RegisterHoverProviderAsync(IJSRuntime jsRuntime, LanguageSelector language, HoverProvider.ProvideDelegate provideHover)
            => RegisterHoverProviderAsync(jsRuntime, language, new HoverProvider(provideHover));

        public static Task RegisterHoverProviderAsync(IJSRuntime jsRuntime, LanguageSelector language, HoverProvider hoverProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerHoverProvider", language, DotNetObjectReference.Create(hoverProvider));


        /**
         * Register a code action provider (used by e.g. quick fix).
         */
        [Obsolete("Please use the new RegisterCodeActionProvider method with async parameters instead.")]
        public static Task RegisterCodeActionProvider(IJSRuntime jsRuntime, LanguageSelector language, CodeActionProvider.ProvideCodeActionsDelegate provideCodeActions, CodeActionProvider.ResolveCodeActionDelegate resolveCodeAction = null, CodeActionProviderMetadata metadata = null)
            => RegisterCodeActionProvider(jsRuntime, language, new CodeActionProvider(provideCodeActions, resolveCodeAction), metadata);

        public static Task RegisterCodeActionProvider(IJSRuntime jsRuntime, LanguageSelector language, CodeActionProvider.ProvideDelegate provideCodeActions, CodeActionProvider.ResolveDelegate resolveCodeAction = null, CodeActionProviderMetadata metadata = null)
            => RegisterCodeActionProvider(jsRuntime, language, new CodeActionProvider(provideCodeActions, resolveCodeAction), metadata);

        public static Task RegisterCodeActionProvider(IJSRuntime jsRuntime, LanguageSelector language, CodeActionProvider codeActionProvider, CodeActionProviderMetadata metadata = null)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerCodeActionProvider", language, DotNetObjectReference.Create(codeActionProvider), metadata);

        /**
         * Register a formatter that can handle only entire models.
         */
        public static Task RegisterDocumentFormattingEditProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentFormattingEditProvider.ProvideDelegate provideDocumentFormattingEdits)
            => RegisterDocumentFormattingEditProvider(jsRuntime, language, new DocumentFormattingEditProvider(null, provideDocumentFormattingEdits));

        public static Task RegisterDocumentFormattingEditProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentFormattingEditProvider documentFormattingEditProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentFormattingEditProvider", language, documentFormattingEditProvider.DisplayName, DotNetObjectReference.Create(documentFormattingEditProvider));


        /**
         * Register a completion item provider (use by e.g. suggestions).
         */
        [Obsolete("Please use the new RegisterCompletionItemProvider method with async parameters instead.")]
        public static Task RegisterCompletionItemProvider(IJSRuntime jsRuntime, LanguageSelector language, CompletionItemProvider.ProvideCompletionItemsDelegate provideCompletionItems, CompletionItemProvider.ResolveCompletionItemDelegate resolveCompletionItem = null)
            => RegisterCompletionItemProvider(jsRuntime, language, new CompletionItemProvider(provideCompletionItems, resolveCompletionItem));

        public static Task RegisterCompletionItemProvider(IJSRuntime jsRuntime, LanguageSelector language, CompletionItemProvider.ProvideDelegate provideCompletionItems, CompletionItemProvider.ResolveDelegate resolveCompletionItem = null)
            => RegisterCompletionItemProvider(jsRuntime, language, new CompletionItemProvider(null, provideCompletionItems, resolveCompletionItem));

        public static Task RegisterCompletionItemProvider(IJSRuntime jsRuntime, LanguageSelector language, CompletionItemProvider completionItemProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerCompletionItemProvider", language, completionItemProvider.TriggerCharacters, DotNetObjectReference.Create(completionItemProvider));

        /**
         * Register a definition provider.
         */
        public static Task RegisterDefinitionProvider(IJSRuntime jsRuntime, LanguageSelector language, DefinitionProvider definitionProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDefinitionProvider", language, DotNetObjectReference.Create(definitionProvider));

        /**
         * Register a reference provider.
         */
        public static Task RegisterReferenceProvider(IJSRuntime jsRuntime, LanguageSelector language, ReferenceProvider referenceProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerReferenceProvider", language, DotNetObjectReference.Create(referenceProvider));

        /**
         * Register an implementation provider.
         */
        public static Task RegisterImplementationProvider(IJSRuntime jsRuntime, LanguageSelector language, ImplementationProvider implementationProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerImplementationProvider", language, DotNetObjectReference.Create(implementationProvider));

        /**
         * Register a type definition provider.
         */
        public static Task RegisterTypeDefinitionProvider(IJSRuntime jsRuntime, LanguageSelector language, TypeDefinitionProvider typeDefinitionProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerTypeDefinitionProvider", language, DotNetObjectReference.Create(typeDefinitionProvider));

        /**
         * Register a declaration provider.
         */
        public static Task RegisterDeclarationProvider(IJSRuntime jsRuntime, LanguageSelector language, DeclarationProvider declarationProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDeclarationProvider", language, DotNetObjectReference.Create(declarationProvider));

        /**
         * Register a document symbol provider.
         */
        public static Task RegisterDocumentSymbolProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentSymbolProvider documentSymbolProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentSymbolProvider", language, DotNetObjectReference.Create(documentSymbolProvider));

        /**
         * Register a folding range provider.
         */
        public static Task RegisterFoldingRangeProvider(IJSRuntime jsRuntime, LanguageSelector language, FoldingRangeProvider foldingRangeProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerFoldingRangeProvider", language, DotNetObjectReference.Create(foldingRangeProvider));

        /**
         * Register a document highlight provider.
         */
        public static Task RegisterDocumentHighlightProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentHighlightProvider documentHighlightProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentHighlightProvider", language, DotNetObjectReference.Create(documentHighlightProvider));

        /**
         * Register a document color provider.
         */
        public static Task RegisterColorProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentColorProvider documentColorProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerColorProvider", language, DotNetObjectReference.Create(documentColorProvider));

        /**
         * Register a selection range provider.
         */
        public static Task RegisterSelectionRangeProvider(IJSRuntime jsRuntime, LanguageSelector language, SelectionRangeProvider selectionRangeProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerSelectionRangeProvider", language, DotNetObjectReference.Create(selectionRangeProvider));

        /**
         * Register a linked editing range provider.
         */
        public static Task RegisterLinkedEditingRangeProvider(IJSRuntime jsRuntime, LanguageSelector language, LinkedEditingRangeProvider linkedEditingRangeProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerLinkedEditingRangeProvider", language, DotNetObjectReference.Create(linkedEditingRangeProvider));

        /**
         * Register a link provider.
         */
        public static Task RegisterLinkProvider(IJSRuntime jsRuntime, LanguageSelector language, LinkProvider linkProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerLinkProvider", language, DotNetObjectReference.Create(linkProvider));

        /**
         * Register a new symbol names provider.
         */
        public static Task RegisterNewSymbolNameProvider(IJSRuntime jsRuntime, LanguageSelector language, NewSymbolNamesProvider newSymbolNamesProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerNewSymbolNameProvider", language, DotNetObjectReference.Create(newSymbolNamesProvider));

        /**
         * Register a language via a language extension point.
         *
         * **Note**: The language id and the language extensions should match.
         * **Note**: To associate a Monaco editor language with this registered language you must also call SetModelLanguage with a model and the language id.
         *
         * @param language The language extension point.
         */
        public static Task RegisterLanguage(IJSRuntime jsRuntime, LanguageExtensionPoint language)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerLanguage", language);

        /**
         * Set a language configuration for a language.
         */
        public static Task SetLanguageConfiguration(IJSRuntime jsRuntime, string languageId, object configuration)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.setLanguageConfiguration", languageId, configuration);

        /**
         * Set a Monarch tokens provider for a language.
         */
        public static Task SetMonarchTokensProvider(IJSRuntime jsRuntime, string languageId, object languageDef)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.setMonarchTokensProvider", languageId, languageDef);

        /**
         * Set the Monarch color map.
         */
        public static Task SetColorMap(IJSRuntime jsRuntime, string[] colorMap)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.setColorMap", colorMap);

        /**
         * Get the encoded language id for a language.
         */
        public static Task<int> GetEncodedLanguageId(IJSRuntime jsRuntime, string languageId)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<int>("blazorMonaco.languages.getEncodedLanguageId", languageId);

        /**
         * Get the information of all the registered languages.
         */
        public static Task<LanguageExtensionPoint[]> GetLanguages(IJSRuntime jsRuntime)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<LanguageExtensionPoint[]>("blazorMonaco.languages.getLanguages");

        /**
         * Register a listener that is called when a language is loaded for the first time.
         */
        public static Task OnLanguage(IJSRuntime jsRuntime, string languageId)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.onLanguage", languageId);

        /**
         * Register a listener that is called when a language is encountered for the first time.
         */
        public static Task OnLanguageEncountered(IJSRuntime jsRuntime, string languageId)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.onLanguageEncountered", languageId);

        /**
         * Compute the score of a language selector against a resource.
         */
        public static Task<int> Score(IJSRuntime jsRuntime, LanguageSelector selector, string candidateUri, string candidateLanguage)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync<int>("blazorMonaco.languages.score", selector, candidateUri, candidateLanguage);

        /**
         * Set the tokens provider for a language (manual implementation).
         */
        public static Task SetTokensProvider(IJSRuntime jsRuntime, string languageId, TokensProvider tokensProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.setTokensProvider", languageId, DotNetObjectReference.Create(tokensProvider));

        /**
         * Set an encoded tokens provider for a language (binary, manual implementation).
         */
        public static Task SetTokensProvider(IJSRuntime jsRuntime, string languageId, EncodedTokensProvider tokensProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.setEncodedTokensProvider", languageId, DotNetObjectReference.Create(tokensProvider));

        /**
         * Register a signature help provider.
         */
        public static Task RegisterSignatureHelpProvider(IJSRuntime jsRuntime, LanguageSelector language, SignatureHelpProvider signatureHelpProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerSignatureHelpProvider", language, DotNetObjectReference.Create(signatureHelpProvider));

        /**
         * Register a code lens provider.
         */
        public static Task RegisterCodeLensProvider(IJSRuntime jsRuntime, LanguageSelector language, CodeLensProvider codeLensProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerCodeLensProvider", language, DotNetObjectReference.Create(codeLensProvider));

        /**
         * Register a rename provider.
         */
        public static Task RegisterRenameProvider(IJSRuntime jsRuntime, LanguageSelector language, RenameProvider renameProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerRenameProvider", language, DotNetObjectReference.Create(renameProvider));

        /**
         * Register a document semantic tokens provider.
         */
        public static Task RegisterDocumentSemanticTokensProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentSemanticTokensProvider documentSemanticTokensProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentSemanticTokensProvider", language, DotNetObjectReference.Create(documentSemanticTokensProvider), documentSemanticTokensProvider.Legend);

        /**
         * Register a document range semantic tokens provider.
         */
        public static Task RegisterDocumentRangeSemanticTokensProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentRangeSemanticTokensProvider documentRangeSemanticTokensProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentRangeSemanticTokensProvider", language, DotNetObjectReference.Create(documentRangeSemanticTokensProvider), documentRangeSemanticTokensProvider.Legend);

        /**
         * Register an inlay hints provider.
         */
        public static Task RegisterInlayHintsProvider(IJSRuntime jsRuntime, LanguageSelector language, InlayHintsProvider inlayHintsProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerInlayHintsProvider", language, DotNetObjectReference.Create(inlayHintsProvider));

        /**
         * Register an inline completions provider.
         */
        public static Task RegisterInlineCompletionsProvider(IJSRuntime jsRuntime, LanguageSelector language, InlineCompletionsProvider inlineCompletionsProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerInlineCompletionsProvider", language, DotNetObjectReference.Create(inlineCompletionsProvider));

        /**
         * Register a document range formatting edit provider.
         */
        public static Task RegisterDocumentRangeFormattingEditProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentRangeFormattingEditProvider documentRangeFormattingEditProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentRangeFormattingEditProvider", language, DotNetObjectReference.Create(documentRangeFormattingEditProvider));

        /**
         * Register an on-type formatting edit provider.
         * @param triggerCharacters The characters that trigger formatting.
         */
        public static Task RegisterOnTypeFormattingEditProvider(IJSRuntime jsRuntime, LanguageSelector language, OnTypeFormattingEditProvider onTypeFormattingEditProvider, string[] triggerCharacters)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerOnTypeFormattingEditProvider", language, DotNetObjectReference.Create(onTypeFormattingEditProvider), triggerCharacters);


    }

    /**
     * Contains additional diagnostic information about the context in which
     * a [code action](#CodeActionProvider.provideCodeActions) is run.
     */
    public class CodeActionContext
    {
        /**
         * An array of diagnostics.
         */
        public List<MarkerData> Markers { get; set; }
        /**
         * Requested kind of actions to return.
         */
        public string Only { get; set; }
        /**
         * The reason why code actions were requested.
         */
        public CodeActionTriggerType Trigger { get; set; }
    }

    /**
     * The code action interface defines the contract between extensions and
     * the [light bulb](https://code.visualstudio.com/docs/editor/editingevolved#_code-action) feature.
     */
    public class CodeActionProvider
    {
        /**
         * Provide commands for the given document and range.
         */
        [Obsolete("Please use the new async ProvideDelegate instead.")]
        public delegate CodeActionList ProvideCodeActionsDelegate(string modelUri, Range range, CodeActionContext context);
        [Obsolete("Please use the new async ProvideMethod instead.")]
        public ProvideCodeActionsDelegate ProvideCodeActionsFunc { get; set; }
        public delegate Task<CodeActionList> ProvideDelegate(string modelUri, Range range, CodeActionContext context);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideCodeActions))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideCodeActions(string modelUri, Range range, CodeActionContext context)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var codeActions = (await ProvideMethod?.Invoke(modelUri, range, context))
                ?? ProvideCodeActionsFunc?.Invoke(modelUri, range, context);
#pragma warning restore CS0618 // Type or member is obsolete
            return JsonElementExt.FromObject(codeActions);
        }

        /**
         * Given a code action fill in the edit. Will only invoked when missing.
         */
        [Obsolete("Please use the new async ResolveDelegate instead.")]
        public delegate CodeAction ResolveCodeActionDelegate(CodeAction codeAction);
        [Obsolete("Please use the new async ResolveMethod instead.")]
        public ResolveCodeActionDelegate ResolveCodeActionFunc { get; set; }
        public delegate Task<CodeAction> ResolveDelegate(CodeAction codeAction);
        public ResolveDelegate ResolveMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ResolveCodeAction))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ResolveCodeAction(CodeAction codeAction)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var resolvedCodeAction = (await ResolveMethod?.Invoke(codeAction))
                ?? ResolveCodeActionFunc?.Invoke(codeAction);
#pragma warning restore CS0618 // Type or member is obsolete
            return JsonElementExt.FromObject(resolvedCodeAction);
        }

        [Obsolete("Please use the new constructor with async parameters instead.")]
        public CodeActionProvider(ProvideCodeActionsDelegate provideCodeActions, ResolveCodeActionDelegate resolveCodeAction = null)
        {
            ProvideCodeActionsFunc = provideCodeActions;
            ResolveCodeActionFunc = resolveCodeAction;
        }

        public CodeActionProvider(ProvideDelegate provideCodeActions, ResolveDelegate resolveCodeAction = null)
        {
            ProvideMethod = provideCodeActions;
            ResolveMethod = resolveCodeAction;
        }
    }

    /**
     * Metadata about the type of code actions that a {@link CodeActionProvider} provides.
     */
    public class CodeActionProviderMetadata
    {
        /**
         * List of code action kinds that a {@link CodeActionProvider} may return.
         *
         * This list is used to determine if a given `CodeActionProvider` should be invoked or not.
         * To avoid unnecessary computation, every `CodeActionProvider` should list use `providedCodeActionKinds`. The
         * list of kinds may either be generic, such as `["quickfix", "refactor", "source"]`, or list out every kind provided,
         * such as `["quickfix.removeLine", "source.fixAll" ...]`.
         */
        public List<string> ProvidedCodeActionKinds { get; set; }
        public List<CodeActionProviderMetadataDocumentation> Documentation { get; set; }
    }

    public class CodeActionProviderMetadataDocumentation
    {
        public string Kind { get; set; }
        public Command Command { get; set; }
    }

    /**
     * Configuration for line comments.
     */

    /**
     * Describes how comments for a language work.
     */

    /**
     * The language configuration interface defines the contract between extensions and
     * various editor features, like automatic bracket insertion, automatic indentation etc.
     */

    /**
     * Describes indentation rules for a language.
     */

    /**
     * Describes language specific folding markers such as '#region' and '#endregion'.
     * The start and end regexes will be tested against the contents of all lines and must be designed efficiently:
     * - the regex should start with '^'
     */

    /**
     * Describes folding rules for a language.
     */

    /**
     * Describes a rule to be evaluated when pressing Enter.
     */

    /**
     * Definition of documentation comments (e.g. Javadoc/JSdoc)
     */

    /**
     * A tuple of two characters, like a pair of
     * opening and closing brackets.
     */


    /**
     * Describes what to do with the indentation when pressing Enter.
     */
    public enum IndentAction
    {
        /**
         * Insert new line and copy the previous line's indentation.
         */
        None = 0,
        /**
         * Insert new line and indent once (relative to the previous line's indentation).
         */
        Indent = 1,
        /**
         * Insert two new lines:
         *  - the first one indented which will hold the cursor
         *  - the second one at the same indentation level
         */
        IndentOutdent = 2,
        /**
         * Insert new line and outdent once (relative to the previous line's indentation).
         */
        Outdent = 3
    }

    /**
     * Describes what to do when pressing Enter.
     */


    /**
     * The state of the tokenizer between two lines.
     * It is useful to store flags such as in multiline comment, etc.
     * The model will clone the previous line's state and pass it in to tokenize the next line.
     */

    /**
     * A provider result represents the values a provider, like the {@link HoverProvider},
     * may return. For once this is the actual result type `T`, like `Hover`, or a thenable that resolves
     * to that type `T`. In addition, `null` and `undefined` can be returned - either directly or from a
     * thenable.
     */

    /**
     * A hover represents additional information for a symbol or word. Hovers are
     * rendered in a tooltip-like widget.
     */
    public class Hover
    {
        /**
         * The contents of this hover.
         */
        public MarkdownString[] Contents { get; set; }
        /**
         * The range to which this hover applies. When missing, the
         * editor will use the range at the current position or the
         * current position itself.
         */
        public Range Range { get; set; }
        /**
         * Can increase the verbosity of the hover
         */
        public bool? CanIncreaseVerbosity { get; set; }
        /**
         * Can decrease the verbosity of the hover
         */
        public bool? CanDecreaseVerbosity { get; set; }
    }

    /**
     * The hover provider interface defines the contract between extensions and
     * the [hover](https://code.visualstudio.com/docs/editor/intellisense)-feature.
     */
    /**
     * The context for reference provider requests.
     */
    public class ReferenceContext
    {
        /**
         * Reference provider is requested at the given position. The result should include the
         * declaration of the symbol at the position if `includeDeclaration` is set to true.
         */
        public bool IncludeDeclaration { get; set; }
    }

    /**
     * Color information sent to `provideColorPresentations`.
     */
    public class ColorInformation
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
        public double Alpha { get; set; }
    }

    /**
     * Legend describing the semantic tokens a `SemanticTokensProvider` supports.
     */
    public class SemanticTokensLegend
    {
        public string[] TokenTypes { get; set; }
        public string[] TokenModifiers { get; set; }
    }

    /**
     * A provider that returns definitions for a given position.
     */
    public class DefinitionProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDefinitions))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDefinitions(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public DefinitionProvider(ProvideDelegate provideDefinitions) => ProvideMethod = provideDefinitions;
    }

    /**
     * A provider that returns references for a given position.
     */
    public class ReferenceProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position, ReferenceContext context);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideReferences))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideReferences(string modelUri, Position position, ReferenceContext context)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position, context));

        public ReferenceProvider(ProvideDelegate provideReferences) => ProvideMethod = provideReferences;
    }

    /**
     * A provider that returns implementations for a given position.
     */
    public class ImplementationProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideImplementations))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideImplementations(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public ImplementationProvider(ProvideDelegate provideImplementations) => ProvideMethod = provideImplementations;
    }

    /**
     * A provider that returns type definitions for a given position.
     */
    public class TypeDefinitionProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideTypeDefinitions))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideTypeDefinitions(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public TypeDefinitionProvider(ProvideDelegate provideTypeDefinitions) => ProvideMethod = provideTypeDefinitions;
    }

    /**
     * A provider that returns declarations for a given position.
     */
    public class DeclarationProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDeclarations))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDeclarations(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public DeclarationProvider(ProvideDelegate provideDeclarations) => ProvideMethod = provideDeclarations;
    }

    /**
     * A provider that returns document symbols for a model.
     */
    public class DocumentSymbolProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentSymbols))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentSymbols(string modelUri)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri));

        public DocumentSymbolProvider(ProvideDelegate provideDocumentSymbols) => ProvideMethod = provideDocumentSymbols;
    }

    /**
     * A provider that returns folding ranges for a model.
     */
    public class FoldingRangeProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideFoldingRanges))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideFoldingRanges(string modelUri)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri));

        public FoldingRangeProvider(ProvideDelegate provideFoldingRanges) => ProvideMethod = provideFoldingRanges;
    }

    /**
     * A provider that returns document highlights for a given position.
     */
    public class DocumentHighlightProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentHighlights))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentHighlights(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public DocumentHighlightProvider(ProvideDelegate provideDocumentHighlights) => ProvideMethod = provideDocumentHighlights;
    }

    /**
     * A provider that registers colors and color presentations for a model.
     */
    public class DocumentColorProvider
    {
        public delegate Task<object> ProvideColorsDelegate(string modelUri);
        public ProvideColorsDelegate ProvideColorsMethod { get; set; }
        public delegate Task<object> ProvideColorPresentationsDelegate(string modelUri, ColorInformation colorInfo);
        public ProvideColorPresentationsDelegate ProvideColorPresentationsMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideColors))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideColors(string modelUri)
            => JsonElementExt.FromObject(await ProvideColorsMethod?.Invoke(modelUri));

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideColorPresentations))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideColorPresentations(string modelUri, ColorInformation colorInfo)
            => JsonElementExt.FromObject(await ProvideColorPresentationsMethod?.Invoke(modelUri, colorInfo));

        public DocumentColorProvider(ProvideColorsDelegate provideColors, ProvideColorPresentationsDelegate provideColorPresentations = null)
        {
            ProvideColorsMethod = provideColors;
            ProvideColorPresentationsMethod = provideColorPresentations;
        }
    }

    /**
     * A provider that returns selection ranges for a set of positions.
     */
    public class SelectionRangeProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position[] positions);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideSelectionRanges))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideSelectionRanges(string modelUri, Position[] positions)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, positions));

        public SelectionRangeProvider(ProvideDelegate provideSelectionRanges) => ProvideMethod = provideSelectionRanges;
    }

    /**
     * A provider that returns signature help for a given position.
     */
    public class SignatureHelpProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideSignatureHelp))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideSignatureHelp(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public SignatureHelpProvider(ProvideDelegate provideSignatureHelp) => ProvideMethod = provideSignatureHelp;
    }

    /**
     * A provider that returns code lenses for a model.
     */
    public class CodeLensProvider
    {
        public delegate Task<object> ProvideCodeLensesDelegate(string modelUri);
        public ProvideCodeLensesDelegate ProvideCodeLensesMethod { get; set; }
        public delegate Task<object> ResolveCodeLensDelegate(string modelUri, object codeLens);
        public ResolveCodeLensDelegate ResolveCodeLensMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideCodeLenses))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideCodeLenses(string modelUri)
            => JsonElementExt.FromObject(await ProvideCodeLensesMethod?.Invoke(modelUri));

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ResolveCodeLens))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ResolveCodeLens(string modelUri, object codeLens)
            => JsonElementExt.FromObject(await ResolveCodeLensMethod?.Invoke(modelUri, codeLens));

        public CodeLensProvider(ProvideCodeLensesDelegate provideCodeLenses, ResolveCodeLensDelegate resolveCodeLens = null)
        {
            ProvideCodeLensesMethod = provideCodeLenses;
            ResolveCodeLensMethod = resolveCodeLens;
        }
    }

    /**
     * A provider that returns rename edits for a given position.
     */
    public class RenameProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position, string newName);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideRenameEdits))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideRenameEdits(string modelUri, Position position, string newName)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position, newName));

        public RenameProvider(ProvideDelegate provideRenameEdits) => ProvideMethod = provideRenameEdits;
    }

    /**
     * A provider that returns document semantic tokens.
     */
    public class DocumentSemanticTokensProvider
    {
        public SemanticTokensLegend Legend { get; }

        public delegate Task<object> ProvideDelegate(string modelUri, string lastResultId);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentSemanticTokens))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentSemanticTokens(string modelUri, string lastResultId)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, lastResultId));

        public DocumentSemanticTokensProvider(SemanticTokensLegend legend, ProvideDelegate provideDocumentSemanticTokens)
        {
            Legend = legend;
            ProvideMethod = provideDocumentSemanticTokens;
        }
    }

    /**
     * A provider that returns document range semantic tokens.
     */
    public class DocumentRangeSemanticTokensProvider
    {
        public SemanticTokensLegend Legend { get; }

        public delegate Task<object> ProvideDelegate(string modelUri, Range range);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentRangeSemanticTokens))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentRangeSemanticTokens(string modelUri, Range range)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, range));

        public DocumentRangeSemanticTokensProvider(SemanticTokensLegend legend, ProvideDelegate provideDocumentRangeSemanticTokens)
        {
            Legend = legend;
            ProvideMethod = provideDocumentRangeSemanticTokens;
        }
    }

    /**
     * A provider that returns inlay hints for a model.
     */
    public class InlayHintsProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Range range);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideInlayHints))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideInlayHints(string modelUri, Range range)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, range));

        public InlayHintsProvider(ProvideDelegate provideInlayHints) => ProvideMethod = provideInlayHints;
    }

    /**
     * A provider that returns inline completions for a given position.
     */
    public class InlineCompletionsProvider
    {
        public delegate Task<object> ProvideInlineCompletionsDelegate(string modelUri, Position position);
        public ProvideInlineCompletionsDelegate ProvideInlineCompletionsMethod { get; set; }
        public delegate Task<object> ResolveInlineCompletionDelegate(string modelUri, object inlineCompletion);
        public ResolveInlineCompletionDelegate ResolveInlineCompletionMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideInlineCompletions))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideInlineCompletions(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideInlineCompletionsMethod?.Invoke(modelUri, position));

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ResolveInlineCompletion))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ResolveInlineCompletion(string modelUri, object inlineCompletion)
            => JsonElementExt.FromObject(await ResolveInlineCompletionMethod?.Invoke(modelUri, inlineCompletion));

        public InlineCompletionsProvider(ProvideInlineCompletionsDelegate provideInlineCompletions, ResolveInlineCompletionDelegate resolveInlineCompletion = null)
        {
            ProvideInlineCompletionsMethod = provideInlineCompletions;
            ResolveInlineCompletionMethod = resolveInlineCompletion;
        }
    }

    /**
     * A provider that returns formatting edits for a range of a document.
     */
    public class DocumentRangeFormattingEditProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Range range, FormattingOptions options);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentRangeFormattingEdits))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentRangeFormattingEdits(string modelUri, Range range, FormattingOptions options)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, range, options));

        public DocumentRangeFormattingEditProvider(ProvideDelegate provideDocumentRangeFormattingEdits) => ProvideMethod = provideDocumentRangeFormattingEdits;
    }

    /**
     * A provider that returns formatting edits as the user types.
     */
    public class OnTypeFormattingEditProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position, string character, FormattingOptions options);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideOnTypeFormattingEdits))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideOnTypeFormattingEdits(string modelUri, Position position, string character, FormattingOptions options)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position, character, options));

        public OnTypeFormattingEditProvider(ProvideDelegate provideOnTypeFormattingEdits) => ProvideMethod = provideOnTypeFormattingEdits;
    }


    /**
     * A language extension point that contributes a language.
     */
    public class LanguageExtensionPoint
    {
        /**
         * The language's id.
         */
        public string Id { get; set; }
        /**
         * The language's extensions (e.g. `cs`).
         */
        public List<string> Extensions { get; set; }
        /**
         * The language's filenames (e.g. `mono.ts`).
         */
        public List<string> Filenames { get; set; }
        /**
         * The language's filename patterns (e.g. "*.cs", "templates/*.html").
         */
        public List<string> FilenamePatterns { get; set; }
        /**
         * The language's first line (e.g. `#!.*\bpython\b`).
         */
        public string FirstLine { get; set; }
        /**
         * The language's aliases (e.g. `["c#","csharp"]`).
         */
        public List<string> Aliases { get; set; }
        /**
         * The language's mime types (e.g. `["text/x-csharp"]`).
         */
        public List<string> Mimetypes { get; set; }
        /**
         * The language's configuration, specified as a `Uri` to a JSON file.
         */
        public Uri Configuration { get; set; }
    }

    /**
     * A linked editing range provider supplies ranges that can be edited together.
     */
    public class LinkedEditingRangeProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Position position);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideLinkedEditingRanges))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideLinkedEditingRanges(string modelUri, Position position)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, position));

        public LinkedEditingRangeProvider(ProvideDelegate provideLinkedEditingRanges)
        {
            ProvideMethod = provideLinkedEditingRanges;
        }
    }

    /**
     * A link provider supplies links to be found in a text model.
     */
    public class LinkProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri);
        public ProvideDelegate ProvideMethod { get; set; }

        public delegate Task<object> ResolveDelegate(object link);
        public ResolveDelegate ResolveMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideLinks))]
        [DynamicDependency(nameof(ResolveLink))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideLinks(string modelUri)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri));

        [JSInvokable]
        public async Task<JsonElement?> ResolveLink(object link)
            => JsonElementExt.FromObject(await ResolveMethod?.Invoke(link));

        public LinkProvider(ProvideDelegate provideLinks, ResolveDelegate resolveLink = null)
        {
            ProvideMethod = provideLinks;
            ResolveMethod = resolveLink;
        }
    }

    /**
     * A new symbol names provider suggests names when creating new symbols.
     */
    public class NewSymbolNamesProvider
    {
        public delegate Task<object> ProvideDelegate(string modelUri, Range range, int triggerKind);
        public ProvideDelegate ProvideMethod { get; set; }

        public delegate Task<object> ResolveDelegate(object symbol);
        public ResolveDelegate ResolveMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideNewSymbolNames))]
        [DynamicDependency(nameof(ResolveNewSymbolName))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideNewSymbolNames(string modelUri, Range range, int triggerKind)
            => JsonElementExt.FromObject(await ProvideMethod?.Invoke(modelUri, range, triggerKind));

        [JSInvokable]
        public async Task<JsonElement?> ResolveNewSymbolName(object symbol)
            => JsonElementExt.FromObject(await ResolveMethod?.Invoke(symbol));

        public NewSymbolNamesProvider(ProvideDelegate provideNewSymbolNames, ResolveDelegate resolveNewSymbolName = null)
        {
            ProvideMethod = provideNewSymbolNames;
            ResolveMethod = resolveNewSymbolName;
        }
    }

    /**
     * A "manual" provider of tokens for a language.
     */
    public class TokensProvider
    {
        public delegate Task<object> GetInitialStateDelegate();
        public GetInitialStateDelegate GetInitialStateMethod { get; set; }

        public delegate Task<object> TokenizeDelegate(string line, object state);
        public TokenizeDelegate TokenizeMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(GetInitialState))]
        [DynamicDependency(nameof(Tokenize))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> GetInitialState()
            => JsonElementExt.FromObject(await GetInitialStateMethod?.Invoke());

        [JSInvokable]
        public async Task<JsonElement?> Tokenize(string line, object state)
            => JsonElementExt.FromObject(await TokenizeMethod?.Invoke(line, state));

        public TokensProvider(GetInitialStateDelegate getInitialState, TokenizeDelegate tokenize)
        {
            GetInitialStateMethod = getInitialState;
            TokenizeMethod = tokenize;
        }
    }

    /**
     * A "manual" provider of tokens, returning tokens in a binary, encoded form.
     */
    public class EncodedTokensProvider
    {
        public delegate Task<object> GetInitialStateDelegate();
        public GetInitialStateDelegate GetInitialStateMethod { get; set; }

        public delegate Task<object> TokenizeEncodedDelegate(string line, object state);
        public TokenizeEncodedDelegate TokenizeEncodedMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(GetInitialState))]
        [DynamicDependency(nameof(TokenizeEncoded))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> GetInitialState()
            => JsonElementExt.FromObject(await GetInitialStateMethod?.Invoke());

        [JSInvokable]
        public async Task<JsonElement?> TokenizeEncoded(string line, object state)
            => JsonElementExt.FromObject(await TokenizeEncodedMethod?.Invoke(line, state));

        public EncodedTokensProvider(GetInitialStateDelegate getInitialState, TokenizeEncodedDelegate tokenizeEncoded)
        {
            GetInitialStateMethod = getInitialState;
            TokenizeEncodedMethod = tokenizeEncoded;
        }
    }

    public class HoverProvider
    {
        /**
         * Provide a hover for the given position, context and document. Multiple hovers at the same
         * position will be merged by the editor. A hover can have a range which defaults
         * to the word range at the position when omitted.
         */
        public delegate Task<Hover> ProvideDelegate(string modelUri, Position position, HoverContext context);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideHover))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideHover(string modelUri, Position position, HoverContext context)
        {
            var hover = await ProvideMethod?.Invoke(modelUri, position, context);
            return JsonElementExt.FromObject(hover);
        }

        public HoverProvider(ProvideDelegate provideHover)
        {
            ProvideMethod = provideHover;
        }
    }

    public class HoverContext
    {
        /**
         * Hover verbosity request
         */
        public HoverVerbosityRequest VerbosityRequest { get; set; }
    }

    public class HoverVerbosityRequest
    {
        /**
         * The delta by which to increase/decrease the hover verbosity level
         */
        public float VerbosityDelta { get; set; }

        /**
         * The previous hover for the same position
         */
        public Hover PreviousHover { get; set; }
    }

    public enum HoverVerbosityAction
    {
        /**
         * Increase the verbosity of the hover
         */
        Increase = 0,
        /**
         * Decrease the verbosity of the hover
         */
        Decrease = 1
    }

    public enum CompletionItemKind
    {
        Method = 0,
        Function = 1,
        Constructor = 2,
        Field = 3,
        Variable = 4,
        Class = 5,
        Struct = 6,
        Interface = 7,
        Module = 8,
        Property = 9,
        Event = 10,
        Operator = 11,
        Unit = 12,
        Value = 13,
        Constant = 14,
        Enum = 15,
        EnumMember = 16,
        Keyword = 17,
        Text = 18,
        Color = 19,
        File = 20,
        Reference = 21,
        Customcolor = 22,
        Folder = 23,
        TypeParameter = 24,
        User = 25,
        Issue = 26,
        Tool = 27,
        Snippet = 28
    }

    public class CompletionItemLabel
    {
        public string Label { get; set; }
        public string Detail { get; set; }
        public string Description { get; set; }
    }

    public enum CompletionItemTag
    {
        Deprecated = 1
    }

    public enum CompletionItemInsertTextRule
    {
        None = 0,
        /**
         * Adjust whitespace/indentation of multiline insert texts to
         * match the current line indentation.
         */
        KeepWhitespace = 1,
        /**
         * `insertText` is a snippet.
         */
        InsertAsSnippet = 4
    }

    public class CompletionItemRanges
    {
        public Range Insert { get; set; }
        public Range Replace { get; set; }
    }

    /**
     * A completion item represents a text snippet that is
     * proposed to complete text that is being typed.
     */
    public class CompletionItem
    {
        /**
         * The label of this completion item. By default
         * this is also the text that is inserted when selecting
         * this completion.
         */
        public JsonElement? Label { get; set; }
        [JsonIgnore]
        public string LabelAsString
        {
            get => Label?.AsString();
            set => Label = JsonElementExt.FromObject(value);
        }
        [JsonIgnore]
        public CompletionItemLabel LabelAsObject
        {
            get => Label?.AsObject<CompletionItemLabel>();
            set => Label = JsonElementExt.FromObject(value);
        }
        /**
         * The kind of this completion item. Based on the kind
         * an icon is chosen by the editor.
         */
        public CompletionItemKind Kind { get; set; }
        /**
         * A modifier to the `kind` which affect how the item
         * is rendered, e.g. Deprecated is rendered with a strikeout
         */
        public List<CompletionItemTag> Tags { get; set; }
        /**
         * A human-readable string with additional information
         * about this item, like type or symbol information.
         */
        public string Detail { get; set; }
        /**
         * A human-readable string that represents a doc-comment.
         */
        public JsonElement? Documentation { get; set; }
        [JsonIgnore]
        public string DocumentationAsString
        {
            get => Documentation?.AsString();
            set => Documentation = JsonElementExt.FromObject(value);
        }
        [JsonIgnore]
        public MarkdownString DocumentationAsObject
        {
            get => Documentation?.AsObject<MarkdownString>();
            set => Documentation = JsonElementExt.FromObject(value);
        }
        /**
         * A string that should be used when comparing this item
         * with other items. When `falsy` the {@link CompletionItem.label label}
         * is used.
         */
        public string SortText { get; set; }
        /**
         * A string that should be used when filtering a set of
         * completion items. When `falsy` the {@link CompletionItem.label label}
         * is used.
         */
        public string FilterText { get; set; }
        /**
         * Select this item when showing. *Note* that only one completion item can be selected and
         * that the editor decides which item that is. The rule is that the *first* item of those
         * that match best is selected.
         */
        public bool? Preselect { get; set; }
        /**
         * A string or snippet that should be inserted in a document when selecting
         * this completion.
         */
        public string InsertText { get; set; }
        /**
         * Additional rules (as bitmask) that should be applied when inserting
         * this completion.
         */
        public CompletionItemInsertTextRule? InsertTextRules { get; set; }
        /**
         * A range of text that should be replaced by this completion item.
         *
         * *Note:* The range must be a {@link Range.isSingleLine single line} and it must
         * {@link Range.contains contain} the position at which completion has been {@link CompletionItemProvider.provideCompletionItems requested}.
         */
        public JsonElement? Range { get; set; }
        [JsonIgnore]
        public Range RangeAsObject
        {
            get => Range?.AsObject<Range>();
            set => Range = JsonElementExt.FromObject(value);
        }
        [JsonIgnore]
        public CompletionItemRanges RangeAsList
        {
            get => Range?.AsObject<CompletionItemRanges>();
            set => Range = JsonElementExt.FromObject(value);
        }
        /**
         * An optional set of characters that when pressed while this completion is active will accept it first and
         * then type that character. *Note* that all commit characters should have `length=1` and that superfluous
         * characters will be ignored.
         */
        public List<string> CommitCharacters { get; set; }
        /**
         * An optional array of additional text edits that are applied when
         * selecting this completion. Edits must not overlap with the main edit
         * nor with themselves.
         */
        public List<SingleEditOperation> AdditionalTextEdits { get; set; }
        /**
         * A command that should be run upon acceptance of this item.
         */
        public Command Command { get; set; }
        /**
         * A command that should be run upon acceptance of this item.
         */
        public Command Action { get; set; }
    }

    public class CompletionList : IDisposable
    {
        public List<CompletionItem> Suggestions { get; set; }
        public bool? Incomplete { get; set; }
        public void Dispose() { }
    }

    /**
     * Info provided on partial acceptance.
     */
    public class PartialAcceptInfo
    {
        public PartialAcceptTriggerKind Kind { get; set; }
        public int AcceptedLength { get; set; }
    }

    /**
     * How a partial acceptance was triggered.
     */
    public enum PartialAcceptTriggerKind
    {
        Word = 0,
        Line = 1,
        Suggest = 2
    }

    /**
     * How a suggest provider was triggered.
     */
    public enum CompletionTriggerKind
    {
        Invoke = 0,
        TriggerCharacter = 1,
        TriggerForIncompleteCompletions = 2
    }

    /**
     * Contains additional information about the context in which
     * {@link CompletionItemProvider.provideCompletionItems completion provider} is triggered.
     */
    public class CompletionContext
    {
        /**
         * How the completion was triggered.
         */
        public CompletionTriggerKind? TriggerKind { get; set; }
        /**
         * Character that triggered the completion item provider.
         *
         * `undefined` if provider was not triggered by a character.
         */
        public string TriggerCharacter { get; set; }
    }

    /**
     * The completion item provider interface defines the contract between extensions and
     * the [IntelliSense](https://code.visualstudio.com/docs/editor/intellisense).
     *
     * When computing *complete* completion items is expensive, providers can optionally implement
     * the `resolveCompletionItem`-function. In that case it is enough to return completion
     * items with a {@link CompletionItem.label label} from the
     * {@link CompletionItemProvider.provideCompletionItems provideCompletionItems}-function. Subsequently,
     * when a completion item is shown in the UI and gains focus this provider is asked to resolve
     * the item, like adding {@link CompletionItem.documentation doc-comment} or {@link CompletionItem.detail details}.
     */
    public class CompletionItemProvider
    {
        public List<string> TriggerCharacters { get; set; }

        /**
         * Provide completion items for the given position and document.
         */
        [Obsolete("Please use the new async ProvideDelegate instead.")]
        public delegate CompletionList ProvideCompletionItemsDelegate(string modelUri, Position position, CompletionContext context);
        [Obsolete("Please use the new async ProvideMethod instead.")]
        public ProvideCompletionItemsDelegate ProvideCompletionItemsFunc { get; set; }
        public delegate Task<CompletionList> ProvideDelegate(string modelUri, Position position, CompletionContext context);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideCompletionItems))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideCompletionItems(string modelUri, Position position, CompletionContext context)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var completions = (await ProvideMethod?.Invoke(modelUri, position, context))
                ?? ProvideCompletionItemsFunc?.Invoke(modelUri, position, context);
#pragma warning restore CS0618 // Type or member is obsolete
            return JsonElementExt.FromObject(completions);
        }

        /**
         * Given a completion item fill in more data, like {@link CompletionItem.documentation doc-comment}
         * or {@link CompletionItem.detail details}.
         *
         * The editor will only resolve a completion item once.
         */
        [Obsolete("Please use the new async ResolveDelegate instead.")]
        public delegate CompletionItem ResolveCompletionItemDelegate(CompletionItem completionItem);
        [Obsolete("Please use the new async ResolveMethod instead.")]
        public ResolveCompletionItemDelegate ResolveCompletionItemFunc { get; set; }
        public delegate Task<CompletionItem> ResolveDelegate(CompletionItem completionItem);
        public ResolveDelegate ResolveMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ResolveCompletionItem))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ResolveCompletionItem(CompletionItem completionItem)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var completion = (await ResolveMethod?.Invoke(completionItem))
                ?? ResolveCompletionItemFunc?.Invoke(completionItem);
#pragma warning restore CS0618 // Type or member is obsolete
            return JsonElementExt.FromObject(completion);
        }

        [Obsolete("Please use the new constructor with async parameters instead.")]
        public CompletionItemProvider(ProvideCompletionItemsDelegate provideCompletionItems, ResolveCompletionItemDelegate resolveCompletionItem = null)
        {
            ProvideCompletionItemsFunc = provideCompletionItems;
            ResolveCompletionItemFunc = resolveCompletionItem;
        }

        public CompletionItemProvider(List<string> triggerCharacters, ProvideDelegate provideCompletionItems, ResolveDelegate resolveCompletionItem = null)
        {
            TriggerCharacters = triggerCharacters;
            ProvideMethod = provideCompletionItems;
            ResolveMethod = resolveCompletionItem;
        }
    }

    /**
     * How an {@link InlineCompletionsProvider inline completion provider} was triggered.
     */
    public enum InlineCompletionTriggerKind
    {
        /**
         * Completion was triggered automatically while editing.
         * It is sufficient to return a single completion item in this case.
         */
        Automatic = 0,
        /**
         * Completion was triggered explicitly by a user gesture.
         * Return multiple completion items to enable cycling through them.
         */
        Explicit = 1
    }

    /**
     * Arbitrary data that the provider can pass when firing {@link InlineCompletionsProvider.onDidChangeInlineCompletions}.
     * This data is passed back to the provider in {@link InlineCompletionContext.changeHint}.
     */
    public class InlineCompletionChangeHint
    {
        /**
         * Arbitrary data that the provider can use to identify what triggered the change.
         * This data must be JSON serializable.
         */
        public object Data { get; set; }
    }

    public class InlineCompletionContext
    {
        /**
         * How the completion was triggered.
         */
        public InlineCompletionTriggerKind TriggerKind { get; set; }
        public SelectedSuggestionInfo SelectedSuggestionInfo { get; set; }
        public bool IncludeInlineEdits { get; set; }
        public bool IncludeInlineCompletions { get; set; }
        public long RequestIssuedDateTime { get; set; }
        public long EarliestShownDateTime { get; set; }
        /**
         * The change hint that was passed to {@link InlineCompletionsProvider.onDidChangeInlineCompletions}.
         * Only set if this request was triggered by such an event.
         */
        public InlineCompletionChangeHint ChangeHint { get; set; }
    }


    public class SelectedSuggestionInfo
    {
        public Range Range { get; set; }
        public string Text { get; set; }
        public CompletionItemKind CompletionKind { get; set; }
        public bool IsSnippetText { get; set; }
    }


    public class CodeAction
    {
        public string Title { get; set; }
        public Command Command { get; set; }
        public WorkspaceEdit Edit { get; set; }
        public List<MarkerData> Diagnostics { get; set; }
        public string Kind { get; set; }
        public bool? IsPreferred { get; set; }
        public bool? IsAI { get; set; }
        public string Disabled { get; set; }
        public List<Range> Ranges { get; set; }
    }

    public enum CodeActionTriggerType
    {
        Invoke = 1,
        Auto = 2
    }

    public class CodeActionList : IDisposable
    {
        public List<CodeAction> Actions { get; set; }
        public void Dispose() { }
    }

    /**
     * Represents a parameter of a callable-signature. A parameter can
     * have a label and a doc-comment.
     */

    /**
     * Represents the signature of something callable. A signature
     * can have a label, like a function-name, a doc-comment, and
     * a set of parameters.
     */

    /**
     * Signature help represents the signature of something
     * callable. There can be multiple signatures but only one
     * active and only one active parameter.
     */


    public enum SignatureHelpTriggerKind
    {
        Invoke = 1,
        TriggerCharacter = 2,
        ContentChange = 3
    }


    /**
     * The signature help provider interface defines the contract between extensions and
     * the [parameter hints](https://code.visualstudio.com/docs/editor/intellisense)-feature.
     */

    /**
     * A document highlight kind.
     */
    public enum DocumentHighlightKind
    {
        /**
         * A textual occurrence.
         */
        Text = 0,
        /**
         * Read-access of a symbol, like reading a variable.
         */
        Read = 1,
        /**
         * Write-access of a symbol, like writing to a variable.
         */
        Write = 2
    }

    /**
     * A document highlight is a range inside a text document which deserves
     * special attention. Usually a document highlight is visualized by changing
     * the background color of its range.
     */

    /**
     * Represents a set of document highlights for a specific Uri.
     */

    /**
     * The document highlight provider interface defines the contract between extensions and
     * the word-highlight-feature.
     */

    /**
     * A provider that can provide document highlights across multiple documents.
     */

    /**
     * The linked editing range provider interface defines the contract between extensions and
     * the linked editing feature.
     */

    /**
     * Represents a list of ranges that can be edited together along with a word pattern to describe valid contents.
     */

    /**
     * Value-object that contains additional information when
     * requesting references.
     */

    /**
     * The reference provider interface defines the contract between extensions and
     * the [find references](https://code.visualstudio.com/docs/editor/editingevolved#_peek)-feature.
     */

    /**
     * Represents a location inside a resource, such as a line
     * inside a text file.
     */


    /**
     * The definition provider interface defines the contract between extensions and
     * the [go to definition](https://code.visualstudio.com/docs/editor/editingevolved#_go-to-definition)
     * and peek definition features.
     */

    /**
     * The definition provider interface defines the contract between extensions and
     * the [go to definition](https://code.visualstudio.com/docs/editor/editingevolved#_go-to-definition)
     * and peek definition features.
     */

    /**
     * The implementation provider interface defines the contract between extensions and
     * the go to implementation feature.
     */

    /**
     * The type definition provider interface defines the contract between extensions and
     * the go to type definition feature.
     */

    /**
     * A symbol kind.
     */
    public enum SymbolKind
    {
        File = 0,
        Module = 1,
        Namespace = 2,
        Package = 3,
        Class = 4,
        Method = 5,
        Property = 6,
        Field = 7,
        Constructor = 8,
        Enum = 9,
        Interface = 10,
        Function = 11,
        Variable = 12,
        Constant = 13,
        String = 14,
        Number = 15,
        Boolean = 16,
        Array = 17,
        Object = 18,
        Key = 19,
        Null = 20,
        EnumMember = 21,
        Struct = 22,
        Event = 23,
        Operator = 24,
        TypeParameter = 25
    }

    public enum SymbolTag
    {
        Deprecated = 1
    }


    /**
     * The document symbol provider interface defines the contract between extensions and
     * the [go to symbol](https://code.visualstudio.com/docs/editor/editingevolved#_go-to-symbol)-feature.
     */

    public class TextEdit
    {
        public Range Range { get; set; }
        public string Text { get; set; }
        public EndOfLineSequence? Eol { get; set; }
    }

    /**
     * Interface used to format a model
     */
    public class FormattingOptions
    {
        public int TabSize { get; set; }
        public bool InsertSpaces { get; set; }
    }

    /**
     * The document formatting provider interface defines the contract between extensions and
     * the formatting-feature.
     */
    public class DocumentFormattingEditProvider
    {
        public string DisplayName { get; set; }

        /**
         * Provide formatting edits for a whole document.
         */
        public delegate Task<TextEdit[]> ProvideDelegate(string modelUri, FormattingOptions options);
        public ProvideDelegate ProvideMethod { get; set; }

#if NET5_0_OR_GREATER
        [DynamicDependency(nameof(ProvideDocumentFormattingEdits))]
#endif
        [JSInvokable]
        public async Task<JsonElement?> ProvideDocumentFormattingEdits(string modelUri, FormattingOptions options)
        {
            var textEdits = await ProvideMethod?.Invoke(modelUri, options);
            return JsonElementExt.FromObject(textEdits);
        }

        public DocumentFormattingEditProvider(string displayName, ProvideDelegate provideDocumentFormattingEditsDelegate)
        {
            DisplayName = displayName;
            ProvideMethod = provideDocumentFormattingEditsDelegate;
        }
    }

    /**
     * The document formatting provider interface defines the contract between extensions and
     * the formatting-feature.
     */

    /**
     * The document formatting provider interface defines the contract between extensions and
     * the formatting-feature.
     */

    /**
     * A link inside the editor.
     */


    /**
     * A provider of links.
     */

    /**
     * A color in RGBA format.
     */

    /**
     * String representations for a color
     */

    /**
     * A color range is a range in a text model which represents a color.
     */

    /**
     * A provider of colors for editor models.
     */


    /**
     * A provider of folding ranges for editor models.
     */


    public class WorkspaceEditMetadata
    {
        public bool? NeedsConfirmation { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }

    public class WorkspaceFileEditOptions
    {
        public bool? Overwrite { get; set; }
        public bool? IgnoreIfNotExists { get; set; }
        public bool? IgnoreIfExists { get; set; }
        public bool? Recursive { get; set; }
        public bool? Copy { get; set; }
        public bool? Folder { get; set; }
        public bool? SkipTrashBin { get; set; }
        public int? MaxSize { get; set; }
    }

    public class WorkspaceFileEdit : IWorkspaceEdit
    {
        [JsonPropertyName("oldResource")]
        public string OldResourceUri { get; set; }
        [JsonPropertyName("newResource")]
        public string NewResourceUri { get; set; }
        public WorkspaceFileEditOptions Options { get; set; }
        public WorkspaceEditMetadata Metadata { get; set; }
    }

    public class WorkspaceTextEdit : IWorkspaceEdit
    {
        [JsonPropertyName("resource")]
        public string ResourceUri { get; set; }
        public TextEditWithOptions TextEdit { get; set; }
        public int? VersionId { get; set; }
        public WorkspaceEditMetadata Metadata { get; set; }
    }

    public class TextEditWithOptions : TextEdit
    {
        public bool? InsertAsSnippet { get; set; }
        public bool? KeepWhitespace { get; set; }
    }

    public interface IWorkspaceEdit
    {
        WorkspaceEditMetadata Metadata { get; set; }
    }

    public class WorkspaceEdit
    {
        [JsonConverter(typeof(ListJsonConverter<IWorkspaceEdit, WorkspaceEditJsonConverter>))]
        public List<IWorkspaceEdit> Edits { get; set; }
    }

    public class CustomEdit : IWorkspaceEdit
    {
        [JsonPropertyName("resource")]
        public string ResourceUri { get; set; }
        public WorkspaceEditMetadata Metadata { get; set; }
    }


    public enum NewSymbolNameTag
    {
        AIGenerated = 1
    }

    public enum NewSymbolNameTriggerKind
    {
        Invoke = 0,
        Automatic = 1
    }


    public class Command
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public List<object> Arguments { get; set; }
    }


    public enum InlayHintKind
    {
        Type = 1,
        Parameter = 2
    }


    /**
     * A Monarch language definition
     */

    /**
     * A rule is either a regular expression and an action
     * 		shorthands: [reg,act] == { regex: reg, action: act}
     *		and       : [reg,act,nxt] == { regex: reg, action: act{ next: nxt }}
     */


    /**
     * An action is either an array of actions...
     * ... or a case statement with guards...
     * ... or a basic action with a token value.
     */


    /**
     * This interface can be shortened as an array, ie. ['{','}','delimiter.curly']
     */

}

namespace BlazorMonaco
{
    /** @deprecated Use Options instead */
    /**
     * Custom CSS properties, at-directives, pseudoClasses and pseudoElements
     * https://github.com/microsoft/vscode-css-languageservice/blob/main/docs/customData.md
     */


    /**
     * Registers a new HTML language service for the languageId.
     * Note: 'html', 'handlebar' and 'razor' are registered by default.
     *
     * Use this method to registter additional language ids with a HTML service.
     * The language server has to be registered before an editor model is opened.
     */
    /**
     * Custom HTML tags attributes and attribute values
     * https://github.com/microsoft/vscode-html-languageservice/blob/main/docs/customData.md
     */


    public enum ModuleKind
    {
        None = 0,
        CommonJS = 1,
        AMD = 2,
        UMD = 3,
        System = 4,
        ES2015 = 5,
        ESNext = 99
    }

    public enum JsxEmit
    {
        None = 0,
        Preserve = 1,
        React = 2,
        ReactNative = 3,
        ReactJSX = 4,
        ReactJSXDev = 5
    }

    public enum NewLineKind
    {
        CarriageReturnLineFeed = 0,
        LineFeed = 1
    }

    public enum ScriptTarget
    {
        ES3 = 0,
        ES5 = 1,
        ES2015 = 2,
        ES2016 = 3,
        ES2017 = 4,
        ES2018 = 5,
        ES2019 = 6,
        ES2020 = 7,
        ESNext = 99,
        JSON = 100,
        Latest = 99
    }

    public enum ModuleResolutionKind
    {
        Classic = 1,
        NodeJs = 2
    }
}
