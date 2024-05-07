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


        public BackupPlanWindow(BackupPlan plan, MainWindow mainWindow)
        {
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
                backupSource.Name = folderInfo.Name;
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
                backupSource.Name = folderInfo.Name;
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
            BackupPlan.Sources = m_backupSources.ToList();
            Console.WriteLine(BackupPlan.Name);
            m_mainWindow.SavePlans();
            this.Close();
        }

        
    }
}
