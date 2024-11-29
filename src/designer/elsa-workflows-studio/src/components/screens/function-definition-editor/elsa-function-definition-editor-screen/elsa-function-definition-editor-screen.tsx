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
import { Monaco, initializeMonacoWorker } from "../../../controls/elsa-monaco/elsa-monaco-utils";

@Component({
  tag: 'elsa-function-definition-editor-screen',
  styleUrl: 'elsa-function-definition-editor-screen.css',
  shadow: false,
})
export class ElsaFunctionDefinitionEditorScreen {
  @Prop({ attribute: 'function-definition-id', reflect: true }) functionDefinitionId: string;
  @Prop({ attribute: 'server-url', reflect: true }) serverUrl: string;
  @Prop({ attribute: 'monaco-lib-path', reflect: true }) monacoLibPath: string;
  @Prop({ attribute: 'suggestion-server-url', reflect: true }) suggestionBaseUrl: string;
  @Prop() culture: string;
  render() {
    return (
      <div class="elsa-flex elsa-h-full">
        <div class="elsa-w-9/12 col-1 h-full-minus-header">
          <drp-monaco-editor
            value="1\n2\n3\n4"
            theme="vs-dark"
            renderLineHighlight="all"
            editor-height="100%"
            single-line={false}
            padding="elsa-p-1"
            language="csharp"
          />
        </div>

        {/* <!-- Cột bên phải --> */}
        <div class="elsa-w-3/12 elsa-flex elsa-flex-col h-full-minus-header">
          {/* <!-- Phần trên --> */}
          <div class="elsa-h-1/3 elsa-p-1 elsa-flex elsa-flex-col">
            <span>Sample Input</span>
            <div class="elsa-flex-1">
              <drp-monaco-editor
                id="sample-input"
                value=""
                theme="vs-dark"
                renderLineHighlight="none"
                editor-height="100%"
                single-line={false}
                padding="elsa-p-0"
                language="json"
                lineNumbers="off"
              />
            </div>
          </div>

          {/* <!-- Phần dưới --> */}
          <div class="elsa-h-2/3 elsa-p-1 elsa-flex elsa-flex-col">
            <span>Output</span>
            <div class="elsa-flex-1">
            <drp-monaco-editor
              id="output"
              value=""
              theme="vs-dark"
              renderLineHighlight="none"
              editor-height="100%"
              single-line={false}
              padding="elsa-p-0"
              language="json"
              lineNumbers="off"
            />
          </div>
          </div>
        </div>
      </div>
    );
  }
  private monaco: Monaco;
  suggestions: any
  i18next: i18n;
  el: HTMLElement;
  async componentWillLoad() {
    const monacoLibPath = this.monacoLibPath ?? state.monacoLibPath;
    this.i18next = await loadTranslations(this.culture, resources);
    await this.monacoLibPathChangedHandler(this.monacoLibPath);
    this.monaco = await initializeMonacoWorker(monacoLibPath);
  }
  @Watch('monacoLibPath')
  async monacoLibPathChangedHandler(newValue: string) {
    state.monacoLibPath = newValue;
  }
}

injectHistory(ElsaFunctionDefinitionEditorScreen);
DashboardTunnel.injectProps(ElsaFunctionDefinitionEditorScreen, ['serverUrl', 'culture', 'monacoLibPath', 'basePath', 'serverFeatures', 'suggestionBaseUrl']);