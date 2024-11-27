import { Component, Event, EventEmitter, h, Host, Listen, Method, Prop, State, Watch } from '@stencil/core';
import { injectHistory, RouterHistory } from '@stencil/router';
import {

} from '../../../../models';
import { ActivityStats, createElsaClient, eventBus, featuresDataManager, monacoEditorDialogService } from '../../../../services';
import state from '../../../../utils/store';
import DashboardTunnel from '../../../../data/dashboard';
import { downloadFromBlob } from '../../../../utils/download';
import { i18n } from 'i18next';
import { loadTranslations } from '../../../i18n/i18n-loader';
import { resources } from './localizations';
import * as collection from 'lodash/collection';
import { tr } from 'cronstrue/dist/i18n/locales/tr';
import { Monaco,initializeMonacoWorker } from "../../../controls/elsa-monaco/elsa-monaco-utils";

@Component({
  tag: 'elsa-function-definition-editor-screen',
  styleUrl: 'elsa-function-definition-editor-screen.css',
  shadow: false,
})
export class ElsaFunctionDefinitionEditorScreen {
  @Prop({ attribute: 'function-definition-id', reflect: true }) functionDefinitionId: string;
  @Prop({ attribute: 'server-url', reflect: true }) serverUrl: string;
  @Prop({ attribute: 'monaco-lib-path', reflect: true }) monacoLibPath: string;
  @Prop() culture: string;
  render() {
    return (
      <div>
        <elsa-monaco
          value=""
          language="csharp"
          theme="vs-dark"
          renderLineHighlight="all"
          isEnabledMinimap={true}
          editor-height="80vh"
          single-line={false}
          suggestions={this.suggestions}
          onValueChanged={e => {
            monacoEditorDialogService.currentValue = e.detail.value;
          }}
          ref={el => (monacoEditorDialogService.monacoEditor = el)}
        />
      </div>
    );
  }
  private monaco: Monaco;
  suggestions:any
  i18next: i18n;
  el: HTMLElement;
  async componentWillLoad() {
    const monacoLibPath = this.monacoLibPath ?? state.monacoLibPath;
    this.i18next = await loadTranslations(this.culture, resources);
    await this.monacoLibPathChangedHandler(this.monacoLibPath);
    this.monaco = await initializeMonacoWorker(monacoLibPath);
    this.suggestions =await this. provideCompletionItems();
  }
  
  @Watch('monacoLibPath')
  async monacoLibPathChangedHandler(newValue: string) {
    state.monacoLibPath = newValue;
  }

  @Method()
  async provideCompletionItems() {
    try {
      // Fetch data from the API
      const response = await fetch('https://localhost:7026/api/Suggestions/exported-types');
      const rawSuggestions = await response.json();

      // Parse data into Monaco format
      const parsedSuggestions = rawSuggestions.map(item => ({
        label: item.label,
        kind: this.mapKindToMonacoKind(item.kind), // Map kind from server to Monaco kind
        insertText: item.insertText,
        insertTextRules: this.monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
        documentation: item.documentation,
      }));

      return parsedSuggestions;
    } catch (error) {
      console.error('Error fetching suggestions:', error);
      return { suggestions: [] };
    }
  }

  mapKindToMonacoKind(kind: number) {
    switch (kind) {
      case 1: // Function
        return this.monaco.languages.CompletionItemKind.Function;
      case 2: // Variable
        return this.monaco.languages.CompletionItemKind.Variable;
      case 3: // Class
        return this.monaco.languages.CompletionItemKind.Class;
      case 4: // Interface
        return this.monaco.languages.CompletionItemKind.Interface;
      default:
        return this.monaco.languages.CompletionItemKind.Text; // Default kind
    }
  }
}

injectHistory(ElsaFunctionDefinitionEditorScreen);
DashboardTunnel.injectProps(ElsaFunctionDefinitionEditorScreen, ['serverUrl', 'culture', 'monacoLibPath', 'basePath', 'serverFeatures']);