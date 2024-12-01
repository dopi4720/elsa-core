import { Mutex } from 'async-mutex';
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

    require.config({ paths: { vs: `${baseUrl}/vs` } });
    win.MonacoEnvironment = { getWorkerUrl: () => proxy };

    let proxy = URL.createObjectURL(
      new Blob(
        [
          `
	self.MonacoEnvironment = {
		baseUrl: '${baseUrl}'
	};
	importScripts('${baseUrl}/vs/base/worker/workerMain.js');
`,
        ],
        { type: 'text/javascript' },
      ),
    );

    return new Promise(resolve => {
      require(['vs/editor/editor.main'], () => {
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
      const keywords = [
        'assign',
        'capture',
        'endcapture',
        'increment',
        'decrement',
        'if',
        'else',
        'elsif',
        'endif',
        'for',
        'endfor',
        'break',
        'continue',
        'limit',
        'offset',
        'range',
        'reversed',
        'cols',
        'case',
        'endcase',
        'when',
        'block',
        'endblock',
        'true',
        'false',
        'in',
        'unless',
        'endunless',
        'cycle',
        'tablerow',
        'endtablerow',
        'contains',
        'startswith',
        'endswith',
        'comment',
        'endcomment',
        'raw',
        'endraw',
        'editable',
        'endentitylist',
        'endentityview',
        'endinclude',
        'endmarker',
        'entitylist',
        'entityview',
        'forloop',
        'image',
        'include',
        'marker',
        'outputcache',
        'plugin',
        'style',
        'text',
        'widget',
        'abs',
        'append',
        'at_least',
        'at_most',
        'capitalize',
        'ceil',
        'compact',
        'concat',
        'date',
        'default',
        'divided_by',
        'downcase',
        'escape',
        'escape_once',
        'first',
        'floor',
        'join',
        'last',
        'lstrip',
        'map',
        'minus',
        'modulo',
        'newline_to_br',
        'plus',
        'prepend',
        'remove',
        'remove_first',
        'replace',
        'replace_first',
        'reverse',
        'round',
        'rstrip',
        'size',
        'slice',
        'sort',
        'sort_natural',
        'split',
        'strip',
        'strip_html',
        'strip_newlines',
        'times',
        'truncate',
        'truncatewords',
        'uniq',
        'upcase',
        'url_decode',
        'url_encode',
      ];

      for (let i = 0; i < keywords.length; i++) {
        autocompleteProviderItems.push({ label: keywords[i], kind: monaco.languages.CompletionItemKind.Keyword });
      }

      return { suggestions: autocompleteProviderItems };
    },
  });
}

function registerSql(monaco: any) {
  monaco.languages.registerCompletionItemProvider('sql', {
    triggerCharacters: ['@'],
    provideCompletionItems: (model, position) => {
      const word = model.getWordUntilPosition(position);

      const autocompleteProviderItems = [];
      for (const varible of EditorVariables) {
        autocompleteProviderItems.push({
          label: `${varible.variableName}: ${varible.type}`,
          kind: monaco.languages.CompletionItemKind.Variable,
          insertText: varible.variableName,
        });
      }

      return { suggestions: autocompleteProviderItems };
    },
  });
}

async function sendRequest(type, request) {
  // let endPoint: string = window.location.origin;
  let endPoint = 'https://localhost:11000'; // URL gốc

  // Xác định endpoint dựa trên loại request
  switch (type) {
    case 'complete':
      endPoint += '/v1/code-analysis/complete';
      break;
    case 'signature':
      endPoint += '/v1/code-analysis/signature';
      break;
    case 'hover':
      endPoint += '/v1/code-analysis/hover';
      break;
    case 'codeCheck':
      endPoint += '/v1/code-analysis/codeCheck';
      break;
    default:
      throw new Error('Invalid request type');
  }

  // Gửi request với axios
  try {
    const response = await axios.post(endPoint, request, {
      headers: {
        'Content-Type': 'application/json', // Đảm bảo gửi request dưới dạng JSON
      },
    });
    return response; // Trả về dữ liệu từ response
  } catch (error) {
    console.error('Request failed:', error.response ? error.response.data : error.message);
    throw error; // Ném lỗi ra ngoài nếu cần xử lý
  }
}

