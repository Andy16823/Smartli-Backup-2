using Backuper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SBWPF
{
    /// <summary>
    /// Interaktionslogik für ExportSecuredDialog.xaml
    /// </summary>
    public partial class ExportSecuredDialog : Window
    {
        private BackupPlan m_backupPlan;
        private MainWindow m_mainWindow;

        /// <summary>
        /// Creates an new instance of the export dialog
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="window"></param>
        public ExportSecuredDialog(BackupPlan plan, MainWindow window)
        {
            InitializeComponent();
            m_backupPlan = plan;
            m_mainWindow = window;
        }

        /// <summary>
        /// Exports the archive enchrypted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportBackups_Click(object sender, RoutedEventArgs e)
        {
            var password = passwordTextfield.Password.ToString();
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".esba";
            if (saveFileDialog.ShowDialog() == true)
            {
                m_mainWindow.StartProgress();
                object[] args = { saveFileDialog.FileName };
                m_mainWindow.SendInfoGrowl("Starting the export from " + m_backupPlan.Name);
                Backuper.Backuper.ExportBackupSecuredAsynch(m_backupPlan, m_mainWindow.BackupsPath, saveFileDialog.FileName, password, (a) =>
                {
                    m_mainWindow.Dispatcher.Invoke(() =>
                    {
                        m_mainWindow.SendSuccessGrowl("Export done!");
                        m_mainWindow.StopProgress();

                        FileInfo fileInfo = new FileInfo((String)a[0]);
                        Process.Start("explorer.exe", fileInfo.DirectoryName);
                    });

                    this.Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });

                }, args);
            }
        }
    }
}
