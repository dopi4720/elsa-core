import { Component, Prop, h } from '@stencil/core';
import { MatchResults } from '@stencil/router';
import { GetIntlMessage, IntlMessage } from "../../../i18n/intl-message";
import { loadTranslations } from "../../../i18n/i18n-loader";
import { i18n } from "i18next";
import { resources } from "./localizations";
import Tunnel from "../../../../data/dashboard";

@Component({
  tag: 'elsa-studio-function-definitions-edit',
  shadow: false,
})
export class ElsaStudioFunctionDefinitionsEdit {
  @Prop() match: MatchResults;
  @Prop() culture: string;
  private i18next: i18n;

  id?: string;

  async componentWillLoad() {
    let id = this.match.params.id;

    if (!!id && id.toLowerCase() == 'new')
      id = null;

    this.id = id;
    this.i18next = await loadTranslations(this.culture, resources);
  }

  render() {
    const id = this.id;
    const IntlMessage = GetIntlMessage(this.i18next);

    return (
      <div>
        <div class="elsa-h-16 elsa-border-b elsa-border-gray-200 elsa-px-4 elsa-py-4 sm:elsa-flex sm:elsa-items-center sm:elsa-justify-between sm:elsa-px-6 lg:elsa-px-8 elsa-bg-white">
          <div class="elsa-flex-1 elsa-min-w-0">
            <h1 class="elsa-text-lg elsa-font-medium elsa-leading-6 elsa-text-gray-900 sm:elsa-truncate">
              <IntlMessage label="Title" />
            </h1>
          </div>
          <div class="elsa-flex elsa-gap-4">
            <button id="complie" type="button" class="elsa-me-3 elsa-bg-green-500 elsa-text-white elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-green-700 hover:elsa-shadow-lg">
              Compile
            </button>
            <button id="beautify" type="button" class="elsa-me-3 elsa-bg-pink-500 elsa-text-white elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-pink-700 hover:elsa-shadow-lg">
              Beautify
            </button>
            <button id="save" type="button" class="elsa-bg-blue-500 elsa-text-white elsa-px-4 elsa-py-2 elsa-rounded hover:elsa-bg-blue-700 hover:elsa-shadow-lg">
              Save & Publish
            </button>
          </div>
        </div>

        <elsa-function-definition-editor-screen function-definition-id={id} />
      </div>
    )
  }
}

Tunnel.injectProps(ElsaStudioFunctionDefinitionsEdit, ['serverUrl', 'culture', 'basePath']);