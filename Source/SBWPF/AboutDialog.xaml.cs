using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaktionslogik für AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            versionLabel.Text = "Version " + version.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var appPath = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            var licencePath = System.IO.Path.Combine(appPath, "Legal");

            Process.Start("explorer.exe", licencePath);
        }
    }
}
