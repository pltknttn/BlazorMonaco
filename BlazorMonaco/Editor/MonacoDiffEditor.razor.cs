using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /**
     * A full-featured diff editor with a VS Code-like toolbar and status bar.
     *
     * The component intentionally derives from StandaloneDiffEditor, so all Monaco APIs,
     * events, OriginalEditor/ModifiedEditor and construction options remain available.
     */
    public partial class MonacoDiffEditor : StandaloneDiffEditor
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

        [Parameter] public string Theme { get; set; } = "vs";
        [Parameter] public double ZoomLevel { get; set; } = 1.0;
        [Parameter] public int? FontSize { get; set; }
        [Parameter] public int MinFontSize { get; set; } = 8;
        [Parameter] public int MaxFontSize { get; set; } = 40;
        [Parameter] public double MinZoomLevel { get; set; } = 0.5;
        [Parameter] public double MaxZoomLevel { get; set; } = 3.0;
        [Parameter] public string FontFamily { get; set; }
        [Parameter] public object FontLigatures { get; set; }
        [Parameter] public int? LineHeight { get; set; }
        [Parameter] public int? LetterSpacing { get; set; }
        [Parameter] public string WordWrap { get; set; } = "off";
        [Parameter] public int? WordWrapColumn { get; set; }
        [Parameter] public string WrappingIndent { get; set; }
        [Parameter] public bool? ReadOnly { get; set; }
        [Parameter] public bool MinimapEnabled { get; set; } = true;
        [Parameter] public string LineNumbers { get; set; }
        [Parameter] public string RenderWhitespace { get; set; }
        [Parameter] public string RenderLineHighlight { get; set; }
        [Parameter] public string CursorStyle { get; set; }
        [Parameter] public string CursorBlinking { get; set; }
        [Parameter] public bool? SmoothScrolling { get; set; }
        [Parameter] public bool? ScrollBeyondLastLine { get; set; }
        [Parameter] public bool? StickyScroll { get; set; }
        [Parameter] public bool? BracketPairColorization { get; set; }

        /**
         * Render the diff side by side (true) or inline (false).
         */
        [Parameter]
        public bool RenderSideBySide { get; set; } = true;

        /**
         * Ignore trailing whitespace when computing the diff.
         */
        [Parameter]
        public bool IgnoreTrimWhitespace { get; set; }

        #endregion

        #region Chrome parameters

        [Parameter] public bool ShowToolbar { get; set; } = true;
        [Parameter] public bool ShowStatusBar { get; set; } = true;
        [Parameter] public string Height { get; set; } = "500px";
        [Parameter] public string Width { get; set; } = "100%";

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
            ScrollBeyondLastLine, StickyScroll, BracketPairColorization, RenderSideBySide,
            IgnoreTrimWhitespace
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

            var options = new DiffEditorOptions
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
                    : null,
                RenderSideBySide = RenderSideBySide,
                IgnoreTrimWhitespace = IgnoreTrimWhitespace
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
        /// Called by JavaScript whenever the modified editor's cursor, selection or model changes.
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

        public async Task SetThemeAsync(string themeName)
        {
            Theme = themeName;
            await ApplyAppearanceAsync(applyTheme: true);
        }

        public async Task SetZoomLevelAsync(double zoomLevel)
        {
            ZoomLevel = MathExt.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetFontSizeAsync(int fontSize)
        {
            FontSize = MathExt.Clamp(fontSize, MinFontSize, MaxFontSize);
            _baseFontSize = FontSize.Value;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetFontFamilyAsync(string fontFamily)
        {
            FontFamily = fontFamily;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetWordWrapAsync(string wordWrap)
        {
            WordWrap = wordWrap;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetReadOnlyAsync(bool readOnly)
        {
            ReadOnly = readOnly;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetMinimapAsync(bool enabled)
        {
            MinimapEnabled = enabled;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetRenderSideBySideAsync(bool renderSideBySide)
        {
            RenderSideBySide = renderSideBySide;
            await ApplyAppearanceAsync(applyTheme: false);
        }

        public async Task SetIgnoreTrimWhitespaceAsync(bool ignoreTrimWhitespace)
        {
            IgnoreTrimWhitespace = ignoreTrimWhitespace;
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

        private Task OnRenderSideBySideChanged(bool renderSideBySide)
        {
            RenderSideBySide = renderSideBySide;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task OnIgnoreTrimWhitespaceChanged(bool ignoreTrimWhitespace)
        {
            IgnoreTrimWhitespace = ignoreTrimWhitespace;
            return ApplyAppearanceAsync(applyTheme: false);
        }

        private Task FormatDocument()
            => ModifiedEditor == null
                ? Task.CompletedTask
                : ModifiedEditor.Trigger("toolbar", "editor.action.formatDocument", null);

        private Task PreviousDiff()
            => GoToDiff("previous");

        private Task NextDiff()
            => GoToDiff("next");

        #endregion
    }
}
