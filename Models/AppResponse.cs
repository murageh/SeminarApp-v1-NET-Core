namespace SeminarIntegration.Models
{
    public class AppResponse<T>
    {
        public class BaseResponse
        {
            public virtual bool Success { get; set; } = true;
            public string Title { get; set; }
            public string Path { get; set; }
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }

        public class SuccessResponse : BaseResponse
        {
            public override bool Success { get; set; } = true;
            public T Data { get; set; }
        }

        public class ErrorResponse : BaseResponse
        {
            public override bool Success { get; set; } = false;
        }
    }
}
