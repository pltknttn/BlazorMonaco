// Monaco custom web worker that is driven by BlazorMonaco's C# WorkerHost.
//
// The worker is created from C# via `Global.CreateWebWorker`. Monaco's AMD
// bootstrap (vs/editor/editor.worker) provides the message protocol and calls
// our factory with an IWorkerContext. Every `context.host.method(...)` call is
// forwarded to the C# `WorkerHost` instance on the main thread.
(function () {
    'use strict';

    // Messages can arrive before the AMD modules below finish loading. Monaco
    // sends an initialization message immediately, so queue everything until
    // the worker bootstrap installs its own onmessage handler.
    var pendingMessages = [];
    self.onmessage = function (event) {
        pendingMessages.push(event);
    };

    // The Monaco distribution shipped by BlazorMonaco. Adjust this path if the
    // application hosts the library somewhere else.
    var monacoBaseUrl = new URL(
        '_content/BlazorMonaco/lib/monaco-editor/min/vs',
        self.location.origin).href;

    importScripts(monacoBaseUrl + '/loader.js');
    require.config({ paths: { vs: monacoBaseUrl } });

    require(['vs/editor/editor.worker'], function (workerModule) {
        workerModule.initialize(function (context) {
            return new EchoWorker(context);
        });

        // `initialize` has replaced self.onmessage; replay what was queued.
        var queued = pendingMessages;
        pendingMessages = null;
        for (var i = 0; i < queued.length; i++)
            self.onmessage(queued[i]);
    });

    class EchoWorker {
        constructor(context) {
            this._context = context;
        }

        echo(text) {
            return 'Echo: ' + text;
        }

        uppercase(text) {
            return String(text).toUpperCase();
        }

        sum(values) {
            var items = Array.isArray(values) ? values : Array.prototype.slice.call(arguments);
            return items.reduce(function (total, value) {
                return total + Number(value);
            }, 0);
        }

        // Calls back into C# through `context.host` (WorkerHost.HandleAsync).
        async hostInfo() {
            return await this._context.host.getHostInfo();
        }

        // Reads the text models that Monaco synchronizes with the worker.
        modelInfo() {
            return this._context.getMirrorModels().map(function (model) {
                return {
                    uri: String(model.uri),
                    version: model.version,
                    length: model.getValue().length
                };
            });
        }
    }
})();
