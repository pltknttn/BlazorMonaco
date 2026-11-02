using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /// <summary>
    /// Toolbar rendered by <see cref="MonacoCodeEditor"/> and <see cref="MonacoDiffEditor"/>.
    /// </summary>
    public partial class EditorToolbar
    {
        private static readonly string[] Themes = { "vs", "vs-dark", "hc-black", "hc-light" };
        private readonly string _themeSelectId = $"bm-theme-{Guid.NewGuid():N}";
        private int _fontSizeInput;
        private int _fontSizeInputVersion;

        [Parameter] public string Theme { get; set; } = "vs";
        [Parameter] public int FontSize { get; set; } = 14;
        [Parameter] public string WordWrap { get; set; } = "off";
        [Parameter] public bool MinimapEnabled { get; set; } = true;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public double ZoomLevel { get; set; } = 1.0;
        [Parameter] public bool IsDiff { get; set; }
        [Parameter] public bool RenderSideBySide { get; set; } = true;
        [Parameter] public bool IgnoreTrimWhitespace { get; set; }
        [Parameter] public int MinFontSize { get; set; } = 8;
        [Parameter] public int MaxFontSize { get; set; } = 40;
        [Parameter] public double MinZoomLevel { get; set; } = 0.5;
        [Parameter] public double MaxZoomLevel { get; set; } = 3.0;

        [Parameter] public EventCallback<string> ThemeChanged { get; set; }
        [Parameter] public EventCallback<int> FontSizeChanged { get; set; }
        [Parameter] public EventCallback<string> WordWrapChanged { get; set; }
        [Parameter] public EventCallback<bool> MinimapChanged { get; set; }
        [Parameter] public EventCallback<bool> ReadOnlyChanged { get; set; }
        [Parameter] public EventCallback<double> ZoomChanged { get; set; }
        [Parameter] public EventCallback<bool> RenderSideBySideChanged { get; set; }
        [Parameter] public EventCallback<bool> IgnoreTrimWhitespaceChanged { get; set; }
        [Parameter] public EventCallback FormatRequested { get; set; }
        [Parameter] public EventCallback PreviousDiffRequested { get; set; }
        [Parameter] public EventCallback NextDiffRequested { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (MinFontSize < 1)
                MinFontSize = 1;
            if (MaxFontSize < MinFontSize)
                MaxFontSize = MinFontSize;
            if (MinZoomLevel <= 0)
                MinZoomLevel = 0.1;
            if (MaxZoomLevel < MinZoomLevel)
                MaxZoomLevel = MinZoomLevel;

            _fontSizeInput = MathExt.Clamp(FontSize, MinFontSize, MaxFontSize);
        }

        private int ZoomPercent => (int)Math.Round(MathExt.Clamp(ZoomLevel, MinZoomLevel, MaxZoomLevel) * 100);

        private int CurrentFontSize => MathExt.Clamp(_fontSizeInput, MinFontSize, MaxFontSize);

        private bool CanDecreaseFontSize => CurrentFontSize > MinFontSize;
        private bool CanIncreaseFontSize => CurrentFontSize < MaxFontSize;
        private bool CanZoomOut => ZoomLevel > MinZoomLevel;
        private bool CanZoomIn => ZoomLevel < MaxZoomLevel;

        private string WordWrapLabel
        {
            get
            {
                switch (WordWrap)
                {
                    case "on":
                        return "On";
                    case "wordWrapColumn":
                        return "Column";
                    case "bounded":
                        return "Bounded";
                    default:
                        return "Off";
                }
            }
        }

        private Task ThemeChangedInternal(ChangeEventArgs args)
            => ThemeChanged.InvokeAsync(args.Value?.ToString());

        private Task FontSizeInputChanged(ChangeEventArgs args)
        {
            if (!int.TryParse(args.Value?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var fontSize))
            {
                ResetFontSizeInput();
                return Task.CompletedTask;
            }

            fontSize = MathExt.Clamp(fontSize, MinFontSize, MaxFontSize);
            if (fontSize == _fontSizeInput && fontSize == FontSize)
                return Task.CompletedTask;

            _fontSizeInput = fontSize;
            return FontSizeChanged.InvokeAsync(fontSize);
        }

        private void ResetFontSizeInput()
        {
            _fontSizeInput = MathExt.Clamp(FontSize, MinFontSize, MaxFontSize);
            // The rejected text is already present in the DOM. Recreate the input so it
            // displays the valid value again even when that value did not change.
            _fontSizeInputVersion++;
        }

        private Task DecreaseFontSize()
        {
            var next = Math.Max(MinFontSize, CurrentFontSize - 1);
            _fontSizeInput = next;
            return FontSizeChanged.InvokeAsync(next);
        }

        private Task IncreaseFontSize()
        {
            var next = Math.Min(MaxFontSize, CurrentFontSize + 1);
            _fontSizeInput = next;
            return FontSizeChanged.InvokeAsync(next);
        }

        private Task ToggleWordWrap()
        {
            string next;
            switch (WordWrap)
            {
                case "off":
                    next = "on";
                    break;
                case "on":
                    next = "wordWrapColumn";
                    break;
                case "wordWrapColumn":
                    next = "bounded";
                    break;
                default:
                    next = "off";
                    break;
            }

            return WordWrapChanged.InvokeAsync(next);
        }

        private Task ToggleMinimap()
            => MinimapChanged.InvokeAsync(!MinimapEnabled);

        private Task ToggleReadOnly()
            => ReadOnlyChanged.InvokeAsync(!ReadOnly);

        private Task ZoomOut()
            => ZoomChanged.InvokeAsync(MathExt.Clamp(Math.Round(ZoomLevel - 0.1, 1), MinZoomLevel, MaxZoomLevel));

        private Task ZoomReset()
            => ZoomChanged.InvokeAsync(MathExt.Clamp(1.0, MinZoomLevel, MaxZoomLevel));

        private Task ZoomIn()
            => ZoomChanged.InvokeAsync(MathExt.Clamp(Math.Round(ZoomLevel + 0.1, 1), MinZoomLevel, MaxZoomLevel));

        private Task ToggleDiffLayout()
            => RenderSideBySideChanged.InvokeAsync(!RenderSideBySide);

        private Task ToggleIgnoreWhitespace()
            => IgnoreTrimWhitespaceChanged.InvokeAsync(!IgnoreTrimWhitespace);

        private Task Format()
            => FormatRequested.InvokeAsync(null);

        private Task PreviousDiff()
            => PreviousDiffRequested.InvokeAsync(null);

        private Task NextDiff()
            => NextDiffRequested.InvokeAsync(null);
    }
}
