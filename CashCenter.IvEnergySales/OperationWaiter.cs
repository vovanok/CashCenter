using System;
using System.Windows.Input;

namespace CashCenter.IvEnergySales
{
    public class OperationWaiter : IDisposable
    {
        public TimeSpan TimeFromStart { get { return DateTime.Now - startTime; } }

        private DateTime startTime;

        public OperationWaiter()
        {
            startTime = DateTime.Now;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = null;
        }
    }
}
