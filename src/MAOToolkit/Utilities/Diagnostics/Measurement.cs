using System.Diagnostics;

namespace MAOToolkit.Utilities.Diagnostics
{
    public class Measurement
    {
        public static IDisposable Run(Action<TimeSpan> afterMeasuredAction) => new Timer(afterMeasuredAction);

        private class Timer : IDisposable
        {
            private readonly Action<TimeSpan> _afterMeasuredAction;
            private readonly long _start;

            public Timer(Action<TimeSpan> afterMeasuredAction)
            {
                _afterMeasuredAction = afterMeasuredAction;
                _start = Stopwatch.GetTimestamp();
            }

            public void Dispose()
            {
                _afterMeasuredAction.Invoke(Stopwatch.GetElapsedTime(_start));
            }
        }
    }
}