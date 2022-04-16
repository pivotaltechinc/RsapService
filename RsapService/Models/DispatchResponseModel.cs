using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class DispatchResponseModel
    {
        [JsonProperty("programId")]
        public int ProgramId { get; set; }

        [JsonProperty("accepted")]
        public bool Accepted { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
