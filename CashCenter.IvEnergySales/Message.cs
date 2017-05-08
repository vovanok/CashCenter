using System.Windows;

namespace CashCenter.IvEnergySales
{
    public static class Message
    {
        public static void Info(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Error(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static bool YesNoQuestion(string message)
        {
            return MessageBox.Show(message, "Подтверждение действия", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
