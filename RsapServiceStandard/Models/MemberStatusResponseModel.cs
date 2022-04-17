using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class MemberStatusResponseModel
    {
        [JsonProperty("programId")]
        public int ProgramId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("registered")]
        public bool Registered { get; set; }

        [JsonProperty("unionMemberId")]
        public string UnionMemberId { get; set; }

        [JsonProperty("IbewId")]
        public string IbewId { get; set; }

        [JsonProperty("sin")]
        public string Sin { get; set; }

        [JsonIgnore]
        public string RawData { get; set; }
    }
}
