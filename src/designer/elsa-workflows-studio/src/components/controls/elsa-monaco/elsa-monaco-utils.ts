import { Mutex } from "async-mutex";
import axios from 'axios';

const win = window as any;
const require = win.require;

export interface Monaco {
  editor: any;
  languages: any;
  KeyCode: any;
  Uri: any;
  theme: any;
}

export interface EditorVariable {
  variableName: string;
  type: string;
}

export var EditorVariables: Array<EditorVariable> = [];

let isInitialized: boolean;
const mutex = new Mutex();

export async function initializeMonacoWorker(libPath?: string): Promise<Monaco> {

  return await mutex.runExclusive(async () => {

    if (isInitialized) {

      return win.monaco;
    }

    const origin = document.location.origin;
    const baseUrl = libPath.startsWith('http') ? libPath : `${origin}/${libPath}`;

    require.config({ paths: { 'vs': `${baseUrl}/vs` } });
    win.MonacoEnvironment = { getWorkerUrl: () => proxy };

    let proxy = URL.createObjectURL(new Blob([`
	self.MonacoEnvironment = {
		baseUrl: '${baseUrl}'
	};
	importScripts('${baseUrl}/vs/base/worker/workerMain.js');
`], { type: 'text/javascript' }));

    return new Promise(resolve => {
      require(["vs/editor/editor.main"], () => {
        isInitialized = true;
        registerLiquid(win.monaco);
        registerSql(win.monaco);
        resolve(win.monaco);
        registerCsharpProvider(win.monaco);
      });
    });
  });


}

function registerLiquid(monaco: any) {
  monaco.languages.register({ id: 'liquid' });

  monaco.languages.registerCompletionItemProvider('liquid', {
    provideCompletionItems: () => {
      const autocompleteProviderItems = [];
      const keywords = ['assign', 'capture', 'endcapture', 'increment', 'decrement',
        'if', 'else', 'elsif', 'endif', 'for', 'endfor', 'break',
        'continue', 'limit', 'offset', 'range', 'reversed', 'cols',
        'case', 'endcase', 'when', 'block', 'endblock', 'true', 'false',
        'in', 'unless', 'endunless', 'cycle', 'tablerow', 'endtablerow',
        'contains', 'startswith', 'endswith', 'comment', 'endcomment',
        'raw', 'endraw', 'editable', 'endentitylist', 'endentityview', 'endinclude',
        'endmarker', 'entitylist', 'entityview', 'forloop', 'image', 'include',
        'marker', 'outputcache', 'plugin', 'style', 'text', 'widget',
        'abs', 'append', 'at_least', 'at_most', 'capitalize', 'ceil', 'compact',
        'concat', 'date', 'default', 'divided_by', 'downcase', 'escape',
        'escape_once', 'first', 'floor', 'join', 'last', 'lstrip', 'map',
        'minus', 'modulo', 'newline_to_br', 'plus', 'prepend', 'remove',
        'remove_first', 'replace', 'replace_first', 'reverse', 'round',
        'rstrip', 'size', 'slice', 'sort', 'sort_natural', 'split', 'strip',
        'strip_html', 'strip_newlines', 'times', 'truncate', 'truncatewords',
        'uniq', 'upcase', 'url_decode', 'url_encode'];

      for (let i = 0; i < keywords.length; i++) {
        autocompleteProviderItems.push({ 'label': keywords[i], kind: monaco.languages.CompletionItemKind.Keyword });
      }

      return { suggestions: autocompleteProviderItems };
    }
  });
}

function registerSql(monaco: any) {

  monaco.languages.registerCompletionItemProvider('sql', {
    triggerCharacters: ["@"],
    provideCompletionItems: (model, position) => {

      const word = model.getWordUntilPosition(position)

      const autocompleteProviderItems = [];
      for (const varible of EditorVariables) {
        autocompleteProviderItems.push({
          label: `${varible.variableName}: ${varible.type}`,
          kind: monaco.languages.CompletionItemKind.Variable,
          insertText: varible.variableName
        });
      }

      return { suggestions: autocompleteProviderItems };
    }
  });

}

async function sendRequest(type, request) {
  let endPoint:string = "http://localhost:5280";
  switch (type) {
    case 'complete': endPoint += '/completion/complete'; break;
    case 'signature': endPoint += '/completion/signature'; break;
    case 'hover': endPoint += '/completion/hover'; break;
    case 'codeCheck': endPoint += '/completion/codeCheck'; break;
  }
  return await axios.post(endPoint, JSON.stringify(request))
}

function registerCsharpProvider(monaco: any) {

  var assemblies = ['.\\bin\\Debug\\net8.0\\System.Text.Json.dll'];

  // Register CompletionItemProvider
  monaco.languages.registerCompletionItemProvider('csharp', {
    triggerCharacters: [".", " "],
    provideCompletionItems: async (model, position) => {
      // Check if the model language is 'csharp'
      if (model.getLanguageId() !== 'csharp') {
        return { suggestions: [] }; // Return empty suggestions if not csharp
      }

      let suggestions = [];

      let request = {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: assemblies
      };

      let resultQ = await sendRequest("complete", request);

      for (let elem of resultQ.data) {
        suggestions.push({
          label: {
            label: elem.Suggestion,
            description: elem.Description
          },
          kind: monaco.languages.CompletionItemKind.Function,
          insertText: elem.Suggestion
        });
      }

      return { suggestions: suggestions };
    }
  });

  // Register SignatureHelpProvider
  monaco.languages.registerSignatureHelpProvider('csharp', {
    signatureHelpTriggerCharacters: ["("],
    signatureHelpRetriggerCharacters: [","],

    provideSignatureHelp: async (model, position, token, context) => {
      // Check if the model language is 'csharp'
      if (model.getLanguageId() !== 'csharp') {
        return null;
      }

      let request = {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: assemblies
      };

      let resultQ = await sendRequest("signature", request);
      if (!resultQ.data) return;

      let signatures = [];
      for (let signature of resultQ.data.Signatures) {
        let params = [];
        for (let param of signature.Parameters) {
          params.push({
            label: param.Label,
            documentation: param.Documentation ?? ""
          });
        }

        signatures.push({
          label: signature.Label,
          documentation: signature.Documentation ?? "",
          parameters: params,
        });
      }

      let signatureHelp: any = {};
      signatureHelp.signatures = signatures;
      signatureHelp.activeParameter = resultQ.data.ActiveParameter;
      signatureHelp.activeSignature = resultQ.data.ActiveSignature;

      return {
        value: signatureHelp,
        dispose: () => { }
      };
    }
  });

  // Register HoverProvider
  monaco.languages.registerHoverProvider('csharp', {
    provideHover: async function (model, position) {
      // Check if the model language is 'csharp'
      if (model.getLanguageId() !== 'csharp') {
        return null;
      }

      let request = {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: assemblies
      };

      let resultQ = await sendRequest("hover", request);

      if (resultQ.data) {
        let posStart = model.getPositionAt(resultQ.data.OffsetFrom);
        let posEnd = model.getPositionAt(resultQ.data.OffsetTo);

        return {
          range: new monaco.Range(posStart.lineNumber, posStart.column, posEnd.lineNumber, posEnd.column),
          contents: [
            { value: resultQ.data.Information }
          ]
        };
      }

      return null;
    }
  });

  // Add validation logic to specific models with language 'csharp'
  monaco.editor.onDidCreateModel(function (model) {
    if (model.getLanguageId() !== 'csharp') {
      return; // Skip models that are not 'csharp'
    }

    async function validate() {
      let request = {
        Code: model.getValue(),
        Assemblies: assemblies
      };

      let resultQ = await sendRequest("codeCheck", request);

      let markers = [];

      for (let elem of resultQ.data) {
        let posStart = model.getPositionAt(elem.OffsetFrom);
        let posEnd = model.getPositionAt(elem.OffsetTo);
        markers.push({
          severity: elem.Severity,
          startLineNumber: posStart.lineNumber,
          startColumn: posStart.column,
          endLineNumber: posEnd.lineNumber,
          endColumn: posEnd.column,
          message: elem.Message,
          code: elem.Id
        });
      }

      monaco.editor.setModelMarkers(model, 'csharp', markers);
    }

    var handle = null;
    model.onDidChangeContent(() => {
      monaco.editor.setModelMarkers(model, 'csharp', []);
      clearTimeout(handle);
      handle = setTimeout(() => validate(), 500);
    });
    validate();
  });
}
