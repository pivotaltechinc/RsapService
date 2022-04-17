using Newtonsoft.Json;
using System;

namespace RsapService.Models
{
    public class OAuthResponseModel
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
