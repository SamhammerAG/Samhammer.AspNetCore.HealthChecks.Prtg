using System.Text.Json.Serialization;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseRoot
    {
        [JsonPropertyName("prtg")]
        public PrtgResponse PrtgResponse { get; set; }

        public PrtgResponseRoot(PrtgResponse responseBase)
        {
            PrtgResponse = responseBase;
        }
    }
}
