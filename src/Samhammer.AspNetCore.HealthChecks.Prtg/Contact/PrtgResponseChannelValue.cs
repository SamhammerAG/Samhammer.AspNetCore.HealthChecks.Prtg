namespace Samhammer.AspNetCore.HealthChecks.Prtg.Contact
{


    public abstract class PrtgResponseChannelValue<T> : PrtgResponseChannelValueBase where T : struct
    {
        public string Channel { get; set; }

        public T Value { get; set; }
    }
}
