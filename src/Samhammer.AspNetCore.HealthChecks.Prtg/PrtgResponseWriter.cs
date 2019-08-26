using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Samhammer.AspNetCore.HealthChecks.Prtg.Contact;

namespace Samhammer.AspNetCore.HealthChecks.Prtg
{
    public class PrtgResponseWriter
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        public static Task WriteHealthCheckPrtgResponse(HttpContext httpContext, HealthReport report)
        {
            var prtgBase = BuildPrtgResponseObject(report);
            var prtgRoot = new PrtgResponseRoot(prtgBase);
            var text = JsonConvert.SerializeObject(prtgRoot, SerializerSettings);

            httpContext.Response.ContentType = "application/json";
            return httpContext.Response.WriteAsync(text);
        }

        public static PrtgResponseBase BuildPrtgResponseObject(HealthReport report)
        {
            if (report.Status == HealthStatus.Healthy || report.Status == HealthStatus.Degraded)
            {
                var results = new List<PrtgResponseChannelValue>();
                results.Add(new PrtgResponseChannelValue { Channel = "Status", Value = report.Status.ToString() });
                results.Add(new PrtgResponseChannelValue { Channel = "TotalDuration", Value = report.TotalDuration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) });

                foreach (var entry in report.Entries)
                {
                    results.Add(new PrtgResponseChannelValue { Channel = $"{entry.Key}.Status", Value = entry.Value.Status.ToString() });
                    results.Add(new PrtgResponseChannelValue { Channel = $"{entry.Key}.Duration", Value = entry.Value.Duration.TotalMilliseconds.ToString(CultureInfo.InvariantCulture) });
                }

                return new PrtgResponseSuccess { Result = results };
            }

            var errors = report.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.Value.Description) || e.Value.Exception != null)
                .Select(e => $"{e.Key}:\n{e.Value.Description}\n{e.Value.Exception}");

            return new PrtgResponseError { Text = string.Join("\n", errors) };
        }
    }
}
