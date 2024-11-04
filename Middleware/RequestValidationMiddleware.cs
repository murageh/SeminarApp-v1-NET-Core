using WeatherApp.Utils;

namespace WeatherApp.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class RequestValidationMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = context.Request.Headers["client_id"].ToString();
            var timestamp = context.Request.Headers["timestamp"].ToString();
            var signature = context.Request.Headers["X-Signature"].ToString();

            // Retrieve the pre-generated secret key for this client ID
            var secretKey = AppSecurity.RetrieveClientSecret(clientId); 

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(timestamp) ||
                string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secretKey) ||
                !AppSecurity.ValidateRequest(clientId, timestamp, context.Request.QueryString.ToString(), signature, secretKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized request");
                return;
            }

            await next(context); // Call the next middleware in the pipeline
        }
    }

}
