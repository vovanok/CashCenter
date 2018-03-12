using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CashCenter.ViewCommon
{
    public static class DependencyObjectBehavior
    {
        #region Select all on focus

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectAllTextOnFocus", typeof(bool),
                typeof(DependencyObjectBehavior), new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        public static bool GetSelectAllTextOnFocus(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                return false;

            return (bool)dependencyObject.GetValue(SelectAllTextOnFocusProperty);
        }

        public static void SetSelectAllTextOnFocus(DependencyObject dependencyObject, bool value)
        {
            dependencyObject?.SetValue(SelectAllTextOnFocusProperty, value);
        }

        private static void OnSelectAllTextOnFocusChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement == null || args == null)
                return;

            if (!(args.NewValue is bool))
                return;

            if ((bool)args.NewValue)
            {
                uiElement.GotFocus += SelectAll;
                uiElement.PreviewMouseDown += IgnoreMouseButton;
            }
            else
            {
                uiElement.GotFocus -= SelectAll;
                uiElement.PreviewMouseDown -= IgnoreMouseButton;
            }
        }

        private static void SelectAll(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBoxBase;
            textBox?.SelectAll();
        }

        private static void IgnoreMouseButton(object sender, MouseButtonEventArgs args)
        {
            var uiElement = sender as UIElement;
            if (uiElement == null || uiElement.IsKeyboardFocusWithin || args == null)
                return;

            args.Handled = true;
            uiElement.Focus();
        }

        #endregion

        #region Tab on Enter

        public static readonly DependencyProperty IsTabOnEnterProperty =
            DependencyProperty.RegisterAttached("IsTabOnEnter", typeof(bool),
                typeof(DependencyObjectBehavior), new UIPropertyMetadata(false, IsTabOnEnterChanged));

        public static bool GetIsTabOnEnter(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                return false;

            return (bool)dependencyObject.GetValue(IsTabOnEnterProperty);
        }

        public static void SetIsTabOnEnter(DependencyObject dependencyObject, bool value)
        {
            dependencyObject?.SetValue(IsTabOnEnterProperty, value);
        }

        private static void TextBoxPreviewKeyDown(object sender, KeyEventArgs args)
        {
            var element = args.OriginalSource as FrameworkElement;
            if (element == null || args == null)
                return;

            if (args.Key == Key.Enter)
            {
                args.Handled = true;
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void IsTabOnEnterChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var element = dependencyObject as FrameworkElement;
            if (element == null || args == null)
                return;

            if ((bool)args.NewValue)
                element.PreviewKeyDown += TextBoxPreviewKeyDown;
            else
                element.PreviewKeyDown -= TextBoxPreviewKeyDown;
        }

        #endregion

        #region Is focus

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused", typeof(bool), typeof(DependencyObjectBehavior),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        public static bool GetIsFocused(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                return false;

            return (bool)dependencyObject.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject?.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedPropertyChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement == null || args == null || !(args.NewValue is bool))
                return;

            if (args.OldValue == null)
            {
                uiElement.GotFocus += (sender, routedArgs) => (sender as TextBox)?.SetValue(IsFocusedProperty, true);
                uiElement.LostFocus += (sender, routedArgs) => (sender as TextBox)?.SetValue(IsFocusedProperty, false);
            }

            if ((bool)args.NewValue)
                uiElement.Focus();
        }

        #endregion

        #region Drop command

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached(
                "DropCommand", typeof(ICommand), typeof(DependencyObjectBehavior),
                new UIPropertyMetadata(null, OnDropCommandChanged));

        public static ICommand GetDropCommand(DependencyObject dependencyObject)
        {
            return (ICommand)dependencyObject?.GetValue(DropCommandProperty);
        }

        public static void SetDropCommand(DependencyObject dependencyObject, ICommand value)
        {
            if (value != null)
                dependencyObject?.SetValue(DropCommandProperty, value);
        }

        private static void OnDropCommandChanged(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var uiElement = dependencyObject as UIElement;
            if (uiElement == null || args == null || !(args.NewValue is ICommand))
                return;

            var command = args.NewValue as ICommand;
            uiElement.Drop += (sender, dropArguments) => command.Execute(dropArguments);
        }

        #endregion
    }

    //public static class Dropable
    //{
        
    //}
}
