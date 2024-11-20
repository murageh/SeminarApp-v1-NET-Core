namespace SeminarIntegration.Utils
{
    public class UrlHelper(IConfiguration config)
    {
        public string GenerateUnboundUrl(string functionName)
        {
            return $"{config["AppSettings:PORTALCODEUNIT"]}{functionName}?company={config["AppSettings:BCOMPANY"]}";
        }
    }
}
