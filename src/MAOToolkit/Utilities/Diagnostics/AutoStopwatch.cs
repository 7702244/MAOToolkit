using System.Diagnostics;

namespace MAOToolkit.Utilities.Diagnostics;

/// <summary>
/// Measures elapsed time and reports it when disposed.
/// </summary>
public sealed class AutoStopwatch : IDisposable
{
    /// <summary>
    /// Starts a stopwatch that reports its elapsed time on disposal.
    /// </summary>
    public static AutoStopwatch Run(Action<TimeSpan> afterMeasuredAction) => new(afterMeasuredAction);
            
    private readonly Action<TimeSpan> _afterMeasuredAction;
    private readonly long _start;
    private bool _disposed;

    /// <summary>
    /// Starts measuring elapsed time and invokes the callback when disposed.
    /// </summary>
    public AutoStopwatch(Action<TimeSpan> afterMeasuredAction)
    {
        _afterMeasuredAction = afterMeasuredAction;
        _start = Stopwatch.GetTimestamp();
    }

    /// <summary>
    /// Reports the measured elapsed time once.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
                
        _afterMeasuredAction.Invoke(Stopwatch.GetElapsedTime(_start));
                
        _disposed = true;
    }
}
