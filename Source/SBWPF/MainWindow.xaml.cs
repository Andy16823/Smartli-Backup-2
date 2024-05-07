using Backuper;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SBWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String ApplicationPath { get; set; }
        public String BackupsPath { get; set; }
        public ObservableCollection<BackupPlan> BackupPlans { get; set; } = new ObservableCollection<BackupPlan>();
        public Dictionary<Guid, BackupWorker> Workers { get; set; }
        public bool IsInProgress { get; set; }

        /// <summary>
        /// Create a new instance of the backup tool
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Workers = new Dictionary<Guid, BackupWorker>();

            ApplicationPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            BackupsPath = System.IO.Path.Combine(ApplicationPath, "Backups");
            if (!Directory.Exists(BackupsPath))
            {
                Directory.CreateDirectory(BackupsPath);
            }
            LoadPlans();
            backupsListView.ItemsSource = BackupPlans;
        }

        /// <summary>
        /// Creates an new backup plan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createPlanButton_Click(object sender, RoutedEventArgs e)
        {
            var plan = new BackupPlan("My new backup", Schedule.Daily);
            BackupPlans.Add(plan);
            BackupPlanWindow newPlanDialog = new BackupPlanWindow(plan, this);
            newPlanDialog.ShowDialog();
        }

        /// <summary>
        /// Save the backup plans
        /// </summary>
        public void SavePlans()
        {
            var file = System.IO.Path.Combine(ApplicationPath, "plans.json");
            Backuper.Backuper.SerializePlans(BackupPlans, file);
        }

        /// <summary>
        /// Load the backup plans
        /// </summary>
        public void LoadPlans()
        {
            var file = System.IO.Path.Combine(ApplicationPath, "plans.json");
            if(File.Exists(file))
            {
                BackupPlans = Backuper.Backuper.DeserializeObservableCollection(file);
            }
        }

        /// <summary>
        /// Refresh the backup plans
        /// </summary>
        private void RefreshPlans()
        {
            backupsListView.ItemsSource = null;
            backupsListView.ItemsSource = BackupPlans;
        }

        /// <summary>
        /// Edit context menu event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editPlanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(backupsListView.SelectedItem != null)
            {
                var plan = (BackupPlan) backupsListView.SelectedItem;
                BackupPlanWindow window = new BackupPlanWindow(plan, this);
                window.Show();
            }
        }

        /// <summary>
        /// Create backup context menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createBackupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CreateBackup();
        }

        /// <summary>
        /// Create backup button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createBackupButton_Click(object sender, RoutedEventArgs e)
        {
            CreateBackup();
        }

        /// <summary>
        /// Creates an backup from the selected listview item
        /// </summary>
        public void CreateBackup()
        {
            if (backupsListView.SelectedItem != null)
            {
                //var plan = (BackupPlan)backupsListView.SelectedItem;

                //BackupWorker backupWorker = new BackupWorker(this, plan);
                //Workers.Add(backupWorker.Guid, backupWorker);
                //backupWorker.StartTimer(250);

                //HandyControl.Controls.Growl.InfoGlobal("Backup creation started!");
                //object[] args = { backupsListView.SelectedIndex, backupWorker.Guid };

                //Backuper.Backuper.CreateBackupAsync(plan, BackupsPath, (a) =>
                //{
                //    Console.WriteLine("Backup completed");
                //    this.Dispatcher.Invoke(new Action(() =>
                //    {
                //        // Get args
                //        int itemIndex = (int)a[0];
                //        Guid guid = (Guid)a[1];

                //        // Stop Worker
                //        Workers[guid].StopTimer();
                //        Workers.Remove(guid);

                //        // End backup process
                //        this.SavePlans();
                //        HandyControl.Controls.Growl.SuccessGlobal("Backup created");
                //        RefreshPlan(itemIndex);
                //    }));
                //}, args);

                var plan = (BackupPlan)backupsListView.SelectedItem;
                var index = backupsListView.SelectedIndex;
                CreateBackup(plan, index);
            }
            else
            {
                MessageBox.Show("Please select an Backup plan first");
            }
        }

        public void CreateBackup(BackupPlan plan, int index)
        {
            BackupWorker backupWorker = new BackupWorker(this, plan);
            Workers.Add(backupWorker.Guid, backupWorker);
            backupWorker.StartTimer(250);

            HandyControl.Controls.Growl.InfoGlobal("Backup creation started!");
            object[] args = { index, backupWorker.Guid };

            Backuper.Backuper.CreateBackupAsync(plan, BackupsPath, (a) =>
            {
                Console.WriteLine("Backup completed");
                this.Dispatcher.Invoke(new Action(() =>
                {
                    // Get args
                    int itemIndex = (int)a[0];
                    Guid guid = (Guid)a[1];

                    // Stop Worker
                    Workers[guid].StopTimer();
                    Workers.Remove(guid);

                    // End backup process
                    this.SavePlans();
                    HandyControl.Controls.Growl.SuccessGlobal("Backup created");
                    RefreshPlan(itemIndex);
                }));
            }, args);
        }

        /// <summary>
        /// Show backup folder event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showBackupFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (backupsListView.SelectedItem != null)
            {
                var plan = (BackupPlan)backupsListView.SelectedItem;
                var directory = Backuper.Backuper.GetPlanFolder(plan, BackupsPath);
                Process.Start("explorer.exe", @directory);
            }  
        }

        /// <summary>
        /// Delete plan menu event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deletePlanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (backupsListView.SelectedItem != null)
            {
                var plan = (BackupPlan)backupsListView.SelectedItem;
                if (System.Windows.MessageBox.Show("Are you sure you want delete the " + plan.Name + " backup plan?", "Smart Backup", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Backuper.Backuper.DeleteBackupDirectory(plan, BackupsPath);
                    this.BackupPlans.Remove(plan);
                    this.SavePlans();
                }
            }
        }

        /// <summary>
        /// Restore backup menu event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restorBackupFromPlanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowRestoreBackupDialog();
        }

        /// <summary>
        /// Restore button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRestoreBackupDialog();
        }

        /// <summary>
        /// Shows the restore backup screen
        /// </summary>
        public void ShowRestoreBackupDialog()
        {
            if (backupsListView.SelectedItem != null)
            {
                var plan = (BackupPlan)backupsListView.SelectedItem;
                RestoreBackupDialog restoreBackup = new RestoreBackupDialog(plan, this);
                restoreBackup.Show();
            }
            else
            {
                MessageBox.Show("Please select an Backup plan first");
            }
        }

        /// <summary>
        /// Sends an info growl
        /// </summary>
        /// <param name="message"></param>
        public void SendInfoGrowl(String message)
        {
            HandyControl.Controls.Growl.InfoGlobal(message);
        }

        /// <summary>
        /// Sends an success growl
        /// </summary>
        /// <param name="message"></param>
        public void SendSuccessGrowl(String message)
        {
            HandyControl.Controls.Growl.SuccessGlobal(message);
        }

        public void SendErrorGrowl(String message)
        {
            HandyControl.Controls.Growl.ErrorGlobal(message);
        }

        /// <summary>
        /// Shows the progress bar from a backup plan
        /// </summary>
        /// <param name="plan"></param>
        public void ShowPlanProgress(BackupPlan plan)
        {
            var selectedListViewItem = backupsListView.ItemContainerGenerator.ContainerFromItem(plan) as ListViewItem;
            ShowPlanProgress(selectedListViewItem);
        }

        /// <summary>
        /// Shows the progress bar from a backup plan at the given index
        /// </summary>
        /// <param name="index"></param>
        public void ShowPlanProgress(int index)
        {
            var selectedListViewItem = backupsListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            ShowPlanProgress(selectedListViewItem);
        }

        /// <summary>
        /// Shows the progress bar for a given listview item
        /// </summary>
        /// <param name="item"></param>
        public void ShowPlanProgress(ListViewItem item)
        {
            if (item != null)
            {
                SVGImage.SVG.SVGImage image = FindVisualChild<SVGImage.SVG.SVGImage>(item);
                image.Visibility = Visibility.Collapsed;
                HandyControl.Controls.CircleProgressBar circleProgressBar = FindVisualChild<HandyControl.Controls.CircleProgressBar>(item);
                circleProgressBar.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Hides the progres bar from a backup plan
        /// </summary>
        /// <param name="plan"></param>
        public void HidePlanProgress(BackupPlan plan)
        {
            var selectedListViewItem = backupsListView.ItemContainerGenerator.ContainerFromItem(plan) as ListViewItem;
            HidePlanProgress(selectedListViewItem);
        }

        /// <summary>
        /// Hides the progress bar from a backup plan at the given index
        /// </summary>
        /// <param name="index"></param>
        public void HidePlanProgress(int index)
        {
            var selectedListViewItem = backupsListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            HidePlanProgress(selectedListViewItem);
        }

        /// <summary>
        /// Hides the progress bar from a given livtview item
        /// </summary>
        /// <param name="item"></param>
        public void HidePlanProgress(ListViewItem item)
        {
            if (item != null)
            {
                SVGImage.SVG.SVGImage image = FindVisualChild<SVGImage.SVG.SVGImage>(item);
                image.Visibility = Visibility.Visible;
                HandyControl.Controls.CircleProgressBar circleProgressBar = FindVisualChild<HandyControl.Controls.CircleProgressBar>(item);
                circleProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Filter the backup list (not supported yet)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(backupsListView.ItemsSource);
            view.Filter = item =>
            {
                var plan = (BackupPlan)item;
                if(plan != null)
                {
                    if (String.IsNullOrWhiteSpace(searchBar.Text))
                    {
                        return true;
                    }

                    if (plan.Name is string str)
                    {
                        return str.IndexOf(searchBar.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                }
                return false;
            };
        }

        /// <summary>
        /// Check for backup button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckForBackups();
        }

        /// <summary>
        /// Checks the backup if scheduled
        /// </summary>
        public void CheckForBackups()
        {
            bool scheduledBackups = false;
            Backuper.Backuper.CheckForDueBackups(BackupPlans, (plan, args) =>
            {
                if(plan.BackupRequired)
                {
                    scheduledBackups = true;
                }

                this.Dispatcher.InvokeAsync(() =>
                {
                    if(plan.BackupRequired)
                    {
                        ChangeItemImage(plan, "/Images/exclamation-triangle-fill.svg");
                    }
                    else
                    {
                        ChangeItemImage(plan, "/Images/box-fill.svg");
                    }
                });
            });

            if(scheduledBackups)
            {
                if(MessageBox.Show("At least one backup plan is expired. Do you want to create this backups now?", "Expired Backups", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    CreateBackups(Backuper.Backuper.GetExpiredPlans(BackupPlans.ToList()));
                }
            }

        }

        /// <summary>
        /// Create an bulk of backups
        /// </summary>
        /// <param name="backupPlans"></param>
        public void CreateBackups(List<BackupPlan> backupPlans)
        {
            foreach (var item in backupPlans)
            {
                var index = BackupPlans.IndexOf(item);
                CreateBackup(item, index);
            }
        }

        /// <summary>
        /// Refresh an single plan
        /// </summary>
        /// <param name="plan"></param>
        public void RefreshPlan(BackupPlan plan)
        {
            var index = BackupPlans.IndexOf(plan);
            RefreshPlan(index);
        }

        /// <summary>
        /// Refreshs an single plan at the given index
        /// </summary>
        /// <param name="index"></param>
        public void RefreshPlan(int index)
        {
            var plan = BackupPlans[index];
            BackupPlans[index] = null;
            BackupPlans[index] = plan;
        }

        /// <summary>
        /// Change the image from a backup plan
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="source"></param>
        public void ChangeItemImage(BackupPlan plan, String source)
        {
            var index = BackupPlans.IndexOf(plan);
            ChangeItemImage(index, source);
        }

        /// <summary>
        /// Change the image from a backup plan at the given index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="source"></param>
        public void ChangeItemImage(int index, String source)
        {
            var selectedListViewItem = backupsListView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
            if (selectedListViewItem != null)
            {
                SVGImage.SVG.SVGImage image = FindVisualChild<SVGImage.SVG.SVGImage>(selectedListViewItem);
                image.Source = source;
            }
        }

        /// <summary>
        /// Starts an main progress
        /// </summary>
        public void StartProgress()
        {
            this.IsInProgress = true;
            this.progressBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Stops an main progress
        /// </summary>
        public void StopProgress()
        {
            this.IsInProgress = false;
            this.progressBar.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Find an visual child in an collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) 
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if(child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfchild = FindVisualChild<T>(child);
                    if(childOfchild != null)
                    {
                        return childOfchild;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Opens the smartli website
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smartliButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://smartli.me");
        }

        /// <summary>
        /// Shows the about dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog();
        }

        /// <summary>
        /// Export plan menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPlanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(this.backupsListView.SelectedItem != null)
            {
                var item = (BackupPlan) this.backupsListView.SelectedItem;
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".sba";
                if(saveFileDialog.ShowDialog() == true)
                {
                    StartProgress();
                    object[] args = { saveFileDialog.FileName };
                    SendInfoGrowl("Starting the export from " + item.Name);
                    Backuper.Backuper.ExportBackupAsync(item, BackupsPath, saveFileDialog.FileName, (a) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            SendSuccessGrowl("Export done!");
                            StopProgress();

                            FileInfo fileInfo = new FileInfo((String)a[0]);
                            Process.Start("explorer.exe", fileInfo.DirectoryName);
                        });
                    }, args);
                }
            }
        }

        /// <summary>
        /// Export plan secured menu item event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPlanSecuredMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.backupsListView.SelectedItem != null)
            {
                var item = (BackupPlan)this.backupsListView.SelectedItem;
                ExportSecuredDialog exportSecuredDialog = new ExportSecuredDialog(item, this);
                exportSecuredDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Import menu item event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importPlanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Smartli Backup Archive (*.sba)|*.sba|Encrypted Smartli Backup Archive (*.esba)|*.esba";
            if(openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                if(fileInfo.Extension.Equals(".esba"))
                {
                    ImportSecruredDialog importSecured = new ImportSecruredDialog(openFileDialog.FileName, this);
                    importSecured.ShowDialog();
                }
                else
                {
                    StartProgress();
                    SendInfoGrowl("Starting the import from the backup archive");
                    Backuper.Backuper.ImportFromArchiveAsync(openFileDialog.FileName, BackupsPath, (plan, args) =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            StopProgress();
                            if (plan != null)
                            {
                                SendSuccessGrowl("Import done");
                                if (!this.BackupPlans.Contains(plan))
                                {
                                    this.BackupPlans.Add(plan);
                                }
                                this.SavePlans();
                            }
                            else
                            {
                                SendErrorGrowl("Something went wrong with the import");
                            }
                        });
                    });
                }
            }
        }
    }
}