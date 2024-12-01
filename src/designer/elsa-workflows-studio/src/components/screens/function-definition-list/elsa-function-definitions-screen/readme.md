# elsa-workflow-definitions-list-screen



<!-- Auto Generated Below -->


## Properties

| Property    | Attribute    | Description | Type            | Default     |
| ----------- | ------------ | ----------- | --------------- | ----------- |
| `basePath`  | `base-path`  |             | `string`        | `undefined` |
| `culture`   | `culture`    |             | `string`        | `undefined` |
| `history`   | --           |             | `RouterHistory` | `undefined` |
| `serverUrl` | `server-url` |             | `string`        | `undefined` |


## Methods

### `loadFunctionDefinitions() => Promise<void>`



#### Returns

Type: `Promise<void>`




## Dependencies

### Used by

 - [elsa-studio-function-definitions-list](../../../dashboard/pages/elsa-studio-function-definitions-list)

### Depends on

- stencil-route-link
- [elsa-context-menu](../../../controls/elsa-context-menu)
- [elsa-pager](../../../controls/elsa-pager)
- [elsa-confirm-dialog](../../../shared/elsa-confirm-dialog)
- context-consumer

### Graph
```mermaid
graph TD;
  elsa-function-definitions-list-screen --> stencil-route-link
  elsa-function-definitions-list-screen --> elsa-context-menu
  elsa-function-definitions-list-screen --> elsa-pager
  elsa-function-definitions-list-screen --> elsa-confirm-dialog
  elsa-function-definitions-list-screen --> context-consumer
  elsa-confirm-dialog --> elsa-modal-dialog
  elsa-studio-function-definitions-list --> elsa-function-definitions-list-screen
  style elsa-function-definitions-list-screen fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
