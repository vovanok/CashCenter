using System;
using System.Windows.Input;

namespace CashCenter.IvEnergySales
{
    public class OperationWaiter : IDisposable
    {
        public OperationWaiter()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = null;
        }
    }
}
