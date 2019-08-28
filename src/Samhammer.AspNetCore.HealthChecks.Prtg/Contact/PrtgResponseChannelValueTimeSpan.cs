namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{
    public class PrtgResponseChannelValueTimeSpan : PrtgResponseChannelValue<double>
    {
        public bool Float { get; set; } = true;
        
        public string Unit { get; set; } = "TimeResponse";
    }
}
