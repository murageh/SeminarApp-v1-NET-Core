using System.Net;
using System.ServiceModel;

namespace SeminarIntegration.Models
{
    public class Credentials
    {
        //private readonly ILogger<Credentials> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        // Constructor to initialize IConfiguration and HttpClient
        public Credentials(ILogger<Credentials> logger, IConfiguration configuration)
        {
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var handler = new HttpClientHandler
            {
                Credentials = Connection.GetCredentials(),
                // Ensure proper server certificate validation
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30) // Set a reasonable timeout
            };
        }


        public NAVWS.SeminarIntegration_PortClient ObjNav()
        {
            // Configure the binding
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Security =
                {
                    Transport = new HttpTransportSecurity
                    {
                        ClientCredentialType = HttpClientCredentialType.Windows
                    }
                }
            };

            // Create the endpoint address
            var endpointAddress = new EndpointAddress("http://murageh:7047/BC240/WS/CRONUS%20International%20Ltd./Codeunit/SeminarIntegration");

            // Instantiate the service client with the binding and endpoint
            return new NAVWS.SeminarIntegration_PortClient(binding, endpointAddress);
        }


    }
}
