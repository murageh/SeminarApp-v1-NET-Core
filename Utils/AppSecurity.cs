using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace WeatherApp.Utils
{
    public class AppSecurity
    {
        private static List<string> _clientIds = ["client1", "client2", "client3"];

        private static readonly Dictionary<string, string> SecretKeys = new Dictionary<string, string>
        {
            ["client1"] = "key1",
            ["client2"] = "key2",
            ["client3"] = "key3"
        };

        public static string RetrieveClientSecret(string clientId)
        {
            return SecretKeys[clientId];
        }

        public static bool ValidateRequest(string clientId, string timestamp, string data, string receivedSignature,
            string secretKey)
        {
            // Parse the timestamp
            if (!DateTime.TryParse(
                    timestamp, 
                    // "yyyy-dd-MM hh:mm tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal,
                    out DateTime requestTime)
               )
            {
                // Invalid timestamp format
                return false;
            }

            // Check if the timestamp is within the last 120 seconds
            var currentTime = DateTime.UtcNow;
            if (Math.Abs((currentTime - requestTime).TotalSeconds) > 120)
            {
                // Request is too old or too far in the future
                return false;
            }

            // Recreate the data string to match what the client used
            var message = $"client_id={clientId}&timestamp={timestamp}&data={data}";

            // Generate hash using the secret key
            var generatedSignature = GenerateHash(message, secretKey);

            // Compare signatures
            return receivedSignature == generatedSignature;
        }

        private static string GenerateHash(string data, string secretKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}