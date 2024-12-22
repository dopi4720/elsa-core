# elsa-studio-workflow-definitions-list



<!-- Auto Generated Below -->


## Properties

| Property    | Attribute    | Description | Type            | Default     |
| ----------- | ------------ | ----------- | --------------- | ----------- |
| `basePath`  | `base-path`  |             | `string`        | `undefined` |
| `culture`   | `culture`    |             | `string`        | `undefined` |
| `history`   | --           |             | `RouterHistory` | `undefined` |
| `serverUrl` | `server-url` |             | `string`        | `undefined` |


## Methods

### `updateModel() => Promise<void>`



#### Returns

Type: `Promise<void>`




## Dependencies

### Depends on

- stencil-route-link
- [elsa-function-definitions-list-screen](../../../screens/function-definition-list/elsa-function-definitions-screen)
- context-consumer

### Graph
```mermaid
graph TD;
  elsa-studio-function-definitions-list --> stencil-route-link
  elsa-studio-function-definitions-list --> elsa-function-definitions-list-screen
  elsa-studio-function-definitions-list --> context-consumer
  elsa-function-definitions-list-screen --> stencil-route-link
  elsa-function-definitions-list-screen --> elsa-context-menu
  elsa-function-definitions-list-screen --> elsa-pager
  elsa-function-definitions-list-screen --> elsa-confirm-dialog
  elsa-function-definitions-list-screen --> context-consumer
  elsa-confirm-dialog --> elsa-modal-dialog
  style elsa-studio-function-definitions-list fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
