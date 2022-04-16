using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class ContractorResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string RawData { get; set; }
    }
}
