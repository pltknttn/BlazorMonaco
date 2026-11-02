namespace BlazorMonaco.Editor
{
    /// <summary>
    /// Snapshot of the editor state shown in <see cref="EditorStatusBar"/>.
    /// </summary>
    public class EditorStatusInfo
    {
        public int Line { get; set; } = 1;
        public int Column { get; set; } = 1;
        public int SelectionLength { get; set; }
        public int Selections { get; set; }
        public string Language { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public string Eol { get; set; } = "\n";
        public int TabSize { get; set; } = 4;
        public bool InsertSpaces { get; set; } = true;
    }
}
