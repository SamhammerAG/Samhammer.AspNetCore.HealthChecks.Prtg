using System.Collections.Generic;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseSuccess : PrtgResponseBase
    {
        public List<PrtgResponseChannelValue> Result { get; set; } = new List<PrtgResponseChannelValue>();
    }
}
