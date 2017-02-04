using System.Windows;
using Microsoft.Win32;
using System.IO;
using CashCenter.Common;
using System;

namespace CashCenter.IvEnergySales
{
    public partial class SettingsDialog : Window
    {
        private bool isCustomerDbfImported;

        public SettingsDialog()
        {
            InitializeComponent();

            tbCasherName.Text = Properties.Settings.Default.CasherName;
            cbIsCustomerOfflineMode.IsChecked = Properties.Settings.Default.IsCustomerOfflineMode;
            tbCustomerInputDbfPath.Text = Properties.Settings.Default.CustomerInputDbfPath;
        }

        private void On_btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CasherName = tbCasherName.Text;

            if (cbIsCustomerOfflineMode.IsChecked != null)
                Properties.Settings.Default.IsCustomerOfflineMode = cbIsCustomerOfflineMode.IsChecked.Value;

            Properties.Settings.Default.CustomerInputDbfPath = tbCustomerInputDbfPath.Text;
            Properties.Settings.Default.Save();

            DialogResult = true;
            Close();
        }

        private void On_btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TryDeleteImportedFile();

            DialogResult = false;
            Close();
        }

        private void On_btnSelectCustomerInputDbfPath_Click(object sender, RoutedEventArgs e)
        {
            var dbfFilename = SelectDbfFile();
            if (dbfFilename != null)
            {
                TryDeleteImportedFile();
                tbCustomerInputDbfPath.Text = dbfFilename;
            }
        }

        private void On_btnImportAndSelectCustomerInputDbfPath_Click(object sender, RoutedEventArgs e)
        {
            var errorPrefix = "Ошибка импортирования файла (физ. лица).";

            var dbfFilename = SelectDbfFile();
            if (dbfFilename == null)
                return;

            if (!File.Exists(dbfFilename))
            {
                Log.Error($"{errorPrefix} Выбран несуществующий файл {dbfFilename}");
                return;
            }

            TryDeleteImportedFile();

            try
            {
                var targetSubdirectoryName = string.Format(Config.CustomerInputSubdirectoryNameFormat,
                    Path.GetFileNameWithoutExtension(dbfFilename), DateTime.Now);

                var targetDirectory = Path.Combine(Config.InputDirectory, targetSubdirectoryName);

                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                var targetShortFilename = Path.GetFileNameWithoutExtension(dbfFilename).Substring(0, 8) + Path.GetExtension(dbfFilename);
                var targetFilename = Path.Combine(targetDirectory, targetShortFilename);

                File.Copy(dbfFilename, targetFilename, true);
                isCustomerDbfImported = true;

                tbCustomerInputDbfPath.Text = targetFilename;
            }
            catch(Exception ex)
            {
                Log.ErrorWithException(errorPrefix, ex);
            }
        }

        private string SelectDbfFile()
        {
            var selectDbfFileDialog = new OpenFileDialog();
            selectDbfFileDialog.Filter = "*.DBF|*.DBF";
            selectDbfFileDialog.CheckFileExists = true;

            var selectDbfFileDialogResult = selectDbfFileDialog.ShowDialog();
            if (selectDbfFileDialogResult != null && selectDbfFileDialogResult.Value)
                return selectDbfFileDialog.FileName;

            return null;
        }

        private void TryDeleteImportedFile()
        {
            try
            {
                if (isCustomerDbfImported && File.Exists(tbCustomerInputDbfPath.Text))
                    File.Delete(tbCustomerInputDbfPath.Text);
            }
            finally
            {
                isCustomerDbfImported = false;
            }
        }
    }
}
