using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAOToolkit.Utilities.HealthChecks;

public class DiskStorageHealthCheck : IHealthCheck
{
    private readonly DriveInfo drive;
    private readonly int minimumFreeMegabytes;

    public DiskStorageHealthCheck(DriveInfo drive, int minimumFreeMegabytes)
    {
        this.drive = drive;
        this.minimumFreeMegabytes = minimumFreeMegabytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Include drive information in the reported diagnostics.
        long freeMb = drive.AvailableFreeSpace / 1024 / 1024;
        var data = new Dictionary<string, object>
        {
            { nameof(drive.Name), drive.Name },
            { nameof(drive.DriveFormat), drive.DriveFormat },
            { "AvailableFreeSpaceMb", freeMb },
            { "TotalSizeMb", drive.TotalSize / 1024 / 1024 },
        };

        return Task.FromResult(new HealthCheckResult(
            status: freeMb > minimumFreeMegabytes ? HealthStatus.Healthy : context.Registration.FailureStatus,
            description: $"Reports degraded status if drive free storage < {minimumFreeMegabytes} mb.",
            exception: null,
            data: data));
    }
}

public static class DiskStorageHealthCheckExtensions
{
    /// <summary>
    /// Adds a healthcheck that allows to check the drive free storage and configure a threshold.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="drive">The drive to be checked.</param>
    /// <param name="minimumFreeMegabytes">The minimum megabytes allowed to be available on disk.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'Drive free storage' will be used for the name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
    /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddDiskStorageHealthCheck(
        this IHealthChecksBuilder builder, DriveInfo drive, int minimumFreeMegabytes, string? name = null,
        HealthStatus? failureStatus = default, IEnumerable<string>? tags = null, TimeSpan? timeout = default)
    {
        const string DRIVE_FREE_STORAGE = "Drive free storage";

        if (!drive.IsReady)
            throw new ArgumentException($"{nameof(drive)} is not ready");

        if (minimumFreeMegabytes <= 0)
            throw new ArgumentException($"{nameof(minimumFreeMegabytes)} should be greater than zero");

        return builder.Add(new HealthCheckRegistration(
            name ?? DRIVE_FREE_STORAGE,
            sp => new DiskStorageHealthCheck(drive, minimumFreeMegabytes),
            failureStatus,
            tags,
            timeout));
    }
}