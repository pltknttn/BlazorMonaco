var require = { paths: { vs: document.baseURI + '_content/BlazorMonaco/lib/monaco-editor/min/vs' } };
window.blazorMonaco = window.blazorMonaco || {};
window.blazorMonaco.editors = [];
window.blazorMonaco.workers = {};

window.blazorMonaco.editor = {

    //#region Utilities

    /**
     * Monaco 0.55+ loads the editor and its language chunks as AMD modules, so
     * `monaco` is not defined immediately after editor.main.js has been parsed.
     * Blazor Server can start the interactive circuit before those modules have
     * finished loading, which used to make `create` fail with "monaco is undefined".
     * Resolve only after the editor module is actually ready.
     */
    waitForMonaco: function () {
        if (typeof monaco !== 'undefined' && monaco.editor != null)
            return Promise.resolve();

        if (window.blazorMonaco.monacoReadyPromise != null)
            return window.blazorMonaco.monacoReadyPromise;

        if (typeof require !== 'function' || typeof define !== 'function') {
            return Promise.reject(new Error("BlazorMonaco: Monaco Editor is not loaded. Please check that the script tags for loader.js and editor.main.js are present in your html file."));
        }

        window.blazorMonaco.monacoReadyPromise = new Promise((resolve, reject) => {
            require(['vs/editor/editor.main'], () => resolve(), error => {
                window.blazorMonaco.monacoReadyPromise = null;
                reject(error instanceof Error
                    ? error
                    : new Error("BlazorMonaco: Failed to load Monaco Editor (vs/editor/editor.main)."));
            });
        });
        return window.blazorMonaco.monacoReadyPromise;
    },

    getEditorHolder: function (id, silent = false) {
        let editorHolder = window.blazorMonaco.editors.find(h => h.id === id);

        let error = editorHolder == null
            ? "Couldn't find the editor with id: " + id + " editors.length: " + window.blazorMonaco.editors.length
            : editorHolder.editor == null
                ? "editor is null for editorHolder: " + editorHolder
                : null;
        if (error != null) {
            if (silent)
                return null;
            throw error;
        }

        return editorHolder;
    },

    getEditor: function (id, silent = false) {
        let editorHolder = this.getEditorHolder(id, silent);
        return editorHolder == null ? null : editorHolder.editor;
    },

    getEditorLineNumber: function (editorId, lineNumber) {
        const editorHolder = this.getEditorHolder(editorId, true);
        if (editorHolder == null) {
            return "";
        }

        let lineNumbersCache = editorHolder.lineNumbersCache;
        if (lineNumbersCache == null) {
            lineNumbersCache = editorHolder.lineNumbersCache = {};
            lineNumbersCache.Promises = {};
        }

        let lineNumberVal = lineNumbersCache[lineNumber];
        if (lineNumberVal != null) {
            return lineNumberVal;
        }

        lineNumbersCache[lineNumber] = "";
        lineNumbersCache.Promises[lineNumber] = editorHolder.dotnetRef.invokeMethodAsync("LineNumbersCallback", lineNumber)
            .then(result => {
                lineNumbersCache[lineNumber] = result;
                delete lineNumbersCache.Promises[lineNumber];
                if (Object.keys(lineNumbersCache.Promises).length == 0) {
                    this.redrawLineNumbers(editorId);
                }
            })
            .catch(err => {
                console.log("BlazorMonaco: LineNumbersCallback(" + lineNumber + ") failed with error : " + err);
                delete lineNumbersCache.Promises[lineNumber];
                if (Object.keys(lineNumbersCache.Promises).length == 0) {
                    this.redrawLineNumbers(editorId);
                }
            });
        return "";
    },

    reloadLineNumbers: function (editorId) {
        const editorHolder = this.getEditorHolder(editorId, true);
        if (editorHolder == null) {
            return;
        }
        editorHolder.lineNumbersCache = null;
        this.redrawLineNumbers(editorId);
    },

    redrawLineNumbers: function (editorId) {
        const editor = this.getEditor(editorId, true);
        if (editor == null) {
            return;
        }
        const optionValue = editor.getOption(monaco.editor.EditorOptions.lineNumbers.id);
        if (optionValue.renderType === monaco.editor.RenderLineNumbersType.Custom) {
            editor.updateOptions({
                lineNumbers: (lineNumber) => {
                    return this.getEditorLineNumber(editorId, lineNumber);
                }
            });
        }
    },

    convertUriToString: function (obj) {
        if (obj == null)
            return obj;
        if (Array.isArray(obj))
            return obj.map(this.convertUriToString);

        Object.keys(obj).forEach(k => {
            if (monaco.Uri.isUri(obj[k]))
                obj[k] = obj[k].toString();
            else if (typeof obj[k] === 'object')
                blazorMonaco.editor.convertUriToString(obj[k]);
        });

        return obj;
    },

    removeCircularReferences: function (orig) {
        if (Array.isArray(orig))
            return orig.map(this.removeCircularReferences);

        const circularReferenceReplacer = () => {
            const seen = new WeakSet();
            return (key, value) => {
                if (typeof value === "object" && value !== null) {
                    if (seen.has(value)) {
                        return;
                    }
                    seen.add(value);
                }
                return value;
            };
        };

        let json_str = JSON.stringify(orig, circularReferenceReplacer());
        return JSON.parse(json_str);
    },

    setWasm: function (isWasm) {
        window.blazorMonaco.isWasm = isWasm;
    },

    /**
     * Configure `self.MonacoEnvironment.getWorkerUrl` so Monaco can locate language workers
     * (json, css, html, typescript, ...). `baseUrl` should point to the folder that contains
     * the `vs` folder, e.g. `_content/BlazorMonaco/lib/monaco-editor/min`.
     */
    setDefaultWorkerUrl: function (baseUrl) {
        if (!baseUrl) baseUrl = '';
        if (!baseUrl.endsWith('/')) baseUrl += '/';
        window.MonacoEnvironment = window.MonacoEnvironment || {};
        window.MonacoEnvironment.getWorkerUrl = function (moduleId, label) {
            if (label === 'json' || label === 'jsonc') return baseUrl + 'vs/language/json/json.worker.js';
            if (label === 'css' || label === 'scss' || label === 'less') return baseUrl + 'vs/language/css/css.worker.js';
            if (label === 'html' || label === 'handlebars' || label === 'razor') return baseUrl + 'vs/language/html/html.worker.js';
            if (label === 'typescript' || label === 'javascript') return baseUrl + 'vs/language/typescript/ts.worker.js';
            return baseUrl + 'vs/editor/editor.worker.js';
        };
        return window.MonacoEnvironment;
    },

    /**
     * Register all localized strings at once as a plain JavaScript object.
     * This is the fast path: Monaco uses the object directly without any JSInterop
     * round-trips per string. Call this once during app startup.
     *
     * @param strings A dictionary/object of { key: localizedValue } pairs.
     */
    registerLocalizationStrings: function (strings) {
        window.MonacoEnvironment = window.MonacoEnvironment || {};
        window.MonacoEnvironment.localizedStrings = strings || {};
        return window.MonacoEnvironment;
    },

    /**
     * Register the C# MonacoLocalization reference so Monaco can resolve localized strings
     * from .NET via `blazorMonaco.editor.registerLocalization`.
     * Use this as a fallback for strings that are added after startup or resolved
     * dynamically. Every missing key triggers a JSInterop call back to .NET.
     */
    registerLocalization: function (dotnetRef) {
        window.MonacoEnvironment = window.MonacoEnvironment || {};
        window.MonacoEnvironment.localizedStrings = function (key) {
            return dotnetRef.invokeMethodAsync("GetLocalizedString", key);
        };
        return window.MonacoEnvironment;
    },

    toContentWidget: function (widget) {
        return {
            getId: () => widget.id,
            getDomNode: () => document.getElementById(widget.domNodeId),
            getPosition: () => widget.position,
            allowEditorOverflow: widget.allowEditorOverflow,
            useDisplayNone: widget.useDisplayNone,
            suppressMouseDown: widget.suppressMouseDown
        };
    },

    toOverlayWidget: function (widget) {
        return {
            getId: () => widget.id,
            getDomNode: () => document.getElementById(widget.domNodeId),
            getPosition: () => widget.position,
            allowEditorOverflow: widget.allowEditorOverflow
        };
    },

    toGlyphMarginWidget: function (widget) {
        return {
            getId: () => widget.id,
            getDomNode: () => document.getElementById(widget.domNodeId),
            getPosition: () => widget.position
        };
    },

    toViewZone: function (viewZone) {
        return {
            afterLineNumber: viewZone.afterLineNumber,
            afterColumn: viewZone.afterColumn,
            showInHiddenAreas: viewZone.showInHiddenAreas,
            ordinal: viewZone.ordinal,
            suppressMouseDown: viewZone.suppressMouseDown,
            heightInLines: viewZone.heightInLines,
            heightInPx: viewZone.heightInPx,
            minWidthInPx: viewZone.minWidthInPx,
            domNode: document.getElementById(viewZone.domNodeId),
            marginDomNode: viewZone.marginDomNodeId ? document.getElementById(viewZone.marginDomNodeId) : null
        };
    },

    //#region Static methods

    colorize: async function (text, languageId, options) {
        let promise = monaco.editor.colorize(text, languageId, options);
        return await promise;
    },

    colorizeElement: async function (elementId, options) {
        let promise = monaco.editor.colorizeElement(document.getElementById(elementId), options);
        await promise;
    },

    colorizeModelLine: function (uriStr, lineNumber, tabSize) {
        let model = this.model.getModel(uriStr);
        if (model == null)
            return null;
        return monaco.editor.colorizeModelLine(model, lineNumber, tabSize);
    },

    create: async function (id, options, override, dotnetRef) {
        await this.waitForMonaco();

        if (options == null)
            options = {};

        let oldEditor = this.getEditor(id, true);
        if (oldEditor !== null) {
            if (options.value == null) {
                options.value = oldEditor.getValue();
            }

            window.blazorMonaco.editors.splice(window.blazorMonaco.editors.findIndex(h => h.id === id), 1);
            oldEditor.dispose();
        }

        if (options.lineNumbers == "function") {
            options.lineNumbers = (lineNumber) => {
                return this.getEditorLineNumber(id, lineNumber);
            };
        }

        let editor = monaco.editor.create(document.getElementById(id), options, override);
        window.blazorMonaco.editors.push({ id: id, editor: editor, dotnetRef: dotnetRef });
    },

    createDiffEditor: async function (id, options, override, dotnetRef, dotnetRefOriginal, dotnetRefModified) {
        await this.waitForMonaco();

        if (options == null)
            options = {};

        let oldEditor = this.getEditor(id, true);
        let oldModel = null;
        if (oldEditor !== null) {
            oldModel = oldEditor.getModel();

            window.blazorMonaco.editors.splice(window.blazorMonaco.editors.findIndex(h => h.id === id + "_original"), 1);
            window.blazorMonaco.editors.splice(window.blazorMonaco.editors.findIndex(h => h.id === id + "_modified"), 1);
            window.blazorMonaco.editors.splice(window.blazorMonaco.editors.findIndex(h => h.id === id), 1);
            oldEditor.dispose();
        }

        if (options.lineNumbers == "function") {
            options.lineNumbers = (lineNumber) => {
                return this.getEditorLineNumber(id, lineNumber);
            };
        }

        let editor = monaco.editor.createDiffEditor(document.getElementById(id), options, override);
        window.blazorMonaco.editors.push({ id: id, editor: editor, dotnetRef: dotnetRef });
        window.blazorMonaco.editors.push({ id: id + "_original", editor: editor.getOriginalEditor(), dotnetRef: dotnetRefOriginal });
        window.blazorMonaco.editors.push({ id: id + "_modified", editor: editor.getModifiedEditor(), dotnetRef: dotnetRefModified });

        if (oldModel !== null && oldModel?.original !== null && oldModel?.modified !== null)
            editor.setModel(oldModel);
    },

    createModel: async function (value, language, uriStr) {
        await this.waitForMonaco();

        //uri is the key; if no uri exists, create one
        if (uriStr == null || uriStr == "") {
            uriStr = "generatedUriKey_" + this.uuidv4();
        }

        let uri = monaco.Uri.parse(uriStr);
        let model = monaco.editor.createModel(value, language, uri);
        if (model == null)
            return null;

        return {
            id: model.id,
            uri: model.uri.toString()
        };
    },

    defineTheme: function (themeName, themeData) {
        monaco.editor.defineTheme(themeName, themeData);
    },

    getModel: function (uriStr) {
        let model = this.model.getModel(uriStr);
        if (model == null)
            return null;

        return {
            id: model.id,
            uri: model.uri.toString()
        };
    },

    getModels: function () {
        return monaco.editor.getModels().map(function (value) {
            return {
                id: value.id,
                uri: value.uri.toString()
            };
        });
    },

    remeasureFonts: function () {
        monaco.editor.remeasureFonts();
    },

    setModelLanguage: function (uriStr, languageId) {
        let model = this.model.getModel(uriStr);
        if (model == null)
            return;
        monaco.editor.setModelLanguage(model, languageId);
    },

    setModelMarkers: function (uriStr, owner, markers) {
        let model = this.model.getModel(uriStr);
        if (model == null)
            return;
        monaco.editor.setModelMarkers(model, owner, markers);
    },

    removeAllMarkers: function (owner) {
        monaco.editor.removeAllMarkers(owner);
    },

    getModelMarkers: function (filter) {
        let markers = monaco.editor.getModelMarkers(filter);
        return blazorMonaco.editor.convertUriToString(markers);
    },

    setTheme: function (theme) {
        monaco.editor.setTheme(theme);
    },

    setZoomLevel: function (id, zoomLevel) {
        if (id == null) {
            // Global zoom level
            monaco.editor.setZoomLevel(zoomLevel);
            return;
        }
        let editor = this.getEditor(id);
        if (editor) {
            editor.updateOptions({ fontSize: editor.getOption(monaco.editor.EditorOptions.fontSize.id) * zoomLevel });
        }
    },

    getZoomLevel: function () {
        return monaco.editor.getZoomLevel();
    },

    getEditors: function () {
        return monaco.editor.getEditors().map(function (editor) {
            return editor.getId();
        });
    },

    getDiffEditors: function () {
        return monaco.editor.getDiffEditors().map(function (editor) {
            return editor.getId();
        });
    },

    tokenize: function (text, languageId) {
        return monaco.editor.tokenize(text, languageId);
    },

    registerCommand: function (commandId, dotnetRef) {
        monaco.editor.registerCommand(commandId, function (accessor) {
            var args = Array.prototype.slice.call(arguments, 1);
            return dotnetRef.invokeMethodAsync("Invoke", commandId, args);
        });
    },

    registerLinkOpener: async function (linkOpenerRef) {
        await monaco.editor.registerLinkOpener({
            open: (resource, isTrusted) => {
                return linkOpenerRef.invokeMethodAsync("OpenLink", resource.toString());
            }
        });
    },

    registerEditorOpener: async function (codeEditorOpenerRef) {
        await monaco.editor.registerEditorOpener({
            openCodeEditor: (source, resource, selectionOrPosition) => {
                let holder = window.blazorMonaco.editors.find(h => h.editor === source);
                return codeEditorOpenerRef.invokeMethodAsync("OpenCodeEditorLink", holder ? holder.id : null, resource.toString(), selectionOrPosition);
            }
        });
    },

    addKeybindingRule: function (rule) {
        monaco.editor.addKeybindingRule(rule);
    },

    addKeybindingRules: function (rules) {
        monaco.editor.addKeybindingRules(rules);
    },

    registerLifecycleListeners: async function (lifecycleRef) {
        if (window.blazorMonaco.lifecycleListenersRegistered) return;
        window.blazorMonaco.lifecycleListenersRegistered = true;
        window.blazorMonaco.lifecycleRef = lifecycleRef;
        try {
            await this.waitForMonaco();
        } catch (error) {
            console.warn("BlazorMonaco: monaco is not loaded; lifecycle listeners will not be registered.", error);
            return;
        }
        monaco.editor.onDidCreateEditor(editor => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnDidCreateEditor", JSON.stringify({ id: editor.getId() }));
        });
        monaco.editor.onDidCreateDiffEditor(editor => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnDidCreateDiffEditor", JSON.stringify({ id: editor.getId() }));
        });
        monaco.editor.onDidChangeModelLanguage(e => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnDidChangeModelLanguage", JSON.stringify({ uri: e.model.uri.toString(), oldLanguage: e.oldLanguage }));
        });
        monaco.editor.onDidChangeMarkers(resources => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnDidChangeMarkers", JSON.stringify({ resources: resources.map(u => u.toString()) }));
        });
        monaco.editor.onDidCreateModel(model => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnDidCreateModel", JSON.stringify({ uri: model.uri.toString() }));
        });
        monaco.editor.onWillDisposeModel(model => {
            lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnWillDisposeModel", JSON.stringify({ uri: model.uri.toString() }));
        });
    },

    createWebWorker: async function (options, hostRef) {
        if (!options || !options.workerUrl)
            throw new Error("BlazorMonaco: WebWorkerOptions.WorkerUrl is required.");

        let host = null;
        if (hostRef) {
            // Expose the C# WorkerHost to the worker as `context.host`. Every property
            // access returns a function that forwards the call to .NET.
            host = new Proxy({}, {
                get: function (_, property) {
                    if (property === "then")
                        return undefined;
                    return function (...args) {
                        return hostRef.invokeMethodAsync("InvokeAsync", String(property), args);
                    };
                }
            });
        }
        else if (options.host) {
            host = options.host;
        }

        let workerOpts = {
            worker: new Worker(options.workerUrl),
            host: host,
            keepIdleModels: options.keepIdleModels
        };
        let worker = monaco.editor.createWebWorker(workerOpts);
        let id = blazorMonaco.editor.uuidv4();
        window.blazorMonaco.workers[id] = worker;
        return id;
    },

    addCommandDescriptor: async function (descriptor) {
        await monaco.editor.addCommand(descriptor);
    },

    addEditorAction: async function (descriptor) {
        await monaco.editor.addEditorAction(descriptor);
    },

    //#endregion

    //#region Instance methods

    addAction: function (id, actionDescriptor) {
        let editorHolder = this.getEditorHolder(id);
        editorHolder.editor.addAction({
            id: actionDescriptor.id,
            label: actionDescriptor.label,
            keybindings: actionDescriptor.keybindings,
            precondition: actionDescriptor.precondition,
            keybindingContext: actionDescriptor.keybindingContext,
            contextMenuGroupId: actionDescriptor.contextMenuGroupId,
            contextMenuOrder: actionDescriptor.contextMenuOrder,
            run: function (editor, args) {
                editorHolder.dotnetRef.invokeMethodAsync("ActionCallback", actionDescriptor.id);
            }
        });
    },

    addCommand: function (id, keybinding, context) {
        let editorHolder = this.getEditorHolder(id);
        editorHolder.editor.addCommand(keybinding, function (args) {
            editorHolder.dotnetRef.invokeMethodAsync("CommandCallback", keybinding);
        }, context);
    },

    getSupportedActions: function (id) {
        let editor = this.getEditor(id);
        let actions = editor.getSupportedActions();
        if (actions == null)
            return [];
        return actions.map(function (action) {
            return {
                id: action.id,
                label: action.label,
                alias: action.alias,
                metadata: action.metadata
            };
        });
    },

    getAction: function (id, actionId) {
        let editor = this.getEditor(id);
        let action = editor.getAction(actionId);
        if (action == null)
            return null;
        return {
            id: action.id,
            label: action.label,
            alias: action.alias,
            metadata: action.metadata
        };
    },

    getContribution: function (id, contributionId) {
        let editor = this.getEditor(id);
        let contribution = editor.getContribution(contributionId);
        if (contribution == null)
            return null;
        return blazorMonaco.editor.removeCircularReferences(contribution);
    },

    executeCommand: function (id, source, commandId, args) {
        let editor = this.getEditor(id);
        let command = { id: commandId, args: args };
        editor.executeCommand(source, command);
    },

    createDecorationsCollection: function (id, owner) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        let ranges = collection.getRanges().map(function (r) {
            return { startLineNumber: r.startLineNumber, startColumn: r.startColumn, endLineNumber: r.endLineNumber, endColumn: r.endColumn };
        });
        return {
            id: editor.getId(),
            owner: owner,
            length: collection.length,
            ranges: ranges
        };
    },

    decorationsSet: function (id, owner, newDecorations) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        return collection.set(newDecorations);
    },

    decorationsAppend: function (id, owner, newDecorations) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        return collection.append(newDecorations);
    },

    decorationsClear: function (id, owner) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        collection.clear();
    },

    decorationsHas: function (id, owner, decoration) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        return collection.has(decoration);
    },

    decorationsGetRange: function (id, owner, index) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        let range = collection.getRange(index);
        if (range == null) return null;
        return { startLineNumber: range.startLineNumber, startColumn: range.startColumn, endLineNumber: range.endLineNumber, endColumn: range.endColumn };
    },

    decorationsGetRanges: function (id, owner) {
        let editor = this.getEditor(id);
        let collection = editor.createDecorationsCollection(owner);
        return collection.getRanges();
    },

    addContentWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.addContentWidget(blazorMonaco.editor.toContentWidget(widget));
    },

    layoutContentWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.layoutContentWidget(blazorMonaco.editor.toContentWidget(widget));
    },

    removeContentWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.removeContentWidget(blazorMonaco.editor.toContentWidget(widget));
    },

    removeAllContentWidgets: function (id) {
        let editor = this.getEditor(id);
        editor.removeAllContentWidgets();
    },

    getContentWidgetPosition: function (id, widget) {
        let editor = this.getEditor(id);
        let pos = editor.getContentWidgetPosition(blazorMonaco.editor.toContentWidget(widget));
        return blazorMonaco.editor.convertUriToString(pos);
    },

    addOverlayWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.addOverlayWidget(blazorMonaco.editor.toOverlayWidget(widget));
    },

    layoutOverlayWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.layoutOverlayWidget(blazorMonaco.editor.toOverlayWidget(widget));
    },

    removeOverlayWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.removeOverlayWidget(blazorMonaco.editor.toOverlayWidget(widget));
    },

    getOverlayWidgetPosition: function (id, widget) {
        let editor = this.getEditor(id);
        let pos = editor.getOverlayWidgetPosition(blazorMonaco.editor.toOverlayWidget(widget));
        return blazorMonaco.editor.convertUriToString(pos);
    },

    addGlyphMarginWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.addGlyphMarginWidget(blazorMonaco.editor.toGlyphMarginWidget(widget));
    },

    layoutGlyphMarginWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.layoutGlyphMarginWidget(blazorMonaco.editor.toGlyphMarginWidget(widget));
    },

    removeGlyphMarginWidget: function (id, widget) {
        let editor = this.getEditor(id);
        editor.removeGlyphMarginWidget(blazorMonaco.editor.toGlyphMarginWidget(widget));
    },

    getGlyphMarginWidgetPosition: function (id, widget) {
        let editor = this.getEditor(id);
        let pos = editor.getGlyphMarginWidgetPosition(blazorMonaco.editor.toGlyphMarginWidget(widget));
        return blazorMonaco.editor.convertUriToString(pos);
    },

    addViewZone: function (id, viewZone) {
        let editor = this.getEditor(id);
        let zoneId = null;
        editor.changeViewZones(accessor => {
            zoneId = accessor.addZone(blazorMonaco.editor.toViewZone(viewZone));
        });
        return zoneId;
    },

    removeViewZone: function (id, viewZoneId) {
        let editor = this.getEditor(id);
        editor.changeViewZones(accessor => {
            accessor.removeZone(viewZoneId);
        });
    },

    layoutViewZone: function (id, viewZoneId) {
        let editor = this.getEditor(id);
        editor.changeViewZones(accessor => {
            accessor.layoutZone(viewZoneId);
        });
    },

    goToDiff: function (id, target) {
        let editor = this.getEditor(id);
        editor.goToDiff(target);
    },

    revealFirstDiff: function (id) {
        let editor = this.getEditor(id);
        editor.revealFirstDiff();
    },

    getModifiedEditor: function (id) {
        let editor = this.getEditor(id);
        return editor.getModifiedEditor();
    },
    getOriginalEditor: function (id) {
        let editor = this.getEditor(id);
        return editor.getOriginalEditor();
    },
    getLineChanges: function (id) {
        let editor = this.getEditor(id);
        return editor.getLineChanges();
    },
    accessibleDiffViewerNext: function (id) {
        let editor = this.getEditor(id);
        editor.accessibleDiffViewerNext();
    },

    accessibleDiffViewerPrev: function (id) {
        let editor = this.getEditor(id);
        editor.accessibleDiffViewerPrev();
    },

    deltaDecorations: function (id, oldDecorations, newDecorations) {
        let editor = this.getEditor(id);
        return editor.deltaDecorations(oldDecorations, newDecorations);
    },

    removeDecorations: function (id, decorationsIds) {
        let editor = this.getEditor(id);
        return editor.removeDecorations(decorationsIds);
    },

    getDecorationsInRange: function (id, range, ownerId, filterOutValidation, filterFontDecorations, onlyMinimapDecorations, onlyMarginDecorations) {
        let editor = this.getEditor(id);
        return editor.getDecorationsInRange(range, ownerId, filterOutValidation, filterFontDecorations, onlyMinimapDecorations, onlyMarginDecorations);
    },
    getLineDecorations: function (id, lineNumber, ownerId, filterOutValidation, filterFontDecorations) {
        let editor = this.getEditor(id);
        return editor.getLineDecorations(lineNumber, ownerId, filterOutValidation, filterFontDecorations);
    },
    executeCommands: function (id, source, commands) {
        let editor = this.getEditor(id);
        editor.executeCommands(source, commands);
    },
    dispose: function (id) {
        let editor = this.getEditor(id);
        editor.dispose();
    },

    focus: function (id) {
        let editor = this.getEditor(id);
        editor.focus();
    },

    executeEdits: function (id, source, edits, endCursorState) {
        let editorHolder = this.getEditorHolder(id);
        if (endCursorState == "function") {
            if (window.blazorMonaco.isWasm) {
                endCursorState = (inverseEditOperations) => {
                    return editorHolder.dotnetRef.invokeMethod("ExecuteEditsCallback", inverseEditOperations);
                }
            }
            else {
                console.warn("BlazorMonaco: Editor.ExecuteEdits() supports endCursorState lambda only in Blazor Wasm apps. Please use the other ExecuteEdits override that takes List<Selection> instead.");
                endCursorState = null;
            }
        }
        return editorHolder.editor.executeEdits(source, edits, endCursorState);
    },

    getContainerDomNodeId: function (id) {
        let editor = this.getEditor(id);
        let containerNode = editor.getContainerDomNode();
        if (containerNode == null)
            return null;
        return containerNode.id;
    },

    getContentHeight: function (id) {
        let editor = this.getEditor(id);
        return editor.getContentHeight();
    },

    getContentWidth: function (id) {
        let editor = this.getEditor(id);
        return editor.getContentWidth();
    },

    getDomNodeId: function (id) {
        let editor = this.getEditor(id);
        let domeNode = editor.getDomNode();
        if (domeNode == null)
            return null;
        return domeNode.id;
    },

    getEditorType: function (id) {
        let editor = this.getEditor(id);
        return editor.getEditorType();
    },

    getInstanceModel: function (id) {
        let editor = this.getEditor(id);
        let model = editor.getModel();
        if (model == null)
            return null;

        return {
            id: model.id,
            uri: model.uri.toString()
        };
    },

    getInstanceDiffModel: function (id) {
        let editor = this.getEditor(id);
        let model = editor.getModel();
        if (model == null)
            return null;

        return {
            original: {
                id: model.original.id,
                uri: model.original.uri.toString()
            },
            modified: {
                id: model.modified.id,
                uri: model.modified.uri.toString()
            },
        };
    },

    getLayoutInfo: function (id) {
        let editor = this.getEditor(id);
        return editor.getLayoutInfo();
    },

    getOffsetForColumn: function (id, lineNumber, column) {
        let editor = this.getEditor(id);
        return editor.getOffsetForColumn(lineNumber, column);
    },

    getWidthOfLine: function (id, lineNumber) {
        let editor = this.getEditor(id);
        return editor.getWidthOfLine(lineNumber);
    },

    getOption: function (id, optionId) {
        let editor = this.getEditor(id);
        return JSON.stringify(editor.getOption(optionId));
    },

    getOptions: function (id) {
        let editor = this.getEditor(id);
        return editor.getOptions()._values.map(function (value) {
            return JSON.stringify(value);
        });
    },

    getPosition: function (id) {
        let editor = this.getEditor(id);
        return editor.getPosition();
    },

    getRawOptions: function (id) {
        let editor = this.getEditor(id);
        return editor.getRawOptions();
    },

    getScrollHeight: function (id) {
        let editor = this.getEditor(id);
        return editor.getScrollHeight();
    },

    getScrollLeft: function (id) {
        let editor = this.getEditor(id);
        return editor.getScrollLeft();
    },

    getScrollTop: function (id) {
        let editor = this.getEditor(id);
        return editor.getScrollTop();
    },

    getScrollWidth: function (id) {
        let editor = this.getEditor(id);
        return editor.getScrollWidth();
    },

    getScrolledVisiblePosition: function (id, position) {
        let editor = this.getEditor(id);
        return editor.getScrolledVisiblePosition(position);
    },

    getSelection: function (id) {
        let editor = this.getEditor(id);
        return editor.getSelection();
    },

    getSelections: function (id) {
        let editor = this.getEditor(id);
        return editor.getSelections();
    },

    getTargetAtClientPoint: function (id, clientX, clientY) {
        let editor = this.getEditor(id);
        return editor.getTargetAtClientPoint(clientX, clientY);
    },

    getFontSizeAtPosition: function (id, position) {
        let editor = this.getEditor(id);
        return editor.getFontSizeAtPosition(position);
    },

    getTopForLineNumber: function (id, lineNumber, includeViewZones) {
        let editor = this.getEditor(id);
        return editor.getTopForLineNumber(lineNumber, includeViewZones);
    },

    getBottomForLineNumber: function (id, lineNumber) {
        let editor = this.getEditor(id);
        return editor.getBottomForLineNumber(lineNumber);
    },

    getTopForPosition: function (id, lineNumber, column) {
        let editor = this.getEditor(id);
        return editor.getTopForPosition(lineNumber, column);
    },

    getLineHeightForPosition: function (id, position) {
        let editor = this.getEditor(id);
        return editor.getLineHeightForPosition(position);
    },

    writeScreenReaderContent: function (id, reason) {
        let editor = this.getEditor(id);
        editor.writeScreenReaderContent(reason);
    },

    getValue: function (id, preserveBOM, lineEnding) {
        let editor = this.getEditor(id);
        let options = null;
        if (preserveBOM != null && lineEnding != null) {
            options = {
                preserveBOM: preserveBOM,
                lineEnding: lineEnding
            };
        }
        return editor.getValue(options);
    },

    getVisibleColumnFromPosition: function (id, position) {
        let editor = this.getEditor(id);
        return editor.getVisibleColumnFromPosition(position);
    },

    getVisibleRanges: function (id) {
        let editor = this.getEditor(id);
        return editor.getVisibleRanges();
    },

    hasTextFocus: function (id) {
        let editor = this.getEditor(id);
        return editor.hasTextFocus();
    },

    hasWidgetFocus: function (id) {
        let editor = this.getEditor(id);
        return editor.hasWidgetFocus();
    },

    layout: function (id, dimension, postponeRendering) {
        let editor = this.getEditor(id);
        editor.layout(dimension, postponeRendering);
    },

    pushUndoStop: function (id) {
        let editor = this.getEditor(id);
        return editor.pushUndoStop();
    },

    popUndoStop: function (id) {
        let editor = this.getEditor(id);
        return editor.popUndoStop();
    },

    render: function (id, forceRedraw) {
        let editor = this.getEditor(id);
        editor.render(forceRedraw);
    },

    renderAsync: function (id, forceRedraw) {
        let editor = this.getEditor(id);
        editor.renderAsync(forceRedraw);
    },

    revealLine: function (id, lineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLine(lineNumber, scrollType);
    },

    revealLineInCenter: function (id, lineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLineInCenter(lineNumber, scrollType);
    },

    revealLineInCenterIfOutsideViewport: function (id, lineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLineInCenterIfOutsideViewport(lineNumber, scrollType);
    },

    revealLines: function (id, startLineNumber, endLineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLine(startLineNumber, endLineNumber, scrollType);
    },

    revealLinesInCenter: function (id, startLineNumber, endLineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLinesInCenter(startLineNumber, endLineNumber, scrollType);
    },

    revealLinesInCenterIfOutsideViewport: function (id, startLineNumber, endLineNumber, scrollType) {
        let editor = this.getEditor(id);
        editor.revealLinesInCenterIfOutsideViewport(startLineNumber, endLineNumber, scrollType);
    },

    revealPosition: function (id, position, scrollType) {
        let editor = this.getEditor(id);
        editor.revealPosition(position, scrollType);
    },

    revealPositionInCenter: function (id, position, scrollType) {
        let editor = this.getEditor(id);
        editor.revealPositionInCenter(position, scrollType);
    },

    revealPositionInCenterIfOutsideViewport: function (id, position, scrollType) {
        let editor = this.getEditor(id);
        editor.revealPositionInCenterIfOutsideViewport(position, scrollType);
    },

    revealRange: function (id, range, scrollType) {
        let editor = this.getEditor(id);
        editor.revealRange(range, scrollType);
    },

    revealRangeAtTop: function (id, range, scrollType) {
        let editor = this.getEditor(id);
        editor.revealRangeAtTop(range, scrollType);
    },

    revealRangeInCenter: function (id, range, scrollType) {
        let editor = this.getEditor(id);
        editor.revealRangeInCenter(range, scrollType);
    },

    revealRangeInCenterIfOutsideViewport: function (id, range, scrollType) {
        let editor = this.getEditor(id);
        editor.revealRangeInCenterIfOutsideViewport(range, scrollType);
    },

    revealAllCursors: function (id, revealHorizontal, minimalReveal) {
        let editor = this.getEditor(id);
        editor.revealAllCursors(revealHorizontal, minimalReveal);
    },

    setEventListener: function (id, eventName) {
        let editorHolder = this.getEditorHolder(id);
        let editor = editorHolder.editor;
        let dotnetRef = editorHolder.dotnetRef;

        let listener = function (e) {
            let eventJson = JSON.stringify(e);
            if (eventName === "OnDidChangeModel") {
                eventJson = JSON.stringify(blazorMonaco.editor.convertUriToString(e));
            }
            else if (eventName === "OnDidChangeConfiguration") {
                eventJson = JSON.stringify(e._values);
            }
            dotnetRef.invokeMethodAsync("EventCallback", eventName, eventJson);
        };

        switch (eventName) {
            case "OnDidCompositionEnd": editor.onDidCompositionEnd(listener); break;
            case "OnDidCompositionStart": editor.onDidCompositionStart(listener); break;
            case "OnContextMenu": editor.onContextMenu(listener); break;
            case "OnDidBlurEditorText": editor.onDidBlurEditorText(listener); break;
            case "OnDidBlurEditorWidget": editor.onDidBlurEditorWidget(listener); break;
            case "OnDidChangeConfiguration": editor.onDidChangeConfiguration(listener); break;
            case "OnDidChangeCursorPosition": editor.onDidChangeCursorPosition(listener); break;
            case "OnDidChangeCursorSelection": editor.onDidChangeCursorSelection(listener); break;
            case "OnWillChangeModel": editor.onWillChangeModel(listener); break;
            case "OnDidChangeModel": editor.onDidChangeModel(listener); break;
            case "OnDidChangeModelContent": editor.onDidChangeModelContent(listener); break;
            case "OnDidChangeModelDecorations": editor.onDidChangeModelDecorations(listener); break;
            case "OnDidChangeModelLanguage": editor.onDidChangeModelLanguage(listener); break;
            case "OnDidChangeModelLanguageConfiguration": editor.onDidChangeModelLanguageConfiguration(listener); break;
            case "OnDidChangeModelOptions": editor.onDidChangeModelOptions(listener); break;
            case "OnDidContentSizeChange": editor.onDidContentSizeChange(listener); break;
            case "OnDidDispose": editor.onDidDispose(listener); break;
            case "OnDidFocusEditorText": editor.onDidFocusEditorText(listener); break;
            case "OnDidFocusEditorWidget": editor.onDidFocusEditorWidget(listener); break;
            case "OnDidLayoutChange": editor.onDidLayoutChange(listener); break;
            case "OnDidPaste": editor.onDidPaste(listener); break;
            case "OnDidScrollChange": editor.onDidScrollChange(listener); break;
            case "OnKeyDown": editor.onKeyDown(listener); break;
            case "OnKeyUp": editor.onKeyUp(listener); break;
            case "OnMouseDown": editor.onMouseDown(listener); break;
            case "OnMouseLeave": editor.onMouseLeave(listener); break;
            case "OnMouseMove": editor.onMouseMove(listener); break;
            case "OnMouseUp": editor.onMouseUp(listener); break;
        }
    },

    setStatusEventListener: function (id, dotnetRef) {
        let editorHolder = this.getEditorHolder(id);
        if (editorHolder == null || editorHolder.editor == null)
            return;

        let editor = editorHolder.editor;
        // Diff editors expose the relevant surface through getModifiedEditor().
        let target = typeof editor.getModifiedEditor === "function" ? editor.getModifiedEditor() : editor;
        if (target == null)
            return;

        let getStatus = function () {
            let model = target.getModel();
            let position = target.getPosition() || { lineNumber: 1, column: 1 };
            let selections = target.getSelections() || [];
            let selectionLength = 0;
            if (model) {
                for (let i = 0; i < selections.length; i++)
                    selectionLength += model.getValueLengthInRange(selections[i]);
            }
            let options = model ? model.getOptions() : null;
            return {
                line: position.lineNumber,
                column: position.column,
                selectionLength: selectionLength,
                selections: selections.length,
                language: model ? model.getLanguageId() : "",
                lineCount: model ? model.getLineCount() : 0,
                eol: options && options.eol === "\r\n" ? "\r\n" : "\n",
                tabSize: options ? options.tabSize : 4,
                insertSpaces: options ? options.insertSpaces : true
            };
        };

        let notify = function () {
            dotnetRef.invokeMethodAsync("UpdateStatus", JSON.stringify(getStatus()));
        };

        target.onDidChangeCursorPosition(notify);
        target.onDidChangeCursorSelection(notify);
        target.onDidChangeModel(notify);
        target.onDidChangeModelContent(notify);
        target.onDidChangeModelLanguage(notify);
        target.onDidChangeModelOptions(notify);

        // Return the current snapshot as well. The Blazor component applies it immediately,
        // which avoids showing the default ("0 lines", "plaintext") state until the first event.
        return JSON.stringify(getStatus());
    },

    setInstanceModel: function (id, uriStr) {
        let model = this.model.getModel(uriStr);
        if (model == null)
            return;

        let editor = this.getEditor(id);
        editor.setModel(model);
    },

    setInstanceDiffModel: function (id, model) {
        let original_model = this.model.getModel(model.original.uri);
        let modified_model = this.model.getModel(model.modified.uri);
        if (original_model == null || modified_model == null)
            return;

        let editor = this.getEditor(id);
        editor.setModel({
            original: original_model,
            modified: modified_model,
        });
    },

    setPosition: function (id, position) {
        let editor = this.getEditor(id);
        editor.setPosition(position);
    },

    setScrollLeft: function (id, newScrollLeft, scrollType) {
        let editor = this.getEditor(id);
        editor.setScrollLeft(newScrollLeft, scrollType);
    },

    setScrollPosition: function (id, newPosition, scrollType) {
        let editor = this.getEditor(id);
        editor.setScrollPosition(newPosition, scrollType);
    },

    hasPendingScrollAnimation: function (id) {
        let editor = this.getEditor(id);
        return editor.hasPendingScrollAnimation();
    },

    setScrollTop: function (id, newScrollTop, scrollType) {
        let editor = this.getEditor(id);
        editor.setScrollTop(newScrollTop, scrollType);
    },

    setSelection: function (id, selection) {
        let editor = this.getEditor(id);
        editor.setSelection(selection);
    },

    setSelections: function (id, selection) {
        let editor = this.getEditor(id);
        editor.setSelections(selection);
    },

    setValue: function (id, value) {
        let editor = this.getEditor(id);
        editor.setValue(value);
    },

    trigger: function (id, source, handlerId, payload) {
        let editor = this.getEditor(id);
        editor.trigger(source, handlerId, payload);
    },

    updateOptions: function (id, options) {
        let editor = this.getEditor(id);
        editor.updateOptions(options);
    },

    uuidv4: function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            let r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    },

    //#endregion

    //#region TextModel methods

    model: {
        getModel: function (uriStr) {
            let uri = monaco.Uri.parse(uriStr);
            if (uri == null)
                return null;
            return monaco.editor.getModel(uri);
        },

        getOptions: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getOptions();
        },

        getVersionId: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getVersionId();
        },

        getAlternativeVersionId: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getAlternativeVersionId();
        },

        setValue: function (uriStr, newValue) {
            let model = this.getModel(uriStr);
            return model.setValue(newValue);
        },

        getValue: function (uriStr, eol, preserveBOM) {
            let model = this.getModel(uriStr);
            return model.getValue(eol, preserveBOM);
        },

        getValueLength: function (uriStr, eol, preserveBOM) {
            let model = this.getModel(uriStr);
            return model.getValueLength(eol, preserveBOM);
        },

        getValueInRange: function (uriStr, range, eol) {
            let model = this.getModel(uriStr);
            return model.getValueInRange(range, eol);
        },

        getValueLengthInRange: function (uriStr, range, eol) {
            let model = this.getModel(uriStr);
            return model.getValueLengthInRange(range, eol);
        },

        getCharacterCountInRange: function (uriStr, range, eol) {
            let model = this.getModel(uriStr);
            return model.getCharacterCountInRange(range, eol);
        },

        getLineCount: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getLineCount();
        },

        getLineContent: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineContent(lineNumber);
        },

        getLineLength: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineLength(lineNumber);
        },

        getLinesContent: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getLinesContent();
        },

        getEOL: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getEOL();
        },

        getEndOfLineSequence: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getEndOfLineSequence();
        },

        getLineMinColumn: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineMinColumn(lineNumber);
        },

        getLineMaxColumn: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineMaxColumn(lineNumber);
        },

        getLineFirstNonWhitespaceColumn: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineFirstNonWhitespaceColumn(lineNumber);
        },

        getLineLastNonWhitespaceColumn: function (uriStr, lineNumber) {
            let model = this.getModel(uriStr);
            return model.getLineLastNonWhitespaceColumn(lineNumber);
        },

        validatePosition: function (uriStr, position) {
            let model = this.getModel(uriStr);
            return model.validatePosition(position);
        },

        modifyPosition: function (uriStr, position, offset) {
            let model = this.getModel(uriStr);
            return model.modifyPosition(position, offset);
        },

        validateRange: function (uriStr, range) {
            let model = this.getModel(uriStr);
            return model.validateRange(range);
        },

        isValidRange: function (uriStr, range) {
            let model = this.getModel(uriStr);
            return model.isValidRange(range);
        },

        getOffsetAt: function (uriStr, position) {
            let model = this.getModel(uriStr);
            return model.getOffsetAt(position);
        },

        getPositionAt: function (uriStr, offset) {
            let model = this.getModel(uriStr);
            return model.getPositionAt(offset);
        },

        getFullModelRange: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getFullModelRange();
        },

        isDisposed: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.isDisposed();
        },

        findMatches: function (uriStr, searchString, searchScope_or_searchOnlyEditableRange, isRegex, matchCase, wordSeparators, captureMatches, limitResultCount) {
            let model = this.getModel(uriStr);
            return model.findMatches(searchString, searchScope_or_searchOnlyEditableRange, isRegex, matchCase, wordSeparators, captureMatches, limitResultCount);
        },

        findNextMatch: function (uriStr, searchString, searchStart, isRegex, matchCase, wordSeparators, captureMatches) {
            let model = this.getModel(uriStr);
            return model.findNextMatch(searchString, searchStart, isRegex, matchCase, wordSeparators, captureMatches);
        },

        findPreviousMatch: function (uriStr, searchString, searchStart, isRegex, matchCase, wordSeparators, captureMatches) {
            let model = this.getModel(uriStr);
            return model.findPreviousMatch(searchString, searchStart, isRegex, matchCase, wordSeparators, captureMatches);
        },

        getLanguageId: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.getLanguageId();
        },

        getWordAtPosition: function (uriStr, position) {
            let model = this.getModel(uriStr);
            return model.getWordAtPosition(position);
        },

        getWordUntilPosition: function (uriStr, position) {
            let model = this.getModel(uriStr);
            return model.getWordUntilPosition(position);
        },

        deltaDecorations: function (uriStr, oldDecorations, newDecorations, ownerId) {
            let model = this.getModel(uriStr);
            return model.deltaDecorations(oldDecorations, newDecorations, ownerId);
        },

        getDecorationOptions: function (uriStr, id) {
            let model = this.getModel(uriStr);
            return model.getDecorationOptions(id);
        },

        getDecorationRange: function (uriStr, id) {
            let model = this.getModel(uriStr);
            return model.getDecorationRange(id);
        },

        getLineDecorations: function (uriStr, lineNumber, ownerId, filterOutValidation, filterFontDecorations) {
            let model = this.getModel(uriStr);
            let result = model.getLineDecorations(lineNumber, ownerId, filterOutValidation, filterFontDecorations);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getLinesDecorations: function (uriStr, startLineNumber, endLineNumber, ownerId, filterOutValidation, filterFontDecorations) {
            let model = this.getModel(uriStr);
            let result = model.getLinesDecorations(startLineNumber, endLineNumber, ownerId, filterOutValidation, filterFontDecorations);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getDecorationsInRange: function (uriStr, range, ownerId, filterOutValidation, filterFontDecorations, onlyMinimapDecorations, onlyMarginDecorations) {
            let model = this.getModel(uriStr);
            let result = model.getDecorationsInRange(range, ownerId, filterOutValidation, filterFontDecorations, onlyMinimapDecorations, onlyMarginDecorations);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getAllDecorations: function (uriStr, ownerId, filterOutValidation, filterFontDecorations) {
            let model = this.getModel(uriStr);
            let result = model.getAllDecorations(ownerId, filterOutValidation, filterFontDecorations);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getAllMarginDecorations: function (uriStr, ownerId) {
            let model = this.getModel(uriStr);
            let result = model.getAllMarginDecorations(ownerId);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getOverviewRulerDecorations: function (uriStr, ownerId, filterOutValidation, filterFontDecorations) {
            let model = this.getModel(uriStr);
            let result = model.getOverviewRulerDecorations(ownerId, filterOutValidation, filterFontDecorations);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getInjectedTextDecorations: function (uriStr, ownerId) {
            let model = this.getModel(uriStr);
            let result = model.getInjectedTextDecorations(ownerId);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getCustomLineHeightsDecorations: function (uriStr, ownerId) {
            let model = this.getModel(uriStr);
            let result = model.getCustomLineHeightsDecorations(ownerId);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        getCustomLineHeightsDecorationsInRange: function (uriStr, range, ownerId) {
            let model = this.getModel(uriStr);
            let result = model.getCustomLineHeightsDecorationsInRange(range, ownerId);
            return blazorMonaco.editor.removeCircularReferences(result);
        },

        normalizeIndentation: function (uriStr, str) {
            let model = this.getModel(uriStr);
            return model.normalizeIndentation(str);
        },

        updateOptions: function (uriStr, newOpts) {
            let model = this.getModel(uriStr);
            return model.updateOptions(newOpts);
        },

        detectIndentation: function (uriStr, defaultInsertSpaces, defaultTabSize) {
            let model = this.getModel(uriStr);
            return model.detectIndentation(defaultInsertSpaces, defaultTabSize);
        },

        pushStackElement: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.pushStackElement();
        },

        popStackElement: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.popStackElement();
        },

        pushEOL: function (uriStr, eol) {
            let model = this.getModel(uriStr);
            return model.pushEOL(eol);
        },

        applyEdits: function (uriStr, operations, computeUndoEdits) {
            let model = this.getModel(uriStr);
            return model.applyEdits(operations, computeUndoEdits);
        },

        setEOL: function (uriStr, eol) {
            let model = this.getModel(uriStr);
            return model.setEOL(eol);
        },

        undo: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.undo();
        },

        canUndo: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.canUndo();
        },

        redo: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.redo();
        },

        canRedo: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.canRedo();
        },

        dispose: function (uriStr) {
            let model = this.getModel(uriStr);
            return model.dispose();
        },

        createSnapshot: function (uriStr, preserveBOM) {
            let model = this.getModel(uriStr);
            if (model == null)
                return "";
            let snapshot = model.createSnapshot(preserveBOM);
            let parts = [];
            let chunk;
            while ((chunk = snapshot.read()) !== null && chunk !== undefined) {
                parts.push(chunk);
            }
            return parts.join('');
        },

        pushEditOperations: function (uriStr, beforeCursorState, editOperations) {
            let model = this.getModel(uriStr);
            if (model == null)
                return null;
            let result = model.pushEditOperations(beforeCursorState, editOperations);
            return result;
        },

        isAttachedToEditor: function (uriStr) {
            let model = this.getModel(uriStr);
            if (model == null)
                return false;
            return model.isAttachedToEditor();
        }
    }

    //#endregion
}

