using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class DispatchRequestModel
    {
        [JsonProperty("programId")]
        public int ProgramId { get; set; }

        [JsonProperty("working")]
        public bool Working { get; set; }

        [JsonProperty("dispatchDate")]
        public string DispatchDate { get; set; }

        [JsonProperty("contractorUuid")]
        public string ContractorUuid { get; set; }

        [JsonProperty("contractorSite")]
        public string ContractorSite { get; set; }
	}
}
