using System;
using System.Collections.Generic;
using AwesomeAssertions;
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
            actual.Error.Should().Be(1);
            actual.Text.Should().BeEquivalentTo("test:\ndescription\nSystem.Exception: testException");
        }

        [Fact]
        public void BuildPrtgResponseObject_Unhealthy_ErrorText()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Unhealthy, null, TimeSpan.Zero, null, null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.Zero);

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Error.Should().Be(1);
            actual.Text.Should().BeEquivalentTo("test:\nUnhealthy");
        }

        [Fact]
        public void BuildPrtgResponseObject_Degraded()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Degraded, null, TimeSpan.FromMilliseconds(10), null, null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValueBase>
            {
                new PrtgResponseChannelValueTimeSpan { Channel = "TotalDuration", Value = 15 },
                new PrtgResponseChannelValueTimeSpan { Channel = "test.Duration", Value = 10 },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Error.Should().Be(0);
            actual.Text.Should().Be(PrtgResponse.DefaultText);
            actual.Result.Should().BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
        }

        [Fact]
        public void BuildPrtgResponseObject_Healthy()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Healthy, null, TimeSpan.FromMilliseconds(10), null, null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValueBase>
            {
                new PrtgResponseChannelValueTimeSpan { Channel = "TotalDuration", Value = 15 },
                new PrtgResponseChannelValueTimeSpan { Channel = "test.Duration", Value = 10 },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Error.Should().Be(0);
            actual.Text.Should().Be(PrtgResponse.DefaultText);
            actual.Result.Should().BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
        }

        [Fact]
        public void BuildPrtgResponseObject_OrderByStatus()
        {
            var healthReportEntryException1 =
                new HealthReportEntry(HealthStatus.Healthy, "description1", TimeSpan.FromMilliseconds(10), new Exception("testException1"), null);
            var healthReportEntryException2 =
                new HealthReportEntry(HealthStatus.Degraded, "description2", TimeSpan.FromMilliseconds(10), new Exception("testException2"), null);
            var healthReportEntryException3 =
                new HealthReportEntry(HealthStatus.Unhealthy, "description3", TimeSpan.FromMilliseconds(10), new Exception("testException3"), null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "ex1", healthReportEntryException1 }, { "ex2", healthReportEntryException2 }, { "ex3", healthReportEntryException3 } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValueBase>
            {
                new PrtgResponseChannelValueTimeSpan { Channel = "TotalDuration", Value = 15 },
                new PrtgResponseChannelValueTimeSpan { Channel = "ex1.Duration", Value = 10 },
                new PrtgResponseChannelValueTimeSpan { Channel = "ex2.Duration", Value = 10 },
                new PrtgResponseChannelValueTimeSpan { Channel = "ex3.Duration", Value = 10 },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Error.Should().Be(1);
            actual.Text.Should().Be("ex3:\ndescription3\nSystem.Exception: testException3\nex2:\ndescription2\nSystem.Exception: testException2\nex1:\ndescription1\nSystem.Exception: testException1");
            actual.Result.Should().BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
        }

        [Fact]
        public void BuildPrtgResponseObject_OrderByException()
        {
            var healthReportEntry =
                new HealthReportEntry(HealthStatus.Unhealthy, "description1", TimeSpan.FromMilliseconds(10), null, null);
            var healthReportEntryException =
                new HealthReportEntry(HealthStatus.Unhealthy, "description2", TimeSpan.FromMilliseconds(10), new Exception("testException"), null);
            var healthReportEntries = new Dictionary<string, HealthReportEntry> { { "test", healthReportEntry }, { "ex", healthReportEntryException } };
            var healthReport = new HealthReport(healthReportEntries, TimeSpan.FromMilliseconds(15));

            var expected = new List<PrtgResponseChannelValueBase>
            {
                new PrtgResponseChannelValueTimeSpan { Channel = "TotalDuration", Value = 15 },
                new PrtgResponseChannelValueTimeSpan { Channel = "test.Duration", Value = 10 },
                new PrtgResponseChannelValueTimeSpan { Channel = "ex.Duration", Value = 10 },
            };

            var actual = PrtgResponseWriter.BuildPrtgResponseObject(healthReport);
            actual.Error.Should().Be(1);
            actual.Text.Should().Be("ex:\ndescription2\nSystem.Exception: testException\ntest:\ndescription1\n");
            actual.Result.Should().BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
        }
    }
}
