namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseError : PrtgResponseBase
    {
        public const string ErrorStatusCode = "1";

        public string Error { get; set; } = ErrorStatusCode;

        public string Text { get; set; } = string.Empty;
    }
}
