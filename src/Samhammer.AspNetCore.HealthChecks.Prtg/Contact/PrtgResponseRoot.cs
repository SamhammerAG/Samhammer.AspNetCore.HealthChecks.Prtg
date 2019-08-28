using Newtonsoft.Json;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseRoot
    {
        [JsonProperty("prtg")]
        public PrtgResponse PrtgResponse { get; set; }

        public PrtgResponseRoot(PrtgResponse responseBase)
        {
            PrtgResponse = responseBase;
        }
    }
}
