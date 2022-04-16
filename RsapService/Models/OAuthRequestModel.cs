using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    internal class OAuthRequestModel
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }

        [JsonProperty("client_id")]
        public int ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
