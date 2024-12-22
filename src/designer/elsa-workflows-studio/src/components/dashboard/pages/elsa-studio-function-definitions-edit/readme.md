# elsa-studio-workflow-definitions-edit



<!-- Auto Generated Below -->


## Properties

| Property    | Attribute    | Description | Type           | Default     |
| ----------- | ------------ | ----------- | -------------- | ----------- |
| `culture`   | `culture`    |             | `string`       | `undefined` |
| `match`     | --           |             | `MatchResults` | `undefined` |
| `serverUrl` | `server-url` |             | `string`       | `undefined` |


## Dependencies

### Depends on

- [elsa-function-definition-editor-screen](../../../screens/function-definition-editor/elsa-function-definition-editor-screen)
- context-consumer

### Graph
```mermaid
graph TD;
  elsa-studio-function-definitions-edit --> elsa-function-definition-editor-screen
  elsa-studio-function-definitions-edit --> context-consumer
  elsa-function-definition-editor-screen --> drp-monaco-editor
  elsa-function-definition-editor-screen --> context-consumer
  style elsa-studio-function-definitions-edit fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
