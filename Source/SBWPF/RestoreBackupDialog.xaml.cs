using Backuper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
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
    /// Interaktionslogik für RestoreBackup.xaml
    /// </summary>
    public partial class RestoreBackupDialog : Window
    {
        private BackupPlan m_backupPlan;
        private MainWindow m_mainWindow;

        public RestoreBackupDialog(BackupPlan plan, MainWindow window)
        {
            InitializeComponent();
            m_backupPlan = plan;
            m_mainWindow = window;

            selectedBackupComboBox.ItemsSource = Backuper.Backuper.GetBackupsObservableCollection(plan, window.BackupsPath);
        }

        private void createPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if(!m_mainWindow.IsInProgress)
            {
                if (extractOnlyCheckbox.IsChecked == true)
                {
                    if (this.selectedBackupComboBox.SelectedItem != null)
                    {
                        String backupName = this.selectedBackupComboBox.SelectedItem.ToString();
                        OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                        if (openFolderDialog.ShowDialog() == true)
                        {
                            m_mainWindow.SendInfoGrowl("Backup extraction started");
                            m_mainWindow.StartProgress();
                            Backuper.Backuper.ExtractBackupAsync(m_backupPlan, backupName, m_mainWindow.BackupsPath, openFolderDialog.FolderName, () =>
                            {
                                m_mainWindow.Dispatcher.Invoke(() =>
                                {
                                    m_mainWindow.StopProgress();
                                    m_mainWindow.SendSuccessGrowl("Backup extraction completed");
                                    Process.Start("explorer.exe", @openFolderDialog.FolderName);
                                });
                            });
                        }
                    }
                }
                else
                {
                    if (System.Windows.MessageBox.Show("With the restore of the backup all existing files gets deleted. Are you sure you want to restore the backup?", "Smart Backup", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (this.selectedBackupComboBox.SelectedItem != null)
                        {
                            var backupDirectory = Backuper.Backuper.GetPlanFolder(m_backupPlan, m_mainWindow.BackupsPath);
                            String backupName = this.selectedBackupComboBox.SelectedItem.ToString();
                            var backupfile = System.IO.Path.Combine(backupDirectory, backupName);
                            m_mainWindow.SendInfoGrowl("Backup extraction started");
                            m_mainWindow.StartProgress();
                            Backuper.Backuper.RestoreBackupAsync(backupfile, () =>
                            {
                                m_mainWindow.Dispatcher.Invoke(() =>
                                {
                                    m_mainWindow.StopProgress();
                                    m_mainWindow.SendSuccessGrowl("Backup Restored");
                                });
                            });
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Another progress is running. Please wait to finish this progress and start again!");
            }
        }

        private void selectedBackupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(selectedBackupComboBox.SelectedItem != null)
            {
                var backupDirectory = Backuper.Backuper.GetPlanFolder(m_backupPlan, m_mainWindow.BackupsPath);
                String backupName = this.selectedBackupComboBox.SelectedItem.ToString();
                var backupfile = System.IO.Path.Combine(backupDirectory, backupName);

                var fileInfo = new FileInfo(backupfile);
                this.creationDateLabel.Text = fileInfo.CreationTime.ToString();
            }
        }
    }
}
