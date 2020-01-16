using System.Linq;
using System.Net;
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
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            return httpContext.Response.WriteAsync(text);
        }

        public static PrtgResponse BuildPrtgResponseObject(HealthReport report)
        {
            var response = new PrtgResponse();
            response.Error = report.Status == HealthStatus.Unhealthy ? 1 : 0;

            var errors = report.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.Value.Description) || e.Value.Exception != null)
                .Select(e => $"{e.Key}:\n{e.Value.Description}\n{e.Value.Exception}")
                .ToList();

            if (errors.Any())
            {
                response.Text = string.Join("\n", errors);
            }

            response.Result.Add(new PrtgResponseChannelValueTimeSpan { Channel = "TotalDuration", Value = report.TotalDuration.TotalMilliseconds });

            foreach (var entry in report.Entries)
            {
                response.Result.Add(new PrtgResponseChannelValueTimeSpan { Channel = $"{entry.Key}.Duration", Value = entry.Value.Duration.TotalMilliseconds });
            }

            return response;
        }
    }
}
