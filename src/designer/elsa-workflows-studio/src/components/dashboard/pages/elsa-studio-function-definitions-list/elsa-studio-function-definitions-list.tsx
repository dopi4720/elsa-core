import {Component, h, Prop, State} from '@stencil/core';
import { createElsaClient } from "../../../../services";
import {RouterHistory} from "@stencil/router";
import 'i18next-wc';
import {GetIntlMessage, IntlMessage} from "../../../i18n/intl-message";
import {loadTranslations} from "../../../i18n/i18n-loader";
import {resources} from "./localizations";
import {i18n} from "i18next";
import Tunnel from "../../../../data/dashboard";
import { leave, toggle } from 'el-transition'

@Component({
  tag: 'elsa-studio-function-definitions-list',
  shadow: false,
})
export class ElsaStudioFunctionDefinitionsList {
  @Prop() history: RouterHistory;
  @Prop() culture: string;
  @Prop() basePath: string;
  @Prop({ attribute: 'server-url' }) serverUrl: string;
  private i18next: i18n;
  private fileInput: HTMLInputElement;
  private workflowDefinitionsListScreen: HTMLElsaWorkflowDefinitionsListScreenElement
  private menu: HTMLElement;

  async componentWillLoad() {
    this.i18next = await loadTranslations(this.culture, resources);
  }

  restoreWorkflows = async (e: Event) => {
    e.preventDefault();
    this.fileInput.value = null;
    this.fileInput.click();
    toggle(this.menu);
  }

  toggleMenu(e?: Event) {
    toggle(this.menu);
  }

  render() {
    const basePath = this.basePath;
    const IntlMessage = GetIntlMessage(this.i18next);

    return (
      <div>
        <div class="elsa-border-b elsa-border-gray-200 elsa-px-4 elsa-py-4 sm:elsa-flex sm:elsa-items-center sm:elsa-justify-between sm:elsa-px-6 lg:elsa-px-8 elsa-bg-white">
          <div class="elsa-flex-1 elsa-min-w-0">
            <h1 class="elsa-text-lg elsa-font-medium elsa-leading-6 elsa-text-gray-900 sm:elsa-truncate">
              <IntlMessage label="Title"/>
            </h1>
          </div>
          <div class="elsa-mt-4 elsa-flex sm:elsa-mt-0 sm:elsa-ml-4">
            <span class="elsa-relative elsa-z-20 elsa-inline-flex elsa-shadow-sm elsa-rounded-md">
              <stencil-route-link url={`${basePath}/function-definitions/new`}
                class="elsa-relative elsa-inline-flex elsa-items-center elsa-px-4 elsa-py-2 elsa-border elsa-border-transparent elsa-shadow-sm elsa-text-sm elsa-font-medium elsa-rounded-md elsa-text-white elsa-bg-blue-600 hover:elsa-bg-blue-700 focus:elsa-outline-none focus:elsa-ring-2 focus:elsa-ring-offset-2 focus:elsa-ring-blue-500 focus:elsa-z-10">
                <IntlMessage label="CreateButton"/>
              </stencil-route-link>
            </span>
          </div>
        </div>

        <elsa-workflow-definitions-list-screen ref={el => this.workflowDefinitionsListScreen = el} />
      </div>
    );
  }
}
Tunnel.injectProps(ElsaStudioFunctionDefinitionsList, ['serverUrl', 'culture', 'basePath']);
