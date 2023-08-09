﻿using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.eShopWeb.Web.HealthChecks;

public class SystemHealthCheck : IHealthCheck
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SystemHealthCheck(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        string drive = request.Query.ContainsKey("drive") ? request.Query["drive"] : "C";
        Process process = new Process();
        process.StartInfo.FileName = @"cmd.exe";
        process.StartInfo.Arguments = $"/C fsutil volume diskfree {drive}:";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();

        double freeSpacePercent = double.Parse(output.Trim().Split(' ')[6]);

        process.WaitForExit();

        if (freeSpacePercent > 10)
        {
            return HealthCheckResult.Healthy("The check indicates a healthy result.");
        }
        else
        {
            return HealthCheckResult.Unhealthy("The check indicates an unhealthy result.");
        }


    }
}
