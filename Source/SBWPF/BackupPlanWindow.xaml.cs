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
                var folderInfo = new FileInfo(openFolderDialog.FolderName);

                var backupSource = new BackupSource();
                backupSource.Name = GetSourceFolderName(folderInfo.Name, m_backupSources.ToList());
                backupSource.Path = openFolderDialog.FolderName;
                backupSource.Type = Backuper.Type.Directory;

                m_backupSources.Add(backupSource);
            }
        }

        private void addFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var folderInfo = new FileInfo(openFileDialog.FileName);

                var backupSource = new BackupSource();
                backupSource.Name = GetSourceFileName(System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName), System.IO.Path.GetExtension(openFileDialog.FileName), m_backupSources.ToList());
                backupSource.Path = openFileDialog.FileName;
                backupSource.Type = Backuper.Type.File;

                m_backupSources.Add(backupSource);
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
            BackupPlan.Sources = m_backupSources.ToList();
            Console.WriteLine(BackupPlan.Name);
            m_mainWindow.SavePlans();
            this.Close();
        }

        private static String GetSourceFolderName(String oldName, List<BackupSource> backupSources)
        {
            if(IsDuplicate(oldName, backupSources))
            {
                var i = 1;
                string fileName = $"{oldName}_{i}";
                while (IsDuplicate(fileName, backupSources))
                {
                    i++;
                    fileName = $"{oldName}_{i}";
                }
                return fileName;
            }
            return oldName;
        }

        private static String GetSourceFileName(String oldName, String extension, List<BackupSource> backupSources)
        {
            if (IsDuplicate(oldName + extension, backupSources))
            {
                var i = 1;
                string fileName = $"{oldName}_{i}{extension}";
                while (IsDuplicate(fileName, backupSources))
                {
                    i++;
                    fileName = $"{oldName}_{i}{extension}";
                }
                return fileName;
            }
            return oldName + extension;
        }

        private static bool IsDuplicate(String name, List<BackupSource> backupSources)
        {
            foreach (var item in backupSources)
            {
                if(item.Name.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
