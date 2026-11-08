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
