import { Component, Method, Prop, h } from '@stencil/core';
import { MatchResults } from '@stencil/router';
import Tunnel from '../../../../data/dashboard';

@Component({
  tag: 'elsa-studio-function-definitions-edit',
  shadow: false,
})
export class ElsaStudioFunctionDefinitionsEdit {
  @Prop() match: MatchResults;
  @Prop() culture: string;

  id?: string;

  async componentWillLoad() {
    let id = this.match.params.id;

    if (!!id && id.toLowerCase() == 'new') id = null;

    this.id = id;
  }

  render() {
    const id = this.id;
    return <elsa-function-definition-editor-screen function-definition-id={id} />;
  }
}

Tunnel.injectProps(ElsaStudioFunctionDefinitionsEdit, ['serverUrl', 'culture', 'basePath']);
