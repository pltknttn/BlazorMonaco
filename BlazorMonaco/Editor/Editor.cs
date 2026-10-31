using BlazorMonaco.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMonaco.Editor
{
    /**
     * An editor.
     */
    public class Editor : ComponentBase, IDisposable
    {
        #region Blazor

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public EventCallback OnDidDispose { get; set; }

        [Parameter]
        public EventCallback OnDidInit { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        internal DotNetObjectReference<Editor> _dotnetObjectRef;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            JsRuntimeExt.UpdateRuntime(JsRuntime); // Update the static IJSRuntime instance for WASM apps
            _dotnetObjectRef = DotNetObjectReference.Create(this);
        }

        public virtual void Dispose()
        {
            _dotnetObjectRef?.Dispose();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrWhiteSpace(Id))
                Id = $"blazor-monaco-{Guid.NewGuid()}";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetEventListeners();
                await OnDidInit.InvokeAsync(this);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        internal virtual async Task SetEventListeners()
        {
            if (OnDidDispose.HasDelegate)
                await SetEventListener("OnDidDispose");
        }

        internal Task SetEventListener(string eventName)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setEventListener", Id, eventName);

        public virtual async Task EventCallback(string eventName, string eventJson)
        {
            switch (eventName)
            {
                case "OnDidDispose": await OnDidDispose.InvokeAsync(this); break;
            }
        }

        protected Func<int, string> LineNumbersLambda { get; set; }

        [JSInvokable]
        public string LineNumbersCallback(int lineNumber)
        {
            return LineNumbersLambda?.Invoke(lineNumber) ?? lineNumber.ToString();
        }

        public Task ReloadLineNumbers()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.reloadLineNumbers", Id);

        #endregion

        /**
         * An event emitted when the editor has been disposed.
         * @event
         */
        /**
         * Dispose the editor.
         */
        public Task DisposeEditor()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.dispose", Id);
        /**
         * Get a unique id for this editor instance.
         */
        public Task<string> GetId()
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.getId", Id);
        /**
         * Get the editor type. Please see `EditorType`.
         * This is to avoid an instanceof check
         */
        public Task<string> GetEditorType()
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.getEditorType", Id);
        /**
         * Instructs the editor to remeasure its container. This method should
         * be called when the container of the editor gets resized.
         *
         * If a dimension is passed in, the passed in value will be used.
         *
         * By default, this will also render the editor immediately.
         * If you prefer to delay rendering to the next animation frame, use postponeRendering == true.
         */
        public Task Layout(Dimension dimension = null, bool? postponeRendering = null)
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.layout", Id, dimension, postponeRendering);
        /**
         * Brings browser focus to the editor text
         */
        public Task Focus()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.focus", Id);
        /**
         * Returns true if the text inside this editor is focused (i.e. cursor is blinking).
         */
        public Task<bool> HasTextFocus()
            => JsRuntime.SafeInvokeAsync<bool>("blazorMonaco.editor.hasTextFocus", Id);
        /**
         * Returns all actions associated with this editor.
         */
        public Task<List<EditorAction>> GetSupportedActions()
            => JsRuntime.SafeInvokeAsync<List<EditorAction>>("blazorMonaco.editor.getSupportedActions", Id);
        /**
         * Get the action with the specified id.
         * @param id The id of the action.
         */
        public Task<EditorAction> GetAction(string id)
            => JsRuntime.SafeInvokeAsync<EditorAction>("blazorMonaco.editor.getAction", Id, id);
        /**
         * Get the contribution with the specified id.
         * @param id The id of the contribution.
         */
        public Task<object> GetContribution(string id)
            => JsRuntime.SafeInvokeAsync<object>("blazorMonaco.editor.getContribution", Id, id);
        /**
         * Directly execute the command with the specified id.
         * @param source The source of the call.
         * @param commandId The id of the command.
         * @param args The args of the command (if any).
         */
        public Task ExecuteCommand(string source, string commandId, object args = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.executeCommand", Id, source, commandId, args);
        /**
         * Create a collection of decorations tracked by this editor.
         * @param owner The owner of the decorations.
         */
        public async Task<EditorDecorationsCollection> CreateDecorationsCollection(string owner = null)
        {
            var collection = await JsRuntime.SafeInvokeAsync<EditorDecorationsCollection>("blazorMonaco.editor.createDecorationsCollection", Id, owner);
            if (collection == null)
                collection = new EditorDecorationsCollection();
            collection.JsRuntime = JsRuntime;
            collection.EditorId = Id;
            collection.Owner = owner;
            return collection;
        }
        /**
         * Saves current view state of the editor in a serializable object.
         */
        public Task<object> SaveViewState()
            => JsRuntime.SafeInvokeAsync<object>("blazorMonaco.editor.saveViewState", Id);
        /**
         * Restores the view state of the editor from a serializable object generated by `saveViewState`.
         */
        public Task RestoreViewState(object state)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.restoreViewState", Id, state);
        /**
         * Given a position, returns a column number that takes tab-widths into account.
         */
        public Task<int> GetVisibleColumnFromPosition(Position position)
            => JsRuntime.SafeInvokeAsync<int>("blazorMonaco.editor.getVisibleColumnFromPosition", Id, position);
        /**
         * Returns the primary position of the cursor.
         */
        public Task<Position> GetPosition()
            => JsRuntime.SafeInvokeAsync<Position>("blazorMonaco.editor.getPosition", Id);
        /**
         * Set the primary position of the cursor. This will remove any secondary cursors.
         * @param position New primary cursor's position
         * @param source Source of the call that caused the position
         */
        public Task SetPosition(Position position, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setPosition", Id, position, source);
        /**
         * Scroll vertically as necessary and reveal a line.
         */
        public Task RevealLine(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLine", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line centered vertically.
         */
        public Task RevealLineInCenter(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLineInCenter", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line centered vertically only if it lies outside the viewport.
         */
        public Task RevealLineInCenterIfOutsideViewport(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLineInCenterIfOutsideViewport", Id, lineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal a line close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        public Task RevealLineNearTop(int lineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLineNearTop", Id, lineNumber, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position.
         */
        public Task RevealPosition(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPosition", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position centered vertically.
         */
        public Task RevealPositionInCenter(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPositionInCenter", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position centered vertically only if it lies outside the viewport.
         */
        public Task RevealPositionInCenterIfOutsideViewport(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPositionInCenterIfOutsideViewport", Id, position, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a position close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        public Task RevealPositionNearTop(Position position, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealPositionNearTop", Id, position, scrollType);
        /**
         * Returns the primary selection of the editor.
         */
        public Task<Selection> GetSelection()
            => JsRuntime.SafeInvokeAsync<Selection>("blazorMonaco.editor.getSelection", Id);
        /**
         * Returns all the selections of the editor.
         */
        public Task<List<Selection>> GetSelections()
            => JsRuntime.SafeInvokeAsync<List<Selection>>("blazorMonaco.editor.getSelections", Id);
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelection(Range selection, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelection", Id, selection, source);
        /**
         * Set the primary selection of the editor. This will remove any secondary cursors.
         * @param selection The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelection(Selection selection, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelection", Id, selection, source);
        /**
         * Set the selections for all the cursors of the editor.
         * Cursors will be removed or added, as necessary.
         * @param selections The new selection
         * @param source Source of the call that caused the selection
         */
        public Task SetSelections(List<Selection> selections, string source)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.setSelections", Id, selections, source);
        /**
         * Scroll vertically as necessary and reveal lines.
         */
        public Task RevealLines(int startLineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLines", Id, startLineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines centered vertically.
         */
        public Task RevealLinesInCenter(int lineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLinesInCenter", Id, lineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines centered vertically only if it lies outside the viewport.
         */
        public Task RevealLinesInCenterIfOutsideViewport(int lineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLinesInCenterIfOutsideViewport", Id, lineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically as necessary and reveal lines close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        public Task RevealLinesNearTop(int startLineNumber, int endLineNumber, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealLinesNearTop", Id, startLineNumber, endLineNumber, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range.
         */
        public Task RevealRange(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRange", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range centered vertically.
         */
        public Task RevealRangeInCenter(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeInCenter", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range at the top of the viewport.
         */
        public Task RevealRangeAtTop(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeAtTop", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range centered vertically only if it lies outside the viewport.
         */
        public Task RevealRangeInCenterIfOutsideViewport(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeInCenterIfOutsideViewport", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal the current cursors.
         */
        public Task RevealAllCursors(bool revealHorizontal, bool? minimalReveal = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealAllCursors", Id, revealHorizontal, minimalReveal);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
         * optimized for viewing a code definition.
         */
        public Task RevealRangeNearTop(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeNearTop", Id, range, scrollType);
        /**
         * Scroll vertically or horizontally as necessary and reveal a range close to the top of the viewport,
         * optimized for viewing a code definition. Only if it lies outside the viewport.
         */
        public Task RevealRangeNearTopIfOutsideViewport(Range range, ScrollType? scrollType = null)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.revealRangeNearTopIfOutsideViewport", Id, range, scrollType);

        /**
         * Get the container DOM node id of the editor.
         */
        public Task<string> GetContainerDomNode()
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.getContainerDomNodeId", Id);

        /**
         * Directly trigger a handler or an editor action.
         * @param source The source of the call.
         * @param handlerId The id of the handler or the id of a contribution.
         * @param payload Extra data to be sent to the handler.
         */
        public Task Trigger(string source, string handlerId, object payload = null)
        {
            var payloadJsonElement = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(payload));
            return JsRuntime.SafeInvokeAsync("blazorMonaco.editor.trigger", Id, source, handlerId, payloadJsonElement);
        }
        /**
         * Add a content widget.
         */
        public Task AddContentWidget(ContentWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.addContentWidget", Id, widget);
        /**
         * Layout/reposition a content widget. This is a ping to the editor to call widget.getPosition() again.
         */
        public Task LayoutContentWidget(ContentWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.layoutContentWidget", Id, widget);
        /**
         * Remove a content widget.
         */
        public Task RemoveContentWidget(ContentWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeContentWidget", Id, widget);
        /**
         * Remove all content widgets.
         */
        public Task RemoveAllContentWidgets()
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeAllContentWidgets", Id);
        /**
         * Get the current content widget position.
         */
        public Task<ContentWidgetPosition> GetContentWidgetPosition(ContentWidget widget)
            => JsRuntime.SafeInvokeAsync<ContentWidgetPosition>("blazorMonaco.editor.getContentWidgetPosition", Id, widget);
        /**
         * Add an overlay widget. Widgets must have unique ids, otherwise they will be overwritten.
         */
        public Task AddOverlayWidget(OverlayWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.addOverlayWidget", Id, widget);
        /**
         * Layout/reposition an overlay widget.
         */
        public Task LayoutOverlayWidget(OverlayWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.layoutOverlayWidget", Id, widget);
        /**
         * Remove an overlay widget.
         */
        public Task RemoveOverlayWidget(OverlayWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeOverlayWidget", Id, widget);
        /**
         * Get the current overlay widget position.
         */
        public Task<object> GetOverlayWidgetPosition(OverlayWidget widget)
            => JsRuntime.SafeInvokeAsync<object>("blazorMonaco.editor.getOverlayWidgetPosition", Id, widget);
        /**
         * Add a glyph margin widget. Widgets must have unique ids, otherwise they will be overwritten.
         */
        public Task AddGlyphMarginWidget(GlyphMarginWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.addGlyphMarginWidget", Id, widget);
        /**
         * Layout/reposition a glyph margin widget.
         */
        public Task LayoutGlyphMarginWidget(GlyphMarginWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.layoutGlyphMarginWidget", Id, widget);
        /**
         * Remove a glyph margin widget.
         */
        public Task RemoveGlyphMarginWidget(GlyphMarginWidget widget)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeGlyphMarginWidget", Id, widget);
        /**
         * Get the current glyph margin widget position.
         */
        public Task<GlyphMarginWidgetPosition> GetGlyphMarginWidgetPosition(GlyphMarginWidget widget)
            => JsRuntime.SafeInvokeAsync<GlyphMarginWidgetPosition>("blazorMonaco.editor.getGlyphMarginWidgetPosition", Id, widget);
        /**
         * Add a view zone after the line specified in `viewZone.AfterLineNumber`.
         */
        public Task<string> AddViewZone(ViewZone viewZone)
            => JsRuntime.SafeInvokeAsync<string>("blazorMonaco.editor.addViewZone", Id, viewZone);
        /**
         * Remove a view zone.
         */
        public Task RemoveViewZone(string viewZoneId)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.removeViewZone", Id, viewZoneId);
        /**
         * Relayout a view zone.
         */
        public Task LayoutViewZone(string viewZoneId)
            => JsRuntime.SafeInvokeAsync("blazorMonaco.editor.layoutViewZone", Id, viewZoneId);
        /**
         * Get a change accessor that can be used to add/remove/layout view zones.
         */
        public ViewZoneChangeAccessor CreateViewZoneChangeAccessor()
            => new ViewZoneChangeAccessor(JsRuntime, Id);
    }
}
