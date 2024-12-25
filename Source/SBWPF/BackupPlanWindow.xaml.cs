using Backuper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaktionslogik für NewPlanDialog.xaml
    /// </summary>
    public partial class BackupPlanWindow : Window
    {
        public Backuper.BackupPlan BackupPlan { get; set; }

        private ObservableCollection<Backuper.BackupSource> m_backupSources;
        private MainWindow m_mainWindow;
        private String m_oldName;

        public BackupPlanWindow(BackupPlan plan, MainWindow mainWindow)
        {
            m_oldName = plan.Name;
            BackupPlan = plan;
            m_mainWindow = mainWindow;

            InitializeComponent();

            m_backupSources = new ObservableCollection<BackupSource>(plan.Sources);
            backupSources.ItemsSource = m_backupSources;

            var nameBinding = new Binding("Name");
            nameBinding.Source = BackupPlan;
            nameBinding.Mode = BindingMode.TwoWay;
            backupNameTextbox.SetBinding(TextBox.TextProperty, nameBinding);

            ScheduleCombobox.ItemsSource = Enum.GetValues(typeof(Backuper.Schedule));
            var schedulebinding = new Binding("Schedule");
            schedulebinding.Source = BackupPlan;
            schedulebinding.Mode = BindingMode.TwoWay;
            ScheduleCombobox.SetBinding(ComboBox.SelectedItemProperty, schedulebinding);
        }

        private void addFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if(openFolderDialog.ShowDialog() == true)
            {
                var folderInfo = new DirectoryInfo(openFolderDialog.FolderName);
                var directoryName = folderInfo.Name;

                if (!Backuper.Backuper.ContainsSource(BackupPlan, directoryName))
                {
                    var backupSource = new BackupSource();
                    backupSource.Name = directoryName;
                    backupSource.Path = openFolderDialog.FolderName;
                    backupSource.Type = Backuper.Type.Directory;
                    m_backupSources.Add(backupSource);
                    UpdateBackupSources();
                }
                else
                {
                    if(MessageBox.Show("You cannot add a directory with the same name into the same backup package. Would you like to create a separate backup package for this directory?", "DATAROWZ Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var newPlan = new BackupPlan(directoryName, BackupPlan.Schedule);
                        var backupSource = new BackupSource();
                        backupSource.Name = directoryName;
                        backupSource.Path = openFolderDialog.FolderName;
                        backupSource.Type = Backuper.Type.Directory;
                        newPlan.Sources.Add(backupSource);
                        m_mainWindow.BackupPlans.Add(newPlan);
                        m_mainWindow.SavePlans();
                        MessageBox.Show($"Plan {directoryName} created");
                    }
                }
            }
        }

        private void addFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                if(!Backuper.Backuper.ContainsSource(this.BackupPlan, fileName))
                {
                    var backupSource = new BackupSource();
                    backupSource.Name = fileName;
                    backupSource.Path = openFileDialog.FileName;
                    backupSource.Type = Backuper.Type.File;
                    m_backupSources.Add(backupSource);
                    UpdateBackupSources();
                }
                else
                {
                    if (MessageBox.Show("You cannot add a file with the same name into the same backup package. Would you like to create a separate backup package for this file?", "DATAROWZ Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var newPlan = new BackupPlan(fileName, BackupPlan.Schedule);
                        var backupSource = new BackupSource();
                        backupSource.Name = fileName;
                        backupSource.Path = openFileDialog.FileName;
                        backupSource.Type = Backuper.Type.File;
                        newPlan.Sources.Add(backupSource);
                        m_mainWindow.BackupPlans.Add(newPlan);
                        m_mainWindow.SavePlans();
                        MessageBox.Show($"Plan {fileName} created");
                    }
                }
            }
        }

        private void removeSelection_Click(object sender, RoutedEventArgs e)
        {
            if(backupSources.SelectedItem != null)
            {
                m_backupSources.Remove((BackupSource)backupSources.SelectedItem);
            }
        }

        private void createPlanButton_Click(object sender, RoutedEventArgs e)
        {
            if(!m_oldName.Equals(this.backupNameTextbox.Text))
            {
                var oldPath = System.IO.Path.Combine(m_mainWindow.BackupsPath, m_oldName);
                var newPath = System.IO.Path.Combine(m_mainWindow.BackupsPath, this.backupNameTextbox.Text);
                if (Directory.Exists(oldPath))
                {
                    Directory.Move(oldPath, newPath);
                }
            }
            UpdateBackupSources();
            Console.WriteLine(BackupPlan.Name);
            m_mainWindow.SavePlans();
            this.Close();
        }

        private void UpdateBackupSources()
        {
            BackupPlan.Sources = m_backupSources.ToList();
        }
    }
}
