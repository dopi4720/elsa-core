using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NodaTime;

namespace Elsa.Models
{
    public class FunctionDefinition : Entity
    {
        public string FunctionId { get; set; } = default!;
        public string Name { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string FunctionType { get; set; } = null!;
        public string Catalog { get; set; } = null!;
        public string Source { get; set; } = null!;
        public byte[] Binary { get; set; } = null!;
        public byte[] Pdb { get; set; } = null!;
        public int Version { get; set; }
        public bool IsPublish { get; set; }
        public DateTime LastUpdate { get; set; }
        public string SampleInput { get; set; } = null!;
    }
}