using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class DispatchRequestNotWorkingModel
    {
        [JsonProperty("programId")]
        public int ProgramId { get; set; }

        [JsonProperty("working")]
        public bool Working { get; set; }

        [JsonProperty("dispatchDate")]
        public string DispatchDate { get; set; }
	}
}
