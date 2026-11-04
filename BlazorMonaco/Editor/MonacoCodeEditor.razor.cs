using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /**
     * A full-featured code editor with a VS Code-like toolbar and status bar.
     *
     * The component intentionally derives from StandaloneCodeEditor, so all Monaco APIs,
     * events and construction options remain available.
     */
    public partial class MonacoCodeEditor : StandaloneCodeEditor
    {
        private static readonly JsonSerializerOptions s_statusJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private EditorStatusInfo _status = new EditorStatusInfo();
        private bool _initialized;
        private string _appliedKey;
        private string _appliedTheme;
        private int _baseFontSize = 14;

        #region Appearance parameters

        /**
         * The theme to use for rendering.
         * Available built-in themes: 'vs', 'vs-dark', 'hc-black', 'hc-light'.
         * Custom themes can be defined via Global.DefineTheme.
         */
        [Parameter]
        public string Theme { get; set; } = "vs";

        /**
         * The zoom level of the editor. 1.0 is default (100%).
         * Zoom is applied on top of FontSize, the same way VS Code's zoom works.
         */
        [Parameter]
        public double ZoomLevel { get; set; } = 1.0;

        /**
         * The font size in pixels. When not set, Monaco's default (14) is used.
         */
        [Parameter]
        public int? FontSize { get; set; }

        /**
         * The smallest font size (in base pixels) the toolbar can select.
         */
        [Parameter]
        public int MinFontSize { get; set; } = 8;

        /**
         * The largest font size (in base pixels) the toolbar can select.
         */
        [Parameter]
        public int MaxFontSize { get; set; } = 40;

        /**
         * The smallest zoom level the toolbar can select. 1.0 is default (100%).
         */
        [Parameter]
        public double MinZoomLevel { get; set; } = 0.5;

        /**
         * The largest zoom level the toolbar can select. 1.0 is default (100%).
         */
        [Parameter]
        public double MaxZoomLevel { get; set; } = 3.0;

        /**
         * The font family.
         */
        [Parameter]
        public string FontFamily { get; set; }

        /**
         * Enable font ligatures, e.g. `true` or `"on"`.
         */
        [Parameter]
        public object FontLigatures { get; set; }

        /**
         * The line height in pixels.
         */
        [Parameter]
        public int? LineHeight { get; set; }

        /**
         * The letter spacing in pixels.
         */
        [Parameter]
        public int? LetterSpacing { get; set; }

        /**
         * Whether word wrap is enabled: 'off', 'on', 'wordWrapColumn' or 'bounded'.
         */
        [Parameter]
        public string WordWrap { get; set; } = "off";

        /**
         * The column at which the editor wraps when WordWrap is 'wordWrapColumn' or 'bounded'.
         */
        [Parameter]
        public int? WordWrapColumn { get; set; }

        /**
         * Controls the indentation of wrapped lines: 'none', 'same', 'indent' or 'deepIndent'.
         */
        [Parameter]
        public string WrappingIndent { get; set; }

        /**
         * Whether the editor is read-only.
         */
        [Parameter]
        public bool? ReadOnly { get; set; }

        /**
         * Whether the minimap is displayed.
         */
        [Parameter]
        public bool MinimapEnabled { get; set; } = true;

        /**
         * Line numbers mode: 'on', 'off', 'relative', 'interval' or a function name.
         */
        [Parameter]
        public string LineNumbers { get; set; }

        /**
         * Render whitespace mode: 'none', 'boundary', 'selection', 'trailing' or 'all'.
         */
        [Parameter]
        public string RenderWhitespace { get; set; }

        /**
         * Render line highlight mode: 'none', 'gutter', 'line' or 'all'.
         */
        [Parameter]
        public string RenderLineHighlight { get; set; }

        /**
         * Cursor style: 'line', 'block', 'underline', 'line-thin', 'block-outline' or 'underline-thin'.
         */
        [Parameter]
        public string CursorStyle { get; set; }

        /**
         * Cursor blinking: 'blink', 'smooth', 'phase', 'expand' or 'solid'.
         */
        [Parameter]
        public string CursorBlinking { get; set; }

        /**
         * Whether scrolling is animated.
         */
        [Parameter]
        public bool? SmoothScrolling { get; set; }

        /**
         * Whether the editor can scroll beyond the last line.
         */
        [Parameter]
        public bool? ScrollBeyondLastLine { get; set; }

        /**
         * Whether sticky scroll is enabled.
         */
        [Parameter]
        public bool? StickyScroll { get; set; }

        /**
         * Whether bracket pair colorization is enabled.
         */
        [Parameter]
        public bool? BracketPairColorization { get; set; }

        #endregion

        #region Chrome parameters

        /**
         * Whether the built-in toolbar is rendered.
         */
        [Parameter]
        public bool ShowToolbar { get; set; } = true;

        /**
         * Whether the built-in status bar is rendered.
         */
        [Parameter]
        public bool ShowStatusBar { get; set; } = true;

        /**
         * The height of the editor component, e.g. '500px' or '100%'.
         */
        [Parameter]
        public string Height { get; set; } = "500px";

        /**
         * The width of the editor component.
         */
        [Parameter]
        public string Width { get; set; } = "100%";

        #endregion

        protected bool IsReadOnly => ReadOnly ?? false;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            // Normalize the configurable limits so clamping never receives an
            // inverted range from a caller.
            if (MinFontSize < 1)
                MinFontSize = 1;
            if (MaxFontSize < MinFontSize)
                MaxFontSize = MinFontSize;
            if (MinZoomLevel <= 0)
                MinZoomLevel = 0.1;
            if (MaxZoomLevel < MinZoomLevel)
                MaxZoomLevel = MinZoomLevel;

            if (FontSize.HasValue)
                _baseFontSize = MathExt.Clamp(FontSize.Value, MinFontSize, MaxFontSize);
        }

        protected double EffectiveZoomLevel => MathExt.Clamp(ZoomLevel, MinZoomLevel, MaxZoomLevel);

        protected int ToolbarFontSize => MathExt.Clamp(_baseFontSize, MinFontSize, MaxFontSize);

        protected int EffectiveFontSize => Math.Max(1, (int)Math.Round(ToolbarFontSize * EffectiveZoomLevel, MidpointRounding.AwayFromZero));

        private int? EffectiveLineHeight => LineHeight.HasValue
            ? Math.Max(1, (int)Math.Round(LineHeight.Value * EffectiveZoomLevel, MidpointRounding.AwayFromZero))
            : (int?)null;

        private int? EffectiveLetterSpacing => LetterSpacing.HasValue
            ? (int)Math.Round(LetterSpacing.Value * EffectiveZoomLevel, MidpointRounding.AwayFromZero)
            : (int?)null;

        private string ContainerStyle => $"width:{Width};height:{Height};";

        private string ThemeCssClass
        {
            get
            {
                switch (Theme)
                {
                    case "vs-dark":
                        return "bm-theme-dark";
                    case "hc-black":
                        return "bm-theme-hc";
                    case "hc-light":
                        return "bm-theme-hc-light";
                    default:
                        return "bm-theme-light";
                }
            }
        }

        private string AppearanceKey => string.Join("|", new object[]
        {
            Theme, EffectiveZoomLevel, MinFontSize, MaxFontSize, _baseFontSize, FontFamily, FontLigatures,
            LineHeight, LetterSpacing, WordWrap, WordWrapColumn, WrappingIndent, ReadOnly, MinimapEnabled,
            LineNumbers, RenderWhitespace, RenderLineHighlight, CursorStyle, CursorBlinking, SmoothScrolling,
            ScrollBeyondLastLine, StickyScroll, BracketPairColorization
        });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _initialized = true;
                if (FontSize.HasValue)
                    _baseFontSize = FontSize.Value;

                await ApplyAppearanceAsync(applyTheme: true);
                await AttachStatusListenerAsync();

                // The status bar is rendered before the editor is created. Refresh it once
                // the editor is ready so it never stays in the "Ready" placeholder state.
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (_initialized && AppearanceKey != _appliedKey)
            {
                await ApplyAppearanceAsync(applyTheme: Theme != _appliedTheme);
            }
        }

        private async Task ApplyAppearanceAsync(bool applyTheme)
        {
            if (applyTheme && !string.IsNullOrWhiteSpace(Theme))
                await Global.SetTheme(JsRuntime, Theme);

            if (FontSize.HasValue)
                _baseFontSize = MathExt.Clamp(FontSize.Value, MinFontSize, MaxFontSize);
            else
                _baseFontSize = MathExt.Clamp(_baseFontSize, MinFontSize, MaxFontSize);

            var options = new EditorUpdateOptions
            {
                FontSize = EffectiveFontSize,
                FontFamily = FontFamily,
                FontLigatures = FontLigatures,
                LineHeight = EffectiveLineHeight,
                LetterSpacing = EffectiveLetterSpacing,
                WordWrap = WordWrap,
                WordWrapColumn = WordWrapColumn,
                WrappingIndent = WrappingIndent,
                ReadOnly = ReadOnly,
                Minimap = new EditorMinimapOptions { Enabled = MinimapEnabled },
                LineNumbers = LineNumbers,
                RenderWhitespace = RenderWhitespace,
                RenderLineHighlight = RenderLineHighlight,
                CursorStyle = CursorStyle,
                CursorBlinking = CursorBlinking,
                SmoothScrolling = SmoothScrolling,
                ScrollBeyondLastLine = ScrollBeyondLastLine,
                StickyScroll = StickyScroll.HasValue
                    ? new EditorStickyScrollOptions { Enabled = StickyScroll }
                    : null,
                BracketPairColorization = BracketPairColorization.HasValue
                    ? new BracketPairColorizationOptions { Enabled = BracketPairColorization }
                    : null
            };

            await UpdateOptions(options);
            _appliedTheme = Theme;
            _appliedKey = AppearanceKey;
        }

        private async Task AttachStatusListenerAsync()
        {
            var initialStatus = await JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.setStatusEventListener", Id, _dotnetObjectRef);
            if (!string.IsNullOrEmpty(initialStatus))
                await UpdateStatus(initialStatus);
        }

        /// <summary>
        /// Called by JavaScript whenever the cursor, selection or model changes.
        /// </summary>
        [JSInvokable]
        public Task UpdateStatus(string statusJson)
        {
            if (string.IsNullOrEmpty(statusJson))
                return Task.CompletedTask;

            EditorStatusInfo status;
            try
            {
                status = JsonSerializer.Deserialize<EditorStatusInfo>(statusJson, s_statusJsonOptions);
            }
            catch (JsonException)
            {
                return Task.CompletedTask;
            }

            if (status == null)
                return Task.CompletedTask;

            return InvokeAsync(() =>
            {
                _status = status;
                StateHasChanged();
            });
        }

        #region Public appearance methods

        /// <summary>Switches the editor theme.</summary>
        public async Task SetThemeAsync(string themeName)
        {
            Theme = themeName;
            await ApplyAppearanceAsync(applyTheme: true);
        }

        /// <summary>Sets the zoom level of the editor.</summary>
        public async Task SetZoomLevelAsync(double zoomLevel)
        {
            ZoomLevel = MathExt.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
            await ApplyAppearanceAsync(applyTheme: false);
        }

        /// <summary>Sets the font size.</summary>
        public async Task SetFontSizeAsync(int fontSize)
        {
            FontSize = MathExt.Clamp(fontSize, MinFontSize, MaxFontSize);
            _baseFontSize = FontSize.Value;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        /// <summary>Sets the font family.</summary>
        public async Task SetFontFamilyAsync(string fontFamily)
        {
            FontFamily = fontFamily;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        /// <summary>Sets the word wrap mode.</summary>
        public async Task SetWordWrapAsync(string wordWrap)
        {
            WordWrap = wordWrap;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        /// <summary>Sets whether the editor is read-only.</summary>
        public async Task SetReadOnlyAsync(bool readOnly)
        {
            ReadOnly = readOnly;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        /// <summary>Shows or hides the minimap.</summary>
        public async Task SetMinimapAsync(bool enabled)
        {
            MinimapEnabled = enabled;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        #endregion

        #region Toolbar handlers

        private Task OnThemeChanged(string theme)
        {
            Theme = theme;
            return ApplyAppearanceAsync(applyTheme: true);
        }

        private Task OnFontSizeChanged(int fontSize)
        {
            var clamped = MathExt.Clamp(fontSize, MinFontSize, MaxFontSize);
            FontSize = clamped;
            _baseFontSize = clamped;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task OnWordWrapChanged(string wordWrap)
        {
            WordWrap = wordWrap;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task OnMinimapChanged(bool enabled)
        {
            MinimapEnabled = enabled;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task OnReadOnlyChanged(bool readOnly)
        {
            ReadOnly = readOnly;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task OnZoomChanged(double zoomLevel)
        {
            ZoomLevel = MathExt.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task FormatDocument()
            => Trigger("toolbar", "editor.action.formatDocument", null);

        #endregion
    }
}
