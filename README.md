# Samhammer.AspNetCore.HealthChecks.Prtg
- Creates the JSON response for the health checks in special PRTG format

## Usage
- add nuget package [Samhammer.AspNetCore.HealthChecks.Prtg](https://www.nuget.org/packages/Samhammer.AspNetCore.HealthChecks.Prtg/)
```csharp
app.UseHealthChecks("/health-prtg", new HealthCheckOptions
{
	ResponseWriter = PrtgResponseWriter.WriteHealthCheckPrtgResponse
});
```

## Prtg Setup
- use the sensor "HTTP data advanced" (https://www.paessler.com/manuals/prtg/http_data_advanced_sensor) and configure to your prtg health endpoint (like in the example above ([URL_TO_YOUR_SERVICE]/health-prtg)

## How to publish package
- Create a tag and let the github action do the publishing for you
