using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions.Configs
{
    public class FunctionDefinitionConfigs
    {
        public static List<string> NeedDllFiles = new List<string>()
        {
            "System.Runtime.dll",
            "System.Threading.Tasks.dll",
            "System.Linq.dll",
            "System.Collections.dll",
            "System.Console.dll",
            "System.IO.dll"
        };
    }
}
