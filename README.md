# Samhammer.AspNetCore.HealthChecks.Prtg
- Creates the JSON response for the health checks in special PRTG format

## Usage
- add nuget package Samhammer.AspNetCore.HealthChecks.Prtg
```csharp
app.UseHealthChecks("/health-prtg", new HealthCheckOptions
{
	ResponseWriter = PrtgResponseWriter.WriteHealthCheckPrtgResponse
});
```

## How to publish package
- set package version in Samhammer.AspNetCore.HealthChecks.Prtg.csproj
- create git tag
- dotnet pack -c Release
- nuget push .\bin\Release\Samhammer.AspNetCore.HealthChecks.Prtg.*.nupkg NUGET_API_KEY -src https://api.nuget.org/v3/index.json
- (optional) nuget setapikey NUGET_API_KEY -source https://api.nuget.org/v3/index.json