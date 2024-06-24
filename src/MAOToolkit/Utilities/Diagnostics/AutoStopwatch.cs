using System.Diagnostics;

namespace MAOToolkit.Utilities.Diagnostics
{
    public sealed class AutoStopwatch : IDisposable
    {
        public static AutoStopwatch Run(Action<TimeSpan> afterMeasuredAction) => new(afterMeasuredAction);
            
        private readonly Action<TimeSpan> _afterMeasuredAction;
        private readonly long _start;
        private bool _disposed;

        public AutoStopwatch(Action<TimeSpan> afterMeasuredAction)
        {
            _afterMeasuredAction = afterMeasuredAction;
            _start = Stopwatch.GetTimestamp();
        }

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
}