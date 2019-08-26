using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Samhammer.AspNetCore.HealthChecks.Prtg.Contact;
using Xunit;

namespace Samhammer.AspNetCore.HealthChecks.Prtg.Test
{
    public class PrtgResponseWriterTest
    {
        [Fact]
        public void BuildPrtgResponseObject_Unhealthy()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Unhealthy, "description", TimeSpan.Zero, new Exception("testException"), null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.Zero);

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Should().BeOfType<PrtgResponseError>();
            ((PrtgResponseError)actual).Error.Should().BeEquivalentTo("1");
            ((PrtgResponseError)actual).Text.Should().BeEquivalentTo("test:\ndescription\nSystem.Exception: testException");
        }

        [Fact]
        public void BuildPrtgResponseObject_Degraded()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Degraded, "description", TimeSpan.FromMilliseconds(10), null, null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValue>
            {
                new PrtgResponseChannelValue { Channel = "Status", Value = "Degraded" },
                new PrtgResponseChannelValue { Channel = "TotalDuration", Value = "15" },
                new PrtgResponseChannelValue { Channel = "test.Status", Value = "Degraded" },
                new PrtgResponseChannelValue { Channel = "test.Duration", Value = "10" },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Should().BeOfType<PrtgResponseSuccess>();
            ((PrtgResponseSuccess)actual).Result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void BuildPrtgResponseObject_Healthy()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Healthy, "description", TimeSpan.FromMilliseconds(10), null, null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValue>
            {
                new PrtgResponseChannelValue { Channel = "Status", Value = "Healthy" },
                new PrtgResponseChannelValue { Channel = "TotalDuration", Value = "15" },
                new PrtgResponseChannelValue { Channel = "test.Status", Value = "Healthy" },
                new PrtgResponseChannelValue { Channel = "test.Duration", Value = "10" },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Should().BeOfType<PrtgResponseSuccess>();
            ((PrtgResponseSuccess)actual).Result.Should().BeEquivalentTo(expected);
        }
    }
}
