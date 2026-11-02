using Microsoft.AspNetCore.Components;
using System;

namespace BlazorMonaco.Editor
{
    /// <summary>
    /// Status bar rendered by <see cref="MonacoCodeEditor"/> and <see cref="MonacoDiffEditor"/>.
    /// </summary>
    public partial class EditorStatusBar
    {
        [Parameter] public EditorStatusInfo Status { get; set; } = new EditorStatusInfo();
        [Parameter] public string Theme { get; set; } = "vs";
        [Parameter] public double ZoomLevel { get; set; } = 1.0;
        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool IsDiff { get; set; }
        [Parameter] public bool RenderSideBySide { get; set; } = true;

        /// <summary>
        /// When false, the bar renders a neutral placeholder instead of default values.
        /// This avoids showing "0 lines"/"plaintext" while the editor is still being created
        /// or while a prerendered page is being hydrated.
        /// </summary>
        [Parameter] public bool IsInitialized { get; set; } = true;

        private int ZoomPercent => (int)Math.Round(ZoomLevel * 100);

        private string EolLabel => Status.Eol == "\r\n" ? "CRLF" : "LF";

        private string IndentLabel => Status.InsertSpaces
            ? $"Spaces: {Status.TabSize}"
            : $"Tab Size: {Status.TabSize}";
    }
}
