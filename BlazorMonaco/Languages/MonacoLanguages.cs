using BlazorMonaco.Helpers;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMonaco.Languages
{
    /**
     * Built-in language identifiers available in monaco-editor v0.57.0.
     * Use these constants instead of magic strings when registering providers or setting model language.
     */
    public static class MonacoLanguages
    {
        #region LanguageIds

        public const string Abap = "abap";
        public const string Actionscript3 = "actionscript3";
        public const string Ada = "ada";
        public const string Apex = "apex";
        public const string Applescript = "applescript";
        public const string Azcli = "azcli";
        public const string Bat = "bat";
        public const string Biblicalhebrew = "biblicalhebrew";//?
        public const string Blazor = "razor";
        public const string C = "c";
        public const string Clojure = "clojure";
        public const string Cameligo = "cameligo";
        public const string Coffeescript = "coffeescript";
        public const string Cpp = "cpp";
        public const string Csharp = "csharp";
        public const string Css = "css";
        public const string Csp = "csp";
        public const string Cypher = "cypher";
        public const string Cuda = "cuda";
        public const string Dart = "dart";
        public const string Dockerfile = "dockerfile";
        public const string Elixir = "elixir";
        public const string Flow9 = "flow9";
        public const string Erlang = "erlang";
        public const string Fsharp = "fsharp";
        public const string Go = "go";
        public const string Graphql = "graphql";
        public const string Groovy = "groovy";
        public const string Handlebars = "handlebars";
        public const string Hcl = "hcl";
        public const string Html = "html";
        public const string Ini = "ini";
        public const string Java = "java";
        public const string Javascript = "javascript";
        public const string Json = "json";
        public const string Jsonc = "jsonc";
        public const string Julia = "julia";
        public const string Kotlin = "kotlin";
        public const string Less = "less";
        public const string Lua = "lua";
        public const string Liquid = "liquid";
        public const string Mysql = "mysql";
        public const string Mdx = "mdx";
        public const string Mips = "mips";
        public const string M3 = "m3";
        public const string Msdax = "msdax";
        public const string Makefile = "makefile";
        public const string Markdown = "markdown";
        public const string Matlab = "matlab";
        public const string ObjectiveC = "objective-c";
        public const string Protobuf = "protobuf";
        public const string Pascal = "pascal";
        public const string Pascaligo = "pascaligo";
        public const string Perl = "perl";
        public const string Php = "php";
        public const string Powershell = "powershell";
        public const string Pgsql = "pgsql";
        public const string Pug = "pug";
        public const string Python = "python";
        public const string Pla = "pla";
        public const string R = "r";
        public const string Redis = "redis";
        public const string Redshift = "redshift";
        public const string Razor = "razor";
        public const string Ruby = "ruby";
        public const string Rust = "rust";
        public const string Solidity = "solidity";
        public const string Scala = "scala";
        public const string Scss = "scss";
        public const string Shell = "shell";
        public const string Sparql = "sparql";
        public const string Sql = "sql";
        public const string Swift = "swift";
        public const string Typescript = "typescript";
        public const string Typespec = "typespec";
        public const string Twig = "twig";
        public const string Vb = "vb";
        public const string Wgsl = "wgsl";
        public const string Xml = "xml";
        public const string Yaml = "yaml";

        #endregion

        /**
         * Register a hover provider (used by e.g. editor hover).
         */

        #region  Providers


        public static Task RegisterHoverProviderAsync(IJSRuntime jsRuntime, LanguageSelector language, HoverProvider.ProvideDelegate provideHover)
            => RegisterHoverProviderAsync(jsRuntime, language, new HoverProvider(provideHover));

        public static Task RegisterHoverProviderAsync(IJSRuntime jsRuntime, LanguageSelector language, HoverProvider hoverProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerHoverProvider", language, DotNetObjectReference.Create(hoverProvider));

        /**
         * Register a code action provider (used by e.g. quick fix).
         */
        public static Task RegisterCodeActionProvider(IJSRuntime jsRuntime, LanguageSelector language, CodeActionProvider codeActionProvider, CodeActionProviderMetadata metadata = null)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerCodeActionProvider", language, DotNetObjectReference.Create(codeActionProvider), metadata);

        /**
         * Register a formatter that can handle only entire models.
         */
        public static Task RegisterDocumentFormattingEditProvider(IJSRuntime jsRuntime, LanguageSelector language, DocumentFormattingEditProvider documentFormattingEditProvider)
            => JsRuntimeExt.UpdateRuntime(jsRuntime).SafeInvokeAsync("blazorMonaco.languages.registerDocumentFormattingEditProvider", language, documentFormattingEditProvider.DisplayName, DotNetObjectReference.Create(documentFormattingEditProvider));

        /**
         * Register a completion item provider (use by e.g. suggestions).
         */
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
         * Register an inline completion's provider.
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


        #endregion
    }
}
