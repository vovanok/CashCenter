using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales
{
    public partial class FileSelectorControl : UserControl
    {
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string),
            typeof(FileSelectorControl), new FrameworkPropertyMetadata(string.Empty));

        private OpenFileDialog openFileDialog;

        public string FilesFilter
        {
            get { return openFileDialog.Filter; }
            set { openFileDialog.Filter = value ?? string.Empty; }
        }

        public string Filename
        {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }

        public FileSelectorControl()
        {
            InitializeComponent();
            openFileDialog = new OpenFileDialog();
        }

        private void On_btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != null && openFileDialogResult.Value)
            {
                tbFileName.Text = Filename = openFileDialog.FileName ?? string.Empty;
            }
        }

        private void On_tbFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filename = tbFileName.Text;
        }
    }
}
