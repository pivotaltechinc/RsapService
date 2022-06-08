using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class DispatchRequestModel : DispatchRequestNotWorkingModel
    {
        [JsonProperty("contractorUuid")]
        public string ContractorUuid { get; set; }

        [JsonProperty("contractorSite")]
        public string ContractorSite { get; set; }
	}
}