function registerCsharpProvider(monaco: any) {
  // Register CompletionItemProvider
  monaco.languages.registerCompletionItemProvider('csharp', {
    triggerCharacters: ['.', ' '],
    provideCompletionItems: async (model, position) => {
      // Check if the model language is 'csharp'
      if (model.getLanguageId() !== 'csharp') {
        return { suggestions: [] }; // Return empty suggestions if not csharp
      }

      let suggestions = [];

      let request = {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: [],
      };

      let resultQ = await sendRequest('complete', request);

      for (let elem of resultQ.data) {
        suggestions.push({
          label: {
            label: elem.suggestion,
            description: elem.description,
          },
          kind: monaco.languages.CompletionItemKind.Function,
          insertText: elem.suggestion,
        });
      }

      return { suggestions: suggestions };
    },
  });

  // Register SignatureHelpProvider
  monaco.languages.registerSignatureHelpProvider('csharp', {
    signatureHelpTriggerCharacters: ['('],
    signatureHelpRetriggerCharacters: [','],

    provideSignatureHelp: async (model, position, token, context) => {
      // Check if the model language is 'csharp'
      if (model.getLanguageId() !== 'csharp') {
        return null;
      }

      let request = {
        Code: model.getValue(),
        Position: model.getOffsetAt(position),
        Assemblies: [],
      };

      let resultQ = await sendRequest('signature', request);
      if (!resultQ.data) return;

      let signatures = [];
      for (let signature of resultQ.data.signatures) {
        let params = [];
        for (let param of signature.parameters) {
          params.push({
            label: param.label,
            documentation: param.documentation ?? '',
          });
        }

        signatures.push({
          label: signature.label,
          documentation: signature.documentation ?? '',
          parameters: params,
        });
      }

      let signatureHelp: any = {};
      signatureHelp.signatures = signatures;
      signatureHelp.activeParameter = resultQ.data.activeParameter;
      signatureHelp.activeSignature = resultQ.data.activeSignature;

      return {
        value: signatureHelp,
        dispose: () => {},
      };
    },
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
        Assemblies: [],
      };

      let resultQ = await sendRequest('hover', request);

      if (resultQ.data) {
        let posStart = model.getPositionAt(resultQ.data.offsetFrom);
        let posEnd = model.getPositionAt(resultQ.data.offsetTo);

        return {
          range: new monaco.Range(posStart.lineNumber, posStart.column, posEnd.lineNumber, posEnd.column),
          contents: [{ value: resultQ.data.information }],
        };
      }

      return null;
    },
  });

  // Add validation logic to specific models with language 'csharp'
  monaco.editor.onDidCreateModel(function (model) {
    if (model.getLanguageId() !== 'csharp') {
      return; // Skip models that are not 'csharp'
    }

    async function validate() {
      let request = {
        Code: model.getValue(),
        Assemblies: [],
      };

      let resultQ = await sendRequest('codeCheck', request);

      let markers = [];

      for (let elem of resultQ.data) {
        let posStart = model.getPositionAt(elem.offsetFrom);
        let posEnd = model.getPositionAt(elem.offsetTo);

        let severity;
    switch (elem.severity) {
      case 'Error':
        severity = monaco.MarkerSeverity.Error;
        break;
      case 'Warning':
        severity = monaco.MarkerSeverity.Warning;
        break;
      case 'Info':
        severity = monaco.MarkerSeverity.Info;
        break;
      case 'Hint':
        severity = monaco.MarkerSeverity.Hint;
        break;
      default:
        severity = monaco.MarkerSeverity.Info; // Mặc định nếu không xác định được
    }

        markers.push({
          severity: severity,
          startLineNumber: posStart.lineNumber,
          startColumn: posStart.column,
          endLineNumber: posEnd.lineNumber,
          endColumn: posEnd.column,
          message: elem.message,
          code: elem.id,
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
