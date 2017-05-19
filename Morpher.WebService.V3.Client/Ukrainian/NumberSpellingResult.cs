﻿using System.Runtime.Serialization;

namespace Morpher.WebService.V3.Ukrainian
{
    [DataContract(Name = "PropisUkrResult", Namespace = "http://schemas.datacontract.org/2004/07/Morpher.WebApi.Models")]
    public class NumberSpellingResult
    {
        [DataMember(Name = "n")]
        public DeclensionForms NumberDeclension { get; set; }

        [DataMember(Name = "unit")]
        public DeclensionForms UnitDeclension { get; set; }
    }
}