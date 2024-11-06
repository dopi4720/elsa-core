using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elsa.Server.Api.Endpoints.FunctionDefinitions.Models
{
    public class FunctionGeneralView
    {
        public FunctionGeneralView()
        {
            Message = string.Empty;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
