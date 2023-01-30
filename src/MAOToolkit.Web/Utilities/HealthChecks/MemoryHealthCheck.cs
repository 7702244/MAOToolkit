using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAOToolkit.Utilities.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly int maxMbAllocated;

        public MemoryHealthCheck(int maxMbAllocated)
        {
            this.maxMbAllocated = maxMbAllocated;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // Include GC information in the reported diagnostics.
            var allocatedMb = GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024;
            var data = new Dictionary<string, object>()
            {
                { "AllocatedMegabytes", allocatedMb },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) },
            };

            return Task.FromResult(new HealthCheckResult(
                status: allocatedMb < maxMbAllocated ? HealthStatus.Healthy : context.Registration.FailureStatus,
                description: $"Reports degraded status if allocated megabytes >= {maxMbAllocated} mb.",
                exception: null,
                data: data));
        }
    }

    public static class MemoryHealthCheckExtensions
    {
        /// <summary>
        /// Adds a healthcheck that allows to check the allocated bytes in memory and configure a threshold
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="maximumMegabytesAllocated">The maximum megabytes allowed to be allocated by the process.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'process_allocated_memory' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddProcessAllocatedMemoryHealthCheck(
            this IHealthChecksBuilder builder, int maximumMegabytesAllocated, string? name = null,
            HealthStatus? failureStatus = default, IEnumerable<string>? tags = null, TimeSpan? timeout = default)
        {
            const string PROCESS_ALLOCATED_MEMORY = "Allocated memory";

            if (maximumMegabytesAllocated <= 0)
                throw new ArgumentException($"{nameof(maximumMegabytesAllocated)} should be greater than zero");

            return builder.Add(new HealthCheckRegistration(
                name ?? PROCESS_ALLOCATED_MEMORY,
                sp => new MemoryHealthCheck(maximumMegabytesAllocated),
                failureStatus,
                tags,
                timeout));
        }
    }
}