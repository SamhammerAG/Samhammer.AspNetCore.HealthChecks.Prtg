using Newtonsoft.Json;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseRoot
    {
        [JsonProperty("prtg")]
        public PrtgResponseBase PrtgResponse { get; set; }

        public PrtgResponseRoot(PrtgResponseBase responseBase)
        {
            PrtgResponse = responseBase;
        }
    }
}
