using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Elsa.Client.Functions.Models
{
    [DataContract]
    public class FunctionDefinition
    {
        [DataMember(Order = 1)] public string FunctionId { get; set; } = null!;

        [DataMember(Order = 2)] public string Name { get; set; } = null!;

        [DataMember(Order = 3)] public string DisplayName { get; set; } = null!;

        [DataMember(Order = 4)] public string Type { get; set; } = null!;

        [DataMember(Order = 5)] public string Catalog { get; set; } = null!;

        [DataMember(Order = 6)] public string Source { get; set; } = null!;

        [DataMember(Order = 7)] public byte[] Binary { get; set; } = null!;

        [DataMember(Order = 8)] public byte[] Pdb { get; set; } = null!;

        [DataMember(Order = 9)] public int Version { get; set; }

        [DataMember(Order = 10)] public bool IsPublish { get; set; }

        [DataMember(Order = 11)] public DateTime LastUpdate { get; set; }

        [DataMember(Order = 11)] public string SampleInput { get; set; } = null!;
    }
}
