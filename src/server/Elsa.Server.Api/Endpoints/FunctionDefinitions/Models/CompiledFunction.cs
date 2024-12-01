using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions.Models
{
    public class CompiledFunction
    {
        public CompiledFunction()
        {
            DllBytes = new byte[0];
            PdbBytes = new byte[0];
            CompileMessage = string.Empty;
            ClassName = string.Empty;
        }

        public byte[] DllBytes { get; set; }
        public byte[] PdbBytes { get; set; }
        public string ClassName { get; set; }
        public string CompileMessage { get; set; }
        public bool IsCompiled { get; set; }
    }
}
