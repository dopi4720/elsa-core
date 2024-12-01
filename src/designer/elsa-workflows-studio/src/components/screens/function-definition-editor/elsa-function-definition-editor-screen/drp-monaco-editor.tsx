import { Component, Event, EventEmitter, h, Host, Method, Prop, Watch } from '@stencil/core';
import { initializeMonacoWorker, Monaco, EditorVariables } from '../../../controls/elsa-monaco/elsa-monaco-utils';
import state from '../../../../utils/store';

export interface DrpMonacoValueChangedArgs {
  value: string;
  markers: Array<any>;
}

@Component({
  tag: 'drp-monaco-editor',
  styleUrl: 'drp-monaco-editor.css',
  shadow: false,
})
export class DRPMonaco {
  private monaco: Monaco;
  @Prop({ attribute: 'monaco-lib-path' }) monacoLibPath: string;
  @Prop({ attribute: 'editor-height', reflect: true }) editorHeight: string = '5em';
  @Prop() value: string;
  @Prop() theme: string;
  @Prop() renderLineHighlight: string;
  @Prop() padding: string;
  @Prop() lineNumbers: string;
  @Prop() language: string;
  @Event({ eventName: 'valueChanged' }) valueChanged: EventEmitter<DrpMonacoValueChangedArgs>;

  container: HTMLElement;
  editor: any;

  @Method()
  async setValue(value: string) {
    if (!this.editor) return;

    const model = this.editor.getModel();
    model.setValue(value || '');
  }
  @Method()
  async formatCSharpCode(sourceCode: string) {
    return sourceCode;
  }
  async componentWillLoad() {
    const monacoLibPath = this.monacoLibPath ?? state.monacoLibPath;
    this.monaco = await initializeMonacoWorker(monacoLibPath);
  }
  componentDidLoad() {
    const monaco = this.monaco;

    const options = {
      value: this.value,
      language: this.language || 'csharp',
      fontFamily: 'Roboto Mono, monospace',
      renderLineHighlight: this.renderLineHighlight ? this.renderLineHighlight : 'none',
      minimap: {
        enabled: false,
      },
      automaticLayout: true,
      lineNumbers: this.lineNumbers || 'on',
      theme: this.theme ? this.theme : 'vs',
      roundedSelection: true,
      scrollBeyondLastLine: false,
      readOnly: false,
      overviewRulerLanes: 0,
      overviewRulerBorder: false,
      lineDecorationsWidth: 0,
      hideCursorInOverviewRuler: true,
      glyphMargin: false,
    };

    if (this.language === 'csharp') {
    }

    this.editor = monaco.editor.create(this.container, options);
    
    this.editor.onDidChangeModelContent(e => {
        const value = this.editor.getValue();
        const markers = monaco.editor.getModelMarkers({ owner: this.language });
        this.valueChanged.emit({ value: value, markers: markers });
      });
  }
  render() {
    const padding = this.padding || 'elsa-pt-1.5 elsa-pl-1';

    return (
      <Host
        class="elsa-monaco-editor-host elsa-border focus:elsa-ring-blue-500 focus:elsa-border-blue-500 elsa-block elsa-w-full elsa-min-w-0 elsa-rounded-md sm:elsa-text-sm elsa-border-gray-600 elsa-p-4"
        style={{ 'min-height': this.editorHeight }}
      >
        <div ref={el => (this.container = el)} class={`elsa-monaco-editor-container ${padding}`} />
      </Host>
    );
  }
}
