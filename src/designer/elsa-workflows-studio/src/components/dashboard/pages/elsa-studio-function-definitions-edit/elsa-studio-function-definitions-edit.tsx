import {Component, Prop, h} from '@stencil/core';
import {MatchResults} from '@stencil/router';

@Component({
  tag: 'elsa-studio-function-definitions-edit',
  shadow: false,
})
export class ElsaStudioFunctionDefinitionsEdit {
  @Prop() match: MatchResults;
  
  id?: string;

  componentWillLoad() {
    let id = this.match.params.id;

    if (!!id && id.toLowerCase() == 'new')
      id = null;

    this.id = id;
  }

  render() {
    const id = this.id;

    return <div>
      <elsa-function-definition-editor-screen function-definition-id={id} />
    </div>;
  }
}
