using Elsa.Client.Functions.Services;
using Elsa.Client.Services;
using Elsa.Client.Webhooks.Services;

namespace Elsa.Client
{
    public class ElsaClient : IElsaClient
    {
        public ElsaClient(
            IActivitiesApi activities,
            IWorkflowDefinitionsApi workflowDefinitions,
            IWorkflowRegistryApi workflowRegistry,
            IWorkflowInstancesApi workflowInstances,
            IWorkflowsApi workflows,
            IWebhookDefinitionsApi webhookDefinitions,
            IScriptingApi scriptingApi,
            IFunctionDefinitionsApi functionDefinitions
            )
        {
            Activities = activities;
            WorkflowDefinitions = workflowDefinitions;
            WorkflowRegistry = workflowRegistry;
            WorkflowInstances = workflowInstances;
            WebhookDefinitions = webhookDefinitions;
            Scripting = scriptingApi;
            Workflows = workflows;
            FunctionDefinitions = functionDefinitions;
        }

        public IActivitiesApi Activities { get; }
        public IWorkflowDefinitionsApi WorkflowDefinitions { get; }
        public IWorkflowRegistryApi WorkflowRegistry { get; }
        public IWorkflowInstancesApi WorkflowInstances { get; }
        public IWorkflowsApi Workflows { get; }
        public IWebhookDefinitionsApi WebhookDefinitions { get; }
        public IScriptingApi Scripting { get; }
        public IFunctionDefinitionsApi FunctionDefinitions { get; }
    }
}