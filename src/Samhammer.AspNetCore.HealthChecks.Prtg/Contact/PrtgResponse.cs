using System.Collections.Generic;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponse
    {
        public const string DefaultText = "Everything is fine :)";

        public int Error { get; set; } = 0;

        public string Text { get; set; } = DefaultText;

        public List<PrtgResponseChannelValueBase> Result { get; set; } = new List<PrtgResponseChannelValueBase>();
    }
}
