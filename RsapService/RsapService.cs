﻿using RsapService.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RsapService
{
    public class Service
    {
        // Public properties
        public string AccessToken
        {
            get
            {
                string result = null;

                if(Token != null)
                {
                    result = Token.AccessToken;
                }

                return result;
            }
        }
        public OAuthResponseModel Token { get; private set; }


        // Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="httpClient"></param>
        /// <remarks>Supply baseUrl as the base RSAP API url (ie: 'https://api-test.rsap.ca/'). Supply httpClient with HttpClientHandler attached with a populated ClientCertificate using 'System.Security.Cryptography.X509Certificates.X509Certificate2'.</remarks>
        public Service(string baseUrl, HttpClient httpClient)
        {
            _BaseUrl = baseUrl;

            _HttpClient = httpClient;
            _HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }


        // Private properties
        private string _BaseUrl { get; set; }
        private HttpClient _HttpClient { get; set; }


        // Private methods
        private void AddRequestHeaders(HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
            {
                throw new Exception("No Bearer Token value found. May need to authenticate to Rsap again.");
            }

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);
        }
        private string Get(string endpoint, bool addRequestHeader = true)
        {
            string uri = string.Empty;
            if (_BaseUrl.EndsWith("/"))
            {
                uri = string.Format("{0}{1}", _BaseUrl, endpoint);
            }
            else
            {
                uri = string.Format("{0}/{1}", _BaseUrl, endpoint);
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(uri);

            if (addRequestHeader)
            {
                //! Only set security protocol if adding token request header. Otherwise RSAP API fails.
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                AddRequestHeaders(request);
            }

            var response = _HttpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                //ToDo: Handle errors
                throw new Exception(response.ReasonPhrase);
            }

            string result = response.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrWhiteSpace(result))
            {
                //ToDo: Handle empty response
                throw new Exception("Rsap response came back empty.");
            }

            return result;
        }
        private HttpResponseMessage Post(string endpoint, string content, bool addRequestHeader = true)
        {
            string uri = string.Empty;
            if (_BaseUrl.EndsWith("/"))
            {
                uri = string.Format("{0}{1}", _BaseUrl, endpoint);
            }
            else
            {
                uri = string.Format("{0}/{1}", _BaseUrl, endpoint);
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(uri);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            if (addRequestHeader)
            {
                //! Only set security protocol if adding token request header. Otherwise RSAP API fails.
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                AddRequestHeaders(request);
            }

            var response = _HttpClient.SendAsync(request).Result;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Handle unauthorized error
                throw new UnauthorizedAccessException(response.ReasonPhrase);
            }

            if (string.IsNullOrWhiteSpace(response.Content.ToString()))
            {
                // Handle empty response
                throw new Exception("Rsap response came back empty.");
            }

            return response;
        }


        // Public methods
        /// <summary>
        /// Get contractors from RSAP API.
        /// </summary>
        /// <returns>Array of ContractorResponseModel</returns>
        public ContractorResponseModel[] GetContractors()
        {
            ContractorResponseModel[] results = null;

            string endpoint = "api/contractors";

            try
            {
                var response = Get(endpoint, true);

                results = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractorResponseModel[]>(response);

                foreach (var result in results)
                {
                    result.RawData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return results;
        }

        /// <summary>
        /// Get statuses for all members excluding sin from RSAP API.
        /// </summary>
        /// <returns>Array of MemberStatusResponseModel</returns>
        public MemberStatusResponseModel[] GetMemberStatus()
        {
            return GetMemberStatus(programId: null, includeSin: null);
        }

        /// <summary>
        /// Get statuses for all members, optionally including sin from RSAP API.
        /// </summary>
        /// <param name="includeSin"></param>
        /// <returns>Array of MemberStatusResponseModel</returns>
        public MemberStatusResponseModel[] GetMemberStatus(bool? includeSin = null)
        {
            return GetMemberStatus(programId: null, includeSin: includeSin);
        }

        /// <summary>
        /// Get statuses for all members, or for a specific member, optionally including sin from RSAP API.
        /// </summary>
        /// <param name="programId"></param>
        /// <param name="includeSin"></param>
        /// <returns>ARray of MemberStatusResponseModel</returns>
        public MemberStatusResponseModel[] GetMemberStatus(int? programId = null, bool? includeSin = null)
        {
            MemberStatusResponseModel[] results = null;

            string endpoint = "api/members/status";
            if (programId.HasValue)
                endpoint = string.Format("api/member/{0}/status", programId.Value);

            if (includeSin.HasValue)
                endpoint += "?sin=" + includeSin.ToString().ToLower();

            try
            {
                var response = Get(endpoint, true);

                if (programId.HasValue)
                {
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<MemberStatusResponseModel>(response);
                    results = new MemberStatusResponseModel[] { result };
                }
                else
                {
                    results = Newtonsoft.Json.JsonConvert.DeserializeObject<MemberStatusResponseModel[]>(response);
                }

                foreach (var result in results)
                {
                    result.RawData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return results;
        }

        /// <summary>
        /// Get the status of the current access token.
        /// </summary>
        /// <returns>bool</returns>
        /// <remarks>Use this to determine whether you need to reauthenticate or not.</remarks>
        public bool IsAuthenticated()
        {
            bool result = false;

            if(Token != null)
            {
                if(!string.IsNullOrWhiteSpace(Token.AccessToken) && Token.ExpiresAt > DateTime.Now)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Submit dispatch information to RSAP API.
        /// </summary>
        /// <param name="models"></param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>Response 200 indicates records were processed. Response 401 unauthorized. Response 422 indicates errors occurred requiring problem records to be removed and request resubmitted.</remarks>
        public HttpResponseMessage PostDispatch<HttpRepsonseMessage>(DispatchRequestModel[] models)
        {
            string endpoint = "api/dispatch";
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(models);

            try
            {
                var response = Post(endpoint, content, true);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Submit dispatch information to RSAP API.
        /// </summary>
        /// <param name="models"></param>
        /// <returns>HttpResponseMessage</returns>
        /// <remarks>Response 200 indicates records were processed. Response 401 unauthorized. Response 422 indicates errors occurred requiring problem records to be removed and request resubmitted.</remarks>
        public DispatchResponseModel[] PostDispatch(DispatchRequestModel[] models)
        {
            try
            {
                var response = PostDispatch<HttpResponseMessage>(models);
                string responseStr = response.Content.ReadAsStringAsync().Result;
                DispatchResponseModel[] responseModels = Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchResponseModel[]>(responseStr);
                return responseModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get OAuth token for use in subsequent calls to RSAP API.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>OAuthResponseModel</returns>
        public OAuthResponseModel PostOAuth(int clientId, string clientSecret)
        {
            var requestModel = new OAuthRequestModel()
            {
                GrantType = "client_credentials",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = ""
            };
            string endpoint = "oauth/token";
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(requestModel);

            OAuthResponseModel result;

            try
            {
                var response = Post(endpoint, content, false);

                string responseContent = response.Content.ReadAsStringAsync().Result;

                result = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthResponseModel>(responseContent);

                if (!string.IsNullOrWhiteSpace(result.Error))
                {
                    throw new Exception(result.Error);
                }

                Token = result;
                Token.ExpiresAt = DateTime.Now.AddSeconds(result.ExpiresIn);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Token;
        }
    }
}
