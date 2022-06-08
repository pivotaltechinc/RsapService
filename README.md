# RsapService Overview
.NET SDK to communicate with the CLRA RSAP (Rapid Site Access Program) API

The RSAP API was developed by https://pderas.com/, and documentation is freely available (at time of writing) here: https://app.swaggerhub.com/apis-docs/pderas/RSAP/2.0.1.

This SDK code is not special or proprietary, it's just code that would have to be written by .NET C# developers wanting to communicate with the RSAP API.

Nuget package available here: (https://www.nuget.org/packages/RsapService/). Search for 'rsap' or 'rsapservice' in Visual Studio Nuget Package Manager.

# Description
- Get Contractors
- Get Member Statuses
- Submit Dispatch information

# Versions
- 1.0.5 - Built on .NET Framework 4.6.2. Should be compatible with any Framework version above 4.6.2 up to 4.8.
- 2.0.7 - Built on .NET Standard 2.0. Should work with .NET Framework versions as well as .NET Standard 2.1.

# Usage Tips
When initializing the RsapService class, you must provide the RSAP base URL and a HttpClient.
- BaseUrl: something like 'https://test-api.rsap.ca/'.
- HttpClient: needs to be initialized with a HttpClientHandler which should include the ClientCertificate property populated using 'System.Security.Cryptography.X509Certificates.X509Certificate2' and your certificate PFX file location.
- Provide your RsapClientId and RsapClientSecret to the PostOAuth method to authenticate and get your oauth access token. The SDK manages your subsequent requests as long as you keep the original RsapService class instantiated. If you're reinitializing the class per request, you'll have to reauthenticate each time.
