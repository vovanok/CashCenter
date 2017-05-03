using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CashCenter.IvEnergySales.Behaviors
{
    public static class TextBoxBehavior
    {
        #region Select all on focus

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllTextOnFocus", typeof(bool),
                typeof(TextBoxBehavior), new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        public static bool GetSelectAllTextOnFocus(TextBox textBox)
        {
            if (textBox == null)
                return false;

            return (bool)textBox.GetValue(SelectAllTextOnFocusProperty);
        }

        public static void SetSelectAllTextOnFocus(TextBox textBox, bool value)
        {
            textBox?.SetValue(SelectAllTextOnFocusProperty, value);
        }

        private static void OnSelectAllTextOnFocusChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as TextBox;
            if (textBox == null || args == null)
                return;

            if (!(args.NewValue is bool))
                return;

            if ((bool)args.NewValue)
            {
                textBox.GotFocus += SelectAll;
                textBox.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                textBox.GotFocus -= SelectAll;
                textBox.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox?.SelectAll();
        }

        private static void IgnoreMouseButton(object sender, MouseButtonEventArgs args)
        {
            var textBox = sender as TextBox;
            if (textBox == null || textBox.IsKeyboardFocusWithin || args == null)
                return;

            args.Handled = true;
            textBox.Focus();
        }

        #endregion

        #region Tab on Enter

        public static readonly DependencyProperty IsTabOnEnterProperty =
            DependencyProperty.RegisterAttached("IsTabOnEnter", typeof(bool),
                typeof(TextBoxBehavior), new UIPropertyMetadata(false, IsTabOnEnterChanged));

        public static bool GetIsTabOnEnter(TextBox textBox)
        {
            if (textBox == null)
                return false;

            return (bool)textBox.GetValue(IsTabOnEnterProperty);
        }

        public static void SetIsTabOnEnter(TextBox textBox, bool value)
        {
            textBox?.SetValue(IsTabOnEnterProperty, value);
        }

        private static void TextBoxPreviewKeyDown(object sender, KeyEventArgs args)
        {
            var textBox = args.OriginalSource as TextBox;
            if (textBox == null || args == null)
                return;

            if (args.Key == Key.Enter)
            {
                args.Handled = true;
                textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void TextBoxUnloaded(object sender, RoutedEventArgs args)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            textBox.Unloaded -= TextBoxUnloaded;
            textBox.PreviewKeyDown -= TextBoxPreviewKeyDown;
        }

        private static void IsTabOnEnterChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as TextBox;
            if (textBox == null || args == null)
                return;

            if ((bool)args.NewValue)
            {
                textBox.Unloaded += TextBoxUnloaded;
                textBox.PreviewKeyDown += TextBoxPreviewKeyDown;
            }
            else
            {
                textBox.PreviewKeyDown -= TextBoxPreviewKeyDown;
            }
        }

        #endregion

        #region Is focus

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused", typeof(bool), typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        public static bool GetIsFocused(TextBox textBox)
        {
            if (textBox == null)
                return false;

            return (bool)textBox.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(TextBox textBox, bool value)
        {
            textBox?.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedPropertyChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as TextBox;
            if (textBox == null || args == null || !(args.NewValue is bool))
                return;

            if (args.OldValue == null)
            {
                textBox.GotFocus += (sender, routedArgs) => (sender as TextBox)?.SetValue(IsFocusedProperty, true);
                textBox.LostFocus += (sender, routedArgs) => (sender as TextBox)?.SetValue(IsFocusedProperty, false);
            }

            if ((bool)args.NewValue)
                textBox.Focus();
        }

        #endregion
    }
}
