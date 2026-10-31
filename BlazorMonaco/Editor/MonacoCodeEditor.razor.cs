using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /**
     * A code editor component with built-in UI functionality:
     * theme switching, zoom control, font size, and other editor appearance settings.
     */
    public partial class MonacoCodeEditor : StandaloneCodeEditor
    {
        #region Parameters

        /**
         * The theme to use for rendering.
         * Available built-in themes: 'vs', 'vs-dark', 'hc-black', 'hc-light'.
         * Custom themes can be defined via Global.DefineTheme.
         */
        [Parameter]
        public string Theme { get; set; }

        /**
         * The zoom level of the editor. 1.0 is default (100%).
         */
        [Parameter]
        public double ZoomLevel { get; set; } = 1.0;

        /**
         * The font size in pixels.
         */
        [Parameter]
        public int? FontSize { get; set; }

        /**
         * The font family.
         */
        [Parameter]
        public string FontFamily { get; set; }

        /**
         * Whether word wrap is enabled.
         */
        [Parameter]
        public string WordWrap { get; set; } = "off";

        /**
         * Whether the editor is read-only.
         */
        [Parameter]
        public bool? ReadOnly { get; set; }

        #endregion

        private string _prevTheme;
        private double _prevZoomLevel = 1.0;
        private int? _prevFontSize;
        private string _prevFontFamily;
        private string _prevWordWrap = "off";
        private bool? _prevReadOnly;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await ApplyAllSettingsAsync();
                _prevTheme = Theme;
                _prevZoomLevel = ZoomLevel;
                _prevFontSize = FontSize;
                _prevFontFamily = FontFamily;
                _prevWordWrap = WordWrap;
                _prevReadOnly = ReadOnly;
            }
            else
            {
                await ApplyChangedSettingsAsync();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            await ApplyChangedSettingsAsync();
        }

        private async Task ApplyChangedSettingsAsync()
        {
            if (Theme != _prevTheme)
            {
                _prevTheme = Theme;
                await ApplyThemeAsync();
            }
            if (Math.Abs(ZoomLevel - _prevZoomLevel) > 0.0001)
            {
                _prevZoomLevel = ZoomLevel;
                await ApplyZoomLevelAsync();
            }
            if (FontSize != _prevFontSize)
            {
                _prevFontSize = FontSize;
                await ApplyFontSizeAsync();
            }
            if (FontFamily != _prevFontFamily)
            {
                _prevFontFamily = FontFamily;
                await ApplyFontFamilyAsync();
            }
            if (WordWrap != _prevWordWrap)
            {
                _prevWordWrap = WordWrap;
                await ApplyWordWrapAsync();
            }
            if (ReadOnly != _prevReadOnly)
            {
                _prevReadOnly = ReadOnly;
                await ApplyReadOnlyAsync();
            }
        }

        private async Task ApplyAllSettingsAsync()
        {
            if (!string.IsNullOrEmpty(Theme))
                await ApplyThemeAsync();
            if (Math.Abs(ZoomLevel - 1.0) > 0.0001)
                await ApplyZoomLevelAsync();
            if (FontSize.HasValue)
                await ApplyFontSizeAsync();
            if (!string.IsNullOrEmpty(FontFamily))
                await ApplyFontFamilyAsync();
            if (!string.IsNullOrEmpty(WordWrap))
                await ApplyWordWrapAsync();
            if (ReadOnly.HasValue)
                await ApplyReadOnlyAsync();
        }

        /**
         * Switches the editor theme.
         */
        public async Task SetThemeAsync(string themeName)
        {
            Theme = themeName;
            await ApplyThemeAsync();
        }

        /**
         * Sets the zoom level of the editor.
         */
        public async Task SetZoomLevelAsync(double zoomLevel)
        {
            ZoomLevel = zoomLevel;
            await ApplyZoomLevelAsync();
        }

        /**
         * Sets the font size.
         */
        public async Task SetFontSizeAsync(int fontSize)
        {
            FontSize = fontSize;
            await ApplyFontSizeAsync();
        }

        /**
         * Sets the font family.
         */
        public async Task SetFontFamilyAsync(string fontFamily)
        {
            FontFamily = fontFamily;
            await ApplyFontFamilyAsync();
        }

        /**
         * Sets the word wrap mode.
         */
        public async Task SetWordWrapAsync(string wordWrap)
        {
            WordWrap = wordWrap;
            await ApplyWordWrapAsync();
        }

        /**
         * Sets whether the editor is read-only.
         */
        public async Task SetReadOnlyAsync(bool readOnly)
        {
            ReadOnly = readOnly;
            await ApplyReadOnlyAsync();
        }

        private async Task ApplyThemeAsync()
        {
            if (string.IsNullOrEmpty(Theme))
                return;

            await Global.DefineTheme(JsRuntime, Theme, new StandaloneThemeData
            {
                Base = Theme,
                Inherit = true
            });
            await Global.SetTheme(JsRuntime, Theme);
        }

        private async Task ApplyZoomLevelAsync()
        {
            await JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setZoomLevel", Id, ZoomLevel);
        }

        private async Task ApplyFontSizeAsync()
        {
            if (!FontSize.HasValue)
                return;

            await UpdateOptions(new EditorUpdateOptions
            {
                FontSize = FontSize.Value
            });
        }

        private async Task ApplyFontFamilyAsync()
        {
            if (string.IsNullOrEmpty(FontFamily))
                return;

            await UpdateOptions(new EditorUpdateOptions
            {
                FontFamily = FontFamily
            });
        }

        private async Task ApplyWordWrapAsync()
        {
            if (string.IsNullOrEmpty(WordWrap))
                return;

            await UpdateOptions(new EditorUpdateOptions
            {
                WordWrap = WordWrap
            });
        }

        private async Task ApplyReadOnlyAsync()
        {
            if (!ReadOnly.HasValue)
                return;

            await UpdateOptions(new EditorUpdateOptions
            {
                ReadOnly = ReadOnly.Value
            });
        }
    }
}
