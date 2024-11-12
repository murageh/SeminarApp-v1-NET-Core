namespace SeminarIntegration.DTOs
{
    public class KCB
    {
        public bool success { get; set; }
        public SuccessRes? successRes { get; set; }
        public ErrorRes? errorRes { get; set; }

        public class SuccessRes
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }

        public class ErrorRes
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
    }
}
