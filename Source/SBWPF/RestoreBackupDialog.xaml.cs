using Backuper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Animation;
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

            var backupDirectory = Backuper.Backuper.GetPlanFolder(m_backupPlan, m_mainWindow.BackupsPath);
            var listViewItems = new List<BackupListviewItem>();
            var backups = Backuper.Backuper.GetBackups(plan, window.BackupsPath);
            foreach (var backup in backups)
            {
                var backupfile = System.IO.Path.Combine(backupDirectory, backup);
                var backupValid = Backuper.Backuper.CanBackupGetRestored(backupfile);
                var backupInformation = Backuper.Backuper.GetBackupInformationFromArchive(backupfile);

                var listViewItem = new BackupListviewItem();
                listViewItem.Name = backup;
                listViewItem.Type = backupInformation.BackupType.ToString();
                listViewItem.CreationTime = backupInformation.BackupTime.ToShortDateString() + " " + backupInformation.BackupTime.ToShortTimeString();

                if(backupValid)
                {
                    listViewItem.Icon = "Images/check-icon.png";
                    listViewItem.Valid = "Valid";
                }
                else
                {
                    listViewItem.Icon = "Images/warning-icon.png";
                    listViewItem.Valid = "Invalid";
                }

                listViewItems.Add(listViewItem);
            }
            backupsListView.ItemsSource = listViewItems;
        }

        private void createPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if(!m_mainWindow.IsInProgress)
            {
                if (extractOnlyCheckbox.IsChecked == true)
                {
                    if (this.backupsListView.SelectedItem != null)
                    {
                        var selectedItem = (BackupListviewItem) this.backupsListView.SelectedItem;
                        String backupName = selectedItem.Name;
                        var backupDirectory = Backuper.Backuper.GetPlanFolder(m_backupPlan, m_mainWindow.BackupsPath);
                        var backupfile = System.IO.Path.Combine(backupDirectory, backupName);

                        if (Backuper.Backuper.CanBackupGetRestored(backupfile))
                        {
                            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                            if (openFolderDialog.ShowDialog() == true)
                            {
                                m_mainWindow.SendInfoGrowl("Backup extraction started");
                                m_mainWindow.StartProgress();
                                Backuper.Backuper.ExtractBackupAsync(backupfile, openFolderDialog.FolderName, () =>
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
                        else
                        {
                            MessageBox.Show("Unable to export the selected backup because required incremental backups are missing.");
                        }
                    }
                }
                else
                {
                    if (System.Windows.MessageBox.Show("With the restore of the backup all existing files gets deleted. Are you sure you want to restore the backup?", "Smart Backup", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (this.backupsListView.SelectedItem != null)
                        {
                            var selectedItem = (BackupListviewItem)this.backupsListView.SelectedItem;
                            var backupDirectory = Backuper.Backuper.GetPlanFolder(m_backupPlan, m_mainWindow.BackupsPath);
                            String backupName = selectedItem.Name;
                            var backupfile = System.IO.Path.Combine(backupDirectory, backupName);

                            if(Backuper.Backuper.CanBackupGetRestored(backupfile))
                            {
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
                            else
                            {
                                MessageBox.Show("Unable to restore the selected backup because required incremental backups are missing.");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Another progress is running. Please wait to finish this progress and start again!");
            }
        }
    }
}
