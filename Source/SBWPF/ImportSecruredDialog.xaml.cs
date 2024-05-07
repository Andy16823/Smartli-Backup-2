using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für ImportSecruredDialog.xaml
    /// </summary>
    public partial class ImportSecruredDialog : Window
    {
        private String m_archiveName;
        private MainWindow m_mainWindow;

        /// <summary>
        /// Creates an new instance of the dialog
        /// </summary>
        /// <param name="archiveName"></param>
        /// <param name="mainWindow"></param>
        public ImportSecruredDialog(String archiveName, MainWindow mainWindow)
        {
            InitializeComponent();
            m_archiveName = archiveName;
            m_mainWindow = mainWindow;
        }

        /// <summary>
        /// Tryes to import asynchron the encrypted backup archive
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importBackups_Click(object sender, RoutedEventArgs e)
        {
            var password = passwordTextfield.Password.ToString();
            m_mainWindow.StartProgress();
            m_mainWindow.SendInfoGrowl("Starting the import from the backup archive");
            Backuper.Backuper.ImportFromArchiveSecuredAsync(m_archiveName, m_mainWindow.BackupsPath, password, (plan, args) =>
            {
                m_mainWindow.Dispatcher.Invoke(() =>
                {
                    m_mainWindow.StopProgress();
                    if (plan != null)
                    {
                        m_mainWindow.SendSuccessGrowl("Import done");
                        if (!m_mainWindow.BackupPlans.Contains(plan))
                        {
                            m_mainWindow.BackupPlans.Add(plan);
                        }
                        m_mainWindow.SavePlans();
                    }
                    else
                    {
                        m_mainWindow.SendErrorGrowl("Wrong Password");
                    }
                });

                if(plan != null)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Close();
                    });
                }
            });
        }
    }
}
