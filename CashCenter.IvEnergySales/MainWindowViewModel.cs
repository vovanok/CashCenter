using System;
using System.Reflection;

namespace CashCenter.IvEnergySales
{
    public class MainWindowViewModel
    {
        public Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;
    }
}
