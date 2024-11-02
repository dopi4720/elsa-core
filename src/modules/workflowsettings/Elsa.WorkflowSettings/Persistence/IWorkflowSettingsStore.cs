using Elsa.Persistence;
using Elsa.WorkflowSettings.Models;
using System.Threading.Tasks;
using System.Threading;
using Elsa.Models;

namespace Elsa.WorkflowSettings.Persistence
{
    public interface IWorkflowSettingsStore : IStore<WorkflowSetting>
    {
    }
}
