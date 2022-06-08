using Microsoft.VisualStudio.TestTools.UnitTesting;
using RsapService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RsapServiceTests
{
    [TestClass]
    public class FrameworkTests
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            string baseUrl = Properties.Settings.Default.RsapBaseUrl;
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(Properties.Settings.Default.CertPfxFilePath));
            _HttpClient = new HttpClient(handler);
            _Process = new RsapService.Service(baseUrl, _HttpClient);
        }

        // Set to a known ProgramId that is associated to your ClientId.
        private const int _KnownProgramIdFromYourAccount = 0;
        private static HttpClient _HttpClient { get; set; }
        private static RsapService.Service _Process { get; set; }

        private OAuthResponseModel Authenticate()
        {
            return _Process.PostOAuth(Properties.Settings.Default.RsapClientId, Properties.Settings.Default.RsapClientSecret);
        }

        private async Task<OAuthResponseModel> AuthenticateAsync()
        {
            return await _Process.PostOAuthAsync(Properties.Settings.Default.RsapClientId, Properties.Settings.Default.RsapClientSecret);
        }

        private void CheckKnownProgramId()
        {
            if (_KnownProgramIdFromYourAccount <= 0)
            {
                throw new Exception("Set " + nameof(_KnownProgramIdFromYourAccount) + " to a ProgramId associated to your ClientId.");
            }
        }

        [TestMethod]
        public void OAuth_Test()
        {
            OAuthResponseModel responseModel = Authenticate();
            Assert.IsTrue(responseModel != null && !string.IsNullOrWhiteSpace(responseModel.AccessToken));
        }

        [TestMethod]
        public void Invalid_BaseUrl_Test()
        {
            string baseUrl = "https://fake-endpoint.rsap.ca/";
            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(Properties.Settings.Default.CertPfxFilePath));
            _HttpClient = new HttpClient(handler);
            _Process = new RsapService.Service(baseUrl, _HttpClient);

            try
            {
                OAuthResponseModel responseModel = Authenticate();
                Assert.Fail("BaseUrl '" + baseUrl + "' should have failed.");
            }
            catch(Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("Service Unavailable"));
            }
        }

        [TestMethod]
        public async Task OAuth_Async_Test()
        {
            OAuthResponseModel responseModel = await AuthenticateAsync();
            Assert.IsTrue(responseModel != null && !string.IsNullOrWhiteSpace(responseModel.AccessToken));
        }

        [TestMethod]
        public void Contractors_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            ContractorResponseModel[] responseModel = _Process.GetContractors();
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && !string.IsNullOrWhiteSpace(responseModel[0].Uuid));
        }

        [TestMethod]
        public async Task Contractors_Async_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            ContractorResponseModel[] responseModel = await _Process.GetContractorsAsync();
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && !string.IsNullOrWhiteSpace(responseModel[0].Uuid));
        }

        [TestMethod]
        public void Statuses_AllWithoutSin_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            MemberStatusResponseModel[] responseModel = _Process.GetMemberStatus();
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0);
        }

        [TestMethod]
        public async Task Statuses_AllWithoutSin_Async_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            MemberStatusResponseModel[] responseModel = await _Process.GetMemberStatusAsync();
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0);
        }

        [TestMethod]
        public void Statuses_AllWithSin_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            MemberStatusResponseModel[] responseModel = _Process.GetMemberStatus(includeSin: true);
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0
                && responseModel.Any(x => !string.IsNullOrWhiteSpace(x.Sin)));
        }

        [TestMethod]
        public async Task Statuses_AllWithSin_Async_Test()
        {
            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            MemberStatusResponseModel[] responseModel = await _Process.GetMemberStatusAsync(includeSin: true);
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0
                && responseModel.Any(x => !string.IsNullOrWhiteSpace(x.Sin)));
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public void Statuses_Single_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            MemberStatusResponseModel[] responseModel = _Process.GetMemberStatus(programId: programId, includeSin: false);
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public async Task Statuses_Single_Async_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            MemberStatusResponseModel[] responseModel = await _Process.GetMemberStatusAsync(programId: programId, includeSin: false);
            Assert.IsTrue(responseModel != null
                && responseModel.Length > 0
                && responseModel[0].ProgramId > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public void Dispatch_NotWorking_HttpResponseMessage_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = false,
                DispatchDate = "2022-04-13"
            });

            HttpResponseMessage response = _Process.PostDispatch<HttpResponseMessage>(requestModels.ToArray());
            string responseStr = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(responseStr));

            DispatchResponseModel[] responseModels = Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchResponseModel[]>(responseStr);
            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public async Task Dispatch_NotWorking_HttpResponseMessage_Async_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = false,
                DispatchDate = "2022-04-13"
            });

            HttpResponseMessage response = await _Process.PostDispatchAsync<HttpResponseMessage>(requestModels.ToArray());
            string responseStr = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(responseStr));

            DispatchResponseModel[] responseModels = Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchResponseModel[]>(responseStr);
            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public void Dispatch_NotWorking_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = false,
                DispatchDate = "2022-04-13"
            });

            DispatchResponseModel[] responseModels = _Process.PostDispatch(requestModels.ToArray());

            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public async Task Dispatch_NotWorking_Async_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = false,
                DispatchDate = "2022-04-13"
            });

            DispatchResponseModel[] responseModels = await _Process.PostDispatchAsync(requestModels.ToArray());

            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public void Dispatch_Working_HttpResponseMessage_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = true,
                DispatchDate = "2022-05-30",
                ContractorUuid = "b859a52e-a155-498b-8397-5a77bca8f871",
                ContractorSite = "TestSite"
            });

            HttpResponseMessage response = _Process.PostDispatch<HttpResponseMessage>(requestModels.ToArray());
            string responseStr = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(responseStr));

            DispatchResponseModel[] responseModels = Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchResponseModel[]>(responseStr);
            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public async Task Dispatch_Working_HttpResponseMessage_Async_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = true,
                DispatchDate = "2022-05-30",
                ContractorUuid = "b859a52e-a155-498b-8397-5a77bca8f871",
                ContractorSite = "TestSite"
            });

            HttpResponseMessage response = await _Process.PostDispatchAsync<HttpResponseMessage>(requestModels.ToArray());
            string responseStr = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(responseStr));

            DispatchResponseModel[] responseModels = Newtonsoft.Json.JsonConvert.DeserializeObject<DispatchResponseModel[]>(responseStr);
            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public void Dispatch_Working_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                Authenticate();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = true,
                DispatchDate = "2022-05-30",
                ContractorUuid = "b859a52e-a155-498b-8397-5a77bca8f871",
                ContractorSite = "TestSite"
            });

            DispatchResponseModel[] responseModels = _Process.PostDispatch(requestModels.ToArray());

            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }

        [TestMethod]
        [DataRow(_KnownProgramIdFromYourAccount)]
        public async Task Dispatch_Working_Async_Test(int programId)
        {
            CheckKnownProgramId();

            if (string.IsNullOrWhiteSpace(_Process.AccessToken))
            {
                await AuthenticateAsync();
            }

            List<DispatchRequestModel> requestModels = new List<DispatchRequestModel>();
            requestModels.Add(new DispatchRequestModel()
            {
                ProgramId = programId,
                Working = true,
                DispatchDate = "2022-05-30",
                ContractorUuid = "b859a52e-a155-498b-8397-5a77bca8f871",
                ContractorSite = "TestSite"
            });

            DispatchResponseModel[] responseModels = await _Process.PostDispatchAsync(requestModels.ToArray());

            Assert.IsTrue(responseModels != null
                && responseModels.Length > 0);
        }
    }
}