window.blazorMonaco.languages = {
    //#region Static methods

    registerHoverProvider: async function (language, hoverProviderRef) {
        await monaco.languages.registerHoverProvider(language, {
            provideHover: (model, position, cancellationToken, context) => {
                return hoverProviderRef.invokeMethodAsync("ProvideHover", decodeURI(model.uri.toString()), position, context);
            }
        });
    },

    registerCodeActionProvider: async function (language, codeActionProviderRef, metadata) {
        await monaco.languages.registerCodeActionProvider(language, {
            provideCodeActions: (model, range, context, cancellationToken) => {
                return codeActionProviderRef.invokeMethodAsync("ProvideCodeActions", decodeURI(model.uri.toString()), range, context)
                    .then(result => {
                        (result.actions || []).forEach(action => {
                            (action.edit.edits || []).forEach(edit => {
                                if (edit.resource != null)
                                    edit.resource = monaco.Uri.parse(edit.resource)
                                if (edit.oldResource != null)
                                    edit.oldResource = monaco.Uri.parse(edit.oldResource)
                                if (edit.newResource != null)
                                    edit.newResource = monaco.Uri.parse(edit.newResource)
                            });
                        });
                        result.dispose = () => { };
                        return result;
                    });
            },
            resolveCodeAction: (codeAction, cancellationToken) => {
                return codeActionProviderRef.invokeMethodAsync("ResolveCodeAction", codeAction);
            }
        }, metadata);
    },

    registerDocumentFormattingEditProvider: async function (language, displayName, documentFormattingEditProviderRef) {
        await monaco.languages.registerDocumentFormattingEditProvider(language, {
            displayName: displayName,
            provideDocumentFormattingEdits: (model, options, cancellationToken) => {
                return documentFormattingEditProviderRef.invokeMethodAsync("ProvideDocumentFormattingEdits", decodeURI(model.uri.toString()), options);
            }
        });
    },

    registerCompletionItemProvider: async function (language, triggerCharacters, completionItemProviderRef) {
        await monaco.languages.registerCompletionItemProvider(language, {
            triggerCharacters: triggerCharacters,
            provideCompletionItems: (model, position, context, cancellationToken) => {
                return completionItemProviderRef.invokeMethodAsync("ProvideCompletionItems", decodeURI(model.uri.toString()), position, context);
            },
            resolveCompletionItem: (completionItem, cancellationToken) => {
                return completionItemProviderRef.invokeMethodAsync("ResolveCompletionItem", completionItem);
            }
        });
    },


    registerDefinitionProvider: async function (language, definitionProviderRef) {
        await monaco.languages.registerDefinitionProvider(language, {
            provideDefinitions: (model, position, token) => {
                return definitionProviderRef.invokeMethodAsync("ProvideDefinitions", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerReferenceProvider: async function (language, referenceProviderRef) {
        await monaco.languages.registerReferenceProvider(language, {
            provideReferences: (model, position, context, token) => {
                return referenceProviderRef.invokeMethodAsync("ProvideReferences", decodeURI(model.uri.toString()), position, context);
            }
        });
    },

    registerImplementationProvider: async function (language, implementationProviderRef) {
        await monaco.languages.registerImplementationProvider(language, {
            provideImplementation: (model, position, token) => {
                return implementationProviderRef.invokeMethodAsync("ProvideImplementations", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerTypeDefinitionProvider: async function (language, typeDefinitionProviderRef) {
        await monaco.languages.registerTypeDefinitionProvider(language, {
            provideTypeDefinition: (model, position, token) => {
                return typeDefinitionProviderRef.invokeMethodAsync("ProvideTypeDefinitions", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerDeclarationProvider: async function (language, declarationProviderRef) {
        await monaco.languages.registerDeclarationProvider(language, {
            provideDeclaration: (model, position, token) => {
                return declarationProviderRef.invokeMethodAsync("ProvideDeclarations", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerDocumentSymbolProvider: async function (language, documentSymbolProviderRef) {
        await monaco.languages.registerDocumentSymbolProvider(language, {
            provideDocumentSymbols: (model, token) => {
                return documentSymbolProviderRef.invokeMethodAsync("ProvideDocumentSymbols", decodeURI(model.uri.toString()));
            }
        });
    },

    registerFoldingRangeProvider: async function (language, foldingRangeProviderRef) {
        await monaco.languages.registerFoldingRangeProvider(language, {
            provideFoldingRanges: (model, context, token) => {
                return foldingRangeProviderRef.invokeMethodAsync("ProvideFoldingRanges", decodeURI(model.uri.toString()));
            }
        });
    },

    registerDocumentHighlightProvider: async function (language, documentHighlightProviderRef) {
        await monaco.languages.registerDocumentHighlightProvider(language, {
            provideDocumentHighlights: (model, position, token) => {
                return documentHighlightProviderRef.invokeMethodAsync("ProvideDocumentHighlights", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerColorProvider: async function (language, documentColorProviderRef) {
        await monaco.languages.registerColorProvider(language, {
            provideColors: (model, token) => {
                return documentColorProviderRef.invokeMethodAsync("ProvideColors", decodeURI(model.uri.toString()));
            },
            provideColorPresentations: (model, colorInfo, token) => {
                return documentColorProviderRef.invokeMethodAsync("ProvideColorPresentations", decodeURI(model.uri.toString()), colorInfo);
            }
        });
    },

    registerSelectionRangeProvider: async function (language, selectionRangeProviderRef) {
        await monaco.languages.registerSelectionRangeProvider(language, {
            provideSelectionRanges: (model, positions, token) => {
                return selectionRangeProviderRef.invokeMethodAsync("ProvideSelectionRanges", decodeURI(model.uri.toString()), positions);
            }
        });
    },

    registerSignatureHelpProvider: async function (language, signatureHelpProviderRef) {
        await monaco.languages.registerSignatureHelpProvider(language, {
            provideSignatureHelp: (model, position, token, context) => {
                return signatureHelpProviderRef.invokeMethodAsync("ProvideSignatureHelp", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerCodeLensProvider: async function (language, codeLensProviderRef) {
        await monaco.languages.registerCodeLensProvider(language, {
            provideCodeLenses: (model, token) => {
                return codeLensProviderRef.invokeMethodAsync("ProvideCodeLenses", decodeURI(model.uri.toString()));
            },
            resolveCodeLens: (model, codeLens, token) => {
                return codeLensProviderRef.invokeMethodAsync("ResolveCodeLens", decodeURI(model.uri.toString()), codeLens);
            }
        });
    },

    registerRenameProvider: async function (language, renameProviderRef) {
        await monaco.languages.registerRenameProvider(language, {
            provideRenameEdits: (model, position, newName, token) => {
                return renameProviderRef.invokeMethodAsync("ProvideRenameEdits", decodeURI(model.uri.toString()), position, newName);
            }
        });
    },

    registerDocumentSemanticTokensProvider: async function (language, documentSemanticTokensProviderRef, legend) {
        await monaco.languages.registerDocumentSemanticTokensProvider(language, {
            getLegend: () => legend,
            provideDocumentSemanticTokens: (model, lastResultId, token) => {
                return documentSemanticTokensProviderRef.invokeMethodAsync("ProvideDocumentSemanticTokens", decodeURI(model.uri.toString()), lastResultId);
            }
        });
    },

    registerDocumentRangeSemanticTokensProvider: async function (language, documentRangeSemanticTokensProviderRef, legend) {
        await monaco.languages.registerDocumentRangeSemanticTokensProvider(language, {
            getLegend: () => legend,
            provideDocumentRangeSemanticTokens: (model, range, token) => {
                return documentRangeSemanticTokensProviderRef.invokeMethodAsync("ProvideDocumentRangeSemanticTokens", decodeURI(model.uri.toString()), range);
            }
        });
    },

    registerInlayHintsProvider: async function (language, inlayHintsProviderRef) {
        await monaco.languages.registerInlayHintsProvider(language, {
            provideInlayHints: (model, range, token) => {
                return inlayHintsProviderRef.invokeMethodAsync("ProvideInlayHints", decodeURI(model.uri.toString()), range);
            }
        });
    },

    registerInlineCompletionsProvider: async function (language, inlineCompletionsProviderRef) {
        await monaco.languages.registerInlineCompletionsProvider(language, {
            provideInlineCompletions: (model, position, context, token) => {
                return inlineCompletionsProviderRef.invokeMethodAsync("ProvideInlineCompletions", decodeURI(model.uri.toString()), position);
            },
            resolveInlineCompletion: (model, inlineCompletion, token) => {
                return inlineCompletionsProviderRef.invokeMethodAsync("ResolveInlineCompletion", decodeURI(model.uri.toString()), inlineCompletion);
            }
        });
    },

    registerDocumentRangeFormattingEditProvider: async function (language, documentRangeFormattingEditProviderRef) {
        await monaco.languages.registerDocumentRangeFormattingEditProvider(language, {
            provideDocumentRangeFormattingEdits: (model, range, options, token) => {
                return documentRangeFormattingEditProviderRef.invokeMethodAsync("ProvideDocumentRangeFormattingEdits", decodeURI(model.uri.toString()), range, options);
            }
        });
    },

    registerOnTypeFormattingEditProvider: async function (language, onTypeFormattingEditProviderRef, triggerCharacters) {
        await monaco.languages.registerOnTypeFormattingEditProvider(language, {
            provideOnTypeFormattingEdits: (model, position, ch, options, token) => {
                return onTypeFormattingEditProviderRef.invokeMethodAsync("ProvideOnTypeFormattingEdits", decodeURI(model.uri.toString()), position, ch, options);
            }
        }, triggerCharacters || []);
    },

    registerLinkedEditingRangeProvider: async function (language, linkedEditingRangeProviderRef) {
        await monaco.languages.registerLinkedEditingRangeProvider(language, {
            provideLinkedEditingRanges: (model, position, token) => {
                return linkedEditingRangeProviderRef.invokeMethodAsync("ProvideLinkedEditingRanges", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerLinkProvider: async function (language, linkProviderRef) {
        await monaco.languages.registerLinkProvider(language, {
            provideLinks: (model, token) => {
                return linkProviderRef.invokeMethodAsync("ProvideLinks", decodeURI(model.uri.toString()));
            },
            resolveLink: (link, token) => {
                return linkProviderRef.invokeMethodAsync("ResolveLink", link);
            }
        });
    },

    registerNewSymbolNameProvider: async function (language, newSymbolNamesProviderRef) {
        await monaco.languages.registerNewSymbolNameProvider(language, {
            provideNewSymbolNames: (model, range, triggerKind, token) => {
                return newSymbolNamesProviderRef.invokeMethodAsync("ProvideNewSymbolNames", decodeURI(model.uri.toString()), range, triggerKind);
            },
            resolveNewSymbolName: (symbol, token) => {
                return newSymbolNamesProviderRef.invokeMethodAsync("ResolveNewSymbolName", symbol);
            }
        });
    },

    registerLanguage: async function (language) {
        await monaco.languages.register(language);
    },

    setLanguageConfiguration: async function (languageId, configuration) {
        await monaco.languages.setLanguageConfiguration(languageId, configuration);
    },

    setMonarchTokensProvider: async function (languageId, languageDef) {
        await monaco.languages.setMonarchTokensProvider(languageId, languageDef);
    },

    setColorMap: async function (colorMap) {
        await monaco.languages.setColorMap(colorMap);
    },

    onLanguage: function (languageId) {
        return monaco.languages.onLanguage(languageId, () => {
            return window.blazorMonaco.lifecycleRef ? window.blazorMonaco.lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnLanguage", JSON.stringify({ languageId: languageId })) : null;
        });
    },

    onLanguageEncountered: function (languageId) {
        return monaco.languages.onLanguageEncountered(languageId, () => {
            return window.blazorMonaco.lifecycleRef ? window.blazorMonaco.lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnLanguageEncountered", JSON.stringify({ languageId: languageId })) : null;
        });
    },

    getEncodedLanguageId: async function (languageId) {
        return monaco.languages.getEncodedLanguageId(languageId);
    },

    getLanguages: async function () {
        return monaco.languages.getLanguages();
    },

    score: async function (selector, candidateUri, candidateLanguage) {
        return monaco.languages.score(selector, monaco.Uri.parse(candidateUri), candidateLanguage);
    },

    setTokensProvider: async function (languageId, tokensProviderRef) {
        await monaco.languages.setTokensProvider(languageId, {
            getInitialState: () => tokensProviderRef.invokeMethodAsync("GetInitialState"),
            tokenize: (line, state) => tokensProviderRef.invokeMethodAsync("Tokenize", line, state)
        });
    },

    setEncodedTokensProvider: async function (languageId, tokensProviderRef) {
        await monaco.languages.setTokensProvider(languageId, {
            getInitialState: () => tokensProviderRef.invokeMethodAsync("GetInitialState"),
            tokenizeEncoded: (line, state) => tokensProviderRef.invokeMethodAsync("TokenizeEncoded", line, state)
        });
    },

    registerDefinitionProvider: async function (language, definitionProviderRef) {
        await monaco.languages.registerDefinitionProvider(language, {
            provideDefinitions: (model, position, token) => {
                return definitionProviderRef.invokeMethodAsync("ProvideDefinitions", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerReferenceProvider: async function (language, referenceProviderRef) {
        await monaco.languages.registerReferenceProvider(language, {
            provideReferences: (model, position, context, token) => {
                return referenceProviderRef.invokeMethodAsync("ProvideReferences", decodeURI(model.uri.toString()), position, context);
            }
        });
    },

    registerImplementationProvider: async function (language, implementationProviderRef) {
        await monaco.languages.registerImplementationProvider(language, {
            provideImplementation: (model, position, token) => {
                return implementationProviderRef.invokeMethodAsync("ProvideImplementations", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerTypeDefinitionProvider: async function (language, typeDefinitionProviderRef) {
        await monaco.languages.registerTypeDefinitionProvider(language, {
            provideTypeDefinition: (model, position, token) => {
                return typeDefinitionProviderRef.invokeMethodAsync("ProvideTypeDefinitions", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerDeclarationProvider: async function (language, declarationProviderRef) {
        await monaco.languages.registerDeclarationProvider(language, {
            provideDeclaration: (model, position, token) => {
                return declarationProviderRef.invokeMethodAsync("ProvideDeclarations", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerDocumentSymbolProvider: async function (language, documentSymbolProviderRef) {
        await monaco.languages.registerDocumentSymbolProvider(language, {
            provideDocumentSymbols: (model, token) => {
                return documentSymbolProviderRef.invokeMethodAsync("ProvideDocumentSymbols", decodeURI(model.uri.toString()));
            }
        });
    },

    registerFoldingRangeProvider: async function (language, foldingRangeProviderRef) {
        await monaco.languages.registerFoldingRangeProvider(language, {
            provideFoldingRanges: (model, context, token) => {
                return foldingRangeProviderRef.invokeMethodAsync("ProvideFoldingRanges", decodeURI(model.uri.toString()));
            }
        });
    },

    registerDocumentHighlightProvider: async function (language, documentHighlightProviderRef) {
        await monaco.languages.registerDocumentHighlightProvider(language, {
            provideDocumentHighlights: (model, position, token) => {
                return documentHighlightProviderRef.invokeMethodAsync("ProvideDocumentHighlights", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerColorProvider: async function (language, documentColorProviderRef) {
        await monaco.languages.registerColorProvider(language, {
            provideColors: (model, token) => {
                return documentColorProviderRef.invokeMethodAsync("ProvideColors", decodeURI(model.uri.toString()));
            },
            provideColorPresentations: (model, colorInfo, token) => {
                return documentColorProviderRef.invokeMethodAsync("ProvideColorPresentations", decodeURI(model.uri.toString()), colorInfo);
            }
        });
    },

    registerSelectionRangeProvider: async function (language, selectionRangeProviderRef) {
        await monaco.languages.registerSelectionRangeProvider(language, {
            provideSelectionRanges: (model, positions, token) => {
                return selectionRangeProviderRef.invokeMethodAsync("ProvideSelectionRanges", decodeURI(model.uri.toString()), positions);
            }
        });
    },

    registerSignatureHelpProvider: async function (language, signatureHelpProviderRef) {
        await monaco.languages.registerSignatureHelpProvider(language, {
            provideSignatureHelp: (model, position, token, context) => {
                return signatureHelpProviderRef.invokeMethodAsync("ProvideSignatureHelp", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerCodeLensProvider: async function (language, codeLensProviderRef) {
        await monaco.languages.registerCodeLensProvider(language, {
            provideCodeLenses: (model, token) => {
                return codeLensProviderRef.invokeMethodAsync("ProvideCodeLenses", decodeURI(model.uri.toString()));
            },
            resolveCodeLens: (model, codeLens, token) => {
                return codeLensProviderRef.invokeMethodAsync("ResolveCodeLens", decodeURI(model.uri.toString()), codeLens);
            }
        });
    },

    registerRenameProvider: async function (language, renameProviderRef) {
        await monaco.languages.registerRenameProvider(language, {
            provideRenameEdits: (model, position, newName, token) => {
                return renameProviderRef.invokeMethodAsync("ProvideRenameEdits", decodeURI(model.uri.toString()), position, newName);
            }
        });
    },

    registerDocumentSemanticTokensProvider: async function (language, documentSemanticTokensProviderRef, legend) {
        await monaco.languages.registerDocumentSemanticTokensProvider(language, {
            getLegend: () => legend,
            provideDocumentSemanticTokens: (model, lastResultId, token) => {
                return documentSemanticTokensProviderRef.invokeMethodAsync("ProvideDocumentSemanticTokens", decodeURI(model.uri.toString()), lastResultId);
            }
        });
    },

    registerDocumentRangeSemanticTokensProvider: async function (language, documentRangeSemanticTokensProviderRef, legend) {
        await monaco.languages.registerDocumentRangeSemanticTokensProvider(language, {
            getLegend: () => legend,
            provideDocumentRangeSemanticTokens: (model, range, token) => {
                return documentRangeSemanticTokensProviderRef.invokeMethodAsync("ProvideDocumentRangeSemanticTokens", decodeURI(model.uri.toString()), range);
            }
        });
    },

    registerInlayHintsProvider: async function (language, inlayHintsProviderRef) {
        await monaco.languages.registerInlayHintsProvider(language, {
            provideInlayHints: (model, range, token) => {
                return inlayHintsProviderRef.invokeMethodAsync("ProvideInlayHints", decodeURI(model.uri.toString()), range);
            }
        });
    },

    registerInlineCompletionsProvider: async function (language, inlineCompletionsProviderRef) {
        await monaco.languages.registerInlineCompletionsProvider(language, {
            provideInlineCompletions: (model, position, context, token) => {
                return inlineCompletionsProviderRef.invokeMethodAsync("ProvideInlineCompletions", decodeURI(model.uri.toString()), position);
            },
            resolveInlineCompletion: (model, inlineCompletion, token) => {
                return inlineCompletionsProviderRef.invokeMethodAsync("ResolveInlineCompletion", decodeURI(model.uri.toString()), inlineCompletion);
            }
        });
    },

    registerDocumentRangeFormattingEditProvider: async function (language, documentRangeFormattingEditProviderRef) {
        await monaco.languages.registerDocumentRangeFormattingEditProvider(language, {
            provideDocumentRangeFormattingEdits: (model, range, options, token) => {
                return documentRangeFormattingEditProviderRef.invokeMethodAsync("ProvideDocumentRangeFormattingEdits", decodeURI(model.uri.toString()), range, options);
            }
        });
    },

    registerOnTypeFormattingEditProvider: async function (language, onTypeFormattingEditProviderRef, triggerCharacters) {
        await monaco.languages.registerOnTypeFormattingEditProvider(language, {
            provideOnTypeFormattingEdits: (model, position, ch, options, token) => {
                return onTypeFormattingEditProviderRef.invokeMethodAsync("ProvideOnTypeFormattingEdits", decodeURI(model.uri.toString()), position, ch, options);
            }
        }, triggerCharacters || []);
    },

    registerLinkedEditingRangeProvider: async function (language, linkedEditingRangeProviderRef) {
        await monaco.languages.registerLinkedEditingRangeProvider(language, {
            provideLinkedEditingRanges: (model, position, token) => {
                return linkedEditingRangeProviderRef.invokeMethodAsync("ProvideLinkedEditingRanges", decodeURI(model.uri.toString()), position);
            }
        });
    },

    registerLinkProvider: async function (language, linkProviderRef) {
        await monaco.languages.registerLinkProvider(language, {
            provideLinks: (model, token) => {
                return linkProviderRef.invokeMethodAsync("ProvideLinks", decodeURI(model.uri.toString()));
            },
            resolveLink: (link, token) => {
                return linkProviderRef.invokeMethodAsync("ResolveLink", link);
            }
        });
    },

    registerNewSymbolNameProvider: async function (language, newSymbolNamesProviderRef) {
        await monaco.languages.registerNewSymbolNameProvider(language, {
            provideNewSymbolNames: (model, range, triggerKind, token) => {
                return newSymbolNamesProviderRef.invokeMethodAsync("ProvideNewSymbolNames", decodeURI(model.uri.toString()), range, triggerKind);
            },
            resolveNewSymbolName: (symbol, token) => {
                return newSymbolNamesProviderRef.invokeMethodAsync("ResolveNewSymbolName", symbol);
            }
        });
    },

    registerLanguage: async function (language) {
        await monaco.languages.register(language);
    },

    setLanguageConfiguration: async function (languageId, configuration) {
        await monaco.languages.setLanguageConfiguration(languageId, configuration);
    },

    setMonarchTokensProvider: async function (languageId, languageDef) {
        await monaco.languages.setMonarchTokensProvider(languageId, languageDef);
    },

    setColorMap: async function (colorMap) {
        await monaco.languages.setColorMap(colorMap);
    },

    onLanguage: function (languageId) {
        return monaco.languages.onLanguage(languageId, () => {
            return window.blazorMonaco.lifecycleRef ? window.blazorMonaco.lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnLanguage", JSON.stringify({ languageId: languageId })) : null;
        });
    },

    onLanguageEncountered: function (languageId) {
        return monaco.languages.onLanguageEncountered(languageId, () => {
            return window.blazorMonaco.lifecycleRef ? window.blazorMonaco.lifecycleRef.invokeMethodAsync("OnLifecycleEvent", "OnLanguageEncountered", JSON.stringify({ languageId: languageId })) : null;
        });
    },

    getEncodedLanguageId: async function (languageId) {
        return monaco.languages.getEncodedLanguageId(languageId);
    },

    getLanguages: async function () {
        return monaco.languages.getLanguages();
    },

    score: async function (selector, candidateUri, candidateLanguage) {
        return monaco.languages.score(selector, monaco.Uri.parse(candidateUri), candidateLanguage);
    }

    //#endregion
}

window.blazorMonaco.worker = {
    dispose: function (id) {
        let worker = window.blazorMonaco.workers[id];
        if (!worker) return;
        // Remove the entry first so a failed dispose cannot leave a stale worker
        // in the registry (and repeated calls stay no-ops).
        delete window.blazorMonaco.workers[id];
        worker.dispose();
    },
    getProxy: async function (id) {
        let worker = window.blazorMonaco.workers[id];
        if (!worker) return null;
        let proxy = await worker.getProxy();
        return blazorMonaco.editor.removeCircularReferences(proxy);
    },
    withSyncedResources: async function (id, resources) {
        let worker = window.blazorMonaco.workers[id];
        if (!worker) return null;
        await worker.withSyncedResources((resources || []).map(u => monaco.Uri.parse(u)));
        return null;
    },

    invoke: async function (id, method, args) {
        let worker = window.blazorMonaco.workers[id];
        if (!worker)
            throw new Error("BlazorMonaco: worker not found (it may have been disposed): " + id);
        let proxy = await worker.getProxy();
        if (proxy == null || typeof proxy[method] !== "function")
            throw new Error("BlazorMonaco: worker proxy method not found: " + method);
        return await proxy[method].apply(proxy, args || []);
    }
};

window.blazorMonaco.localization = {
    getReference: function () {
        return window.blazorMonaco.localizationRef;
    },
    setReference: function (ref) {
        window.blazorMonaco.localizationRef = ref;
    },
    getString: async function (key) {
        if (window.blazorMonaco.localizationRef) {
            return await window.blazorMonaco.localizationRef.invokeMethodAsync("GetLocalizedString", key);
        }
        return null;
    },
    getAllStrings: async function () {
        if (window.blazorMonaco.localizationRef) {
            return await window.blazorMonaco.localizationRef.invokeMethodAsync("GetAllStrings");
        }
        return {};
    }
};
