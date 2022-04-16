# RsapService Overview
.NET SDK to communicate with the CLRA RSAP (Rapid Site Access Program) API

# Description
- Get Contractors
- Get Member Statuses
- Submit Dispatch information

# Usage Tips
When initializing the RsapService class, you must provide the RSAP base URL and a HttpClient.

The RSAP base URL should be something like 'https://test-api.rsap.ca/'.

The HttpClient needs to be initialized with a HttpClientHandler which should include the ClientCertificate property populated using 'System.Security.Cryptography.X509Certificates.X509Certificate2' and your certificate PFX file location.

