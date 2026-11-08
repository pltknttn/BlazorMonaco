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
}
