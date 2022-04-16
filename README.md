# RsapService Overview
.NET SDK to communicate with the CLRA RSAP (Rapid Site Access Program) API

RSAP API documentation is freely available (at the time of writing) on https://app.swaggerhub.com/apis-docs/pderas/RSAP/2.0.1.

The code written here is not special or proprietary, it's just code that would have to be written by .NET C# developers wanting to communicate with the RSAP API.

Nuget package available to assist other developers (https://www.nuget.org/packages/RsapService/).  Search for 'rsap' or 'rsapservice' in Visual Studio Nuget Package Manager.

# Description
- Get Contractors
- Get Member Statuses
- Submit Dispatch information

# Usage Tips
When initializing the RsapService class, you must provide the RSAP base URL and a HttpClient.
- BaseUrl: something like 'https://test-api.rsap.ca/'.
- HttpClient: needs to be initialized with a HttpClientHandler which should include the ClientCertificate property populated using 'System.Security.Cryptography.X509Certificates.X509Certificate2' and your certificate PFX file location.
- Provide your RsapClientId and RsapClientSecret to the PostOAuth method to authenticate and get your oauth access token. The SDK manages your subsequent requests as long as you keep the original RsapService class instantiated. If you're reinitializing the class per request, you'll have to reauthenticate each time.
