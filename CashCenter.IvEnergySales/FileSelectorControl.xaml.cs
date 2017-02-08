using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CashCenter.IvEnergySales
{
    public partial class FileSelectorControl : UserControl
    {
        public string FilesFilter
        {
            get { return openFileDialog.Filter; }
            set { openFileDialog.Filter = value ?? string.Empty; }
        }

        public string FileName
        {
            get { return tbFileName.Text; }
            set { tbFileName.Text = value ?? string.Empty; }
        }

        private OpenFileDialog openFileDialog;

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
                tbFileName.Text = openFileDialog.FileName ?? string.Empty;
            }
        }
    }
}
