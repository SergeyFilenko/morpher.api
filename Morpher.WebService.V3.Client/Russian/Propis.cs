﻿using System.Runtime.Serialization;

namespace Morpher.WebService.V3.Russian
{
    [DataContract(Name = "SummaPropisResult")]
    public class Propis
    {
        [DataMember(Name = "propis1")]
        public string propis1 { get; set; }

        [DataMember(Name = "propis2")]
        public string propis2 { get; set; }

        [DataMember(Name = "propis3")]
        public string propis3 { get; set; }

        [DataMember(Name = "amount")]
        public string amount { get; set; }

        [DataMember(Name = "currency")]
        public string currency { get; set; }

        [DataMember(Name = "cents")]
        public string cents { get; set; }
    }
}