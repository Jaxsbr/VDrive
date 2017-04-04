using JJDev.VDrive.Core;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JJDev.VDrive.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool loading = true;
        private string basePath = @"C:\Virtual Drives";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DriveActionButton.Click += DriveActionButton_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDrives();            
        }

        private void DriveActionButton_Click(object sender, RoutedEventArgs e)
        {            
            var drive = DriveSelectionComboBox.SelectedItem.ToString();
            var path = ValidateDirectory(basePath, drive.Replace(":", string.Empty));

            if (DriveMaster.DriveValid(drive))
            {                
                DriveMaster.UnMapDrive(drive);

                // TODO:
                // Hanle files in use.
                // Encrypt directory into unreadable file
                return;
            }

            DriveMaster.MapDrive(drive, path);
        }


        public void LoadDrives()
        {
            var availableDrives = DriveMaster.GenerateDriveLetters();
            var actualDrives = DriveMaster.AvailableDrives(false);

            for (int i = 0; i < availableDrives.Count; i++)
            {
                if (actualDrives.Contains(availableDrives[i]))
                {
                    availableDrives.Remove(availableDrives[i]);
                    i--;
                }
            }

            availableDrives.ForEach(d => DriveSelectionComboBox.Items.Add(d));
        }

        private string ValidateDirectory(string basePath, string directoryName)
        {
            var path = System.IO.Path.Combine(basePath, directoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
