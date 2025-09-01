﻿using System.Collections.Generic;
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
    public static class PrtgResponseWriter
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
                .Where(e => !string.IsNullOrWhiteSpace(e.Value.Description) || e.Value.Exception != null || e.Value.Status == HealthStatus.Unhealthy)
                .OrderBy(e => e.Value.Exception != null ? 0 : 1)
                .ThenBy(e => e.Value.Status)
                .Select(BuildErrorText)
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

        private static string BuildErrorText(KeyValuePair<string, HealthReportEntry> entry)
        {
            if (!string.IsNullOrWhiteSpace(entry.Value.Description) || entry.Value.Exception != null)
            {
                return $"{entry.Key}:\n{entry.Value.Description}\n{entry.Value.Exception}";
            }

            return $"{entry.Key}:\n{entry.Value.Status}";
        }
    }
}
