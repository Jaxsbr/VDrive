using JJDev.VDrive.Core;
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

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DriveSelectionComboBox.SelectionChanged += DriveSelectionComboBox_SelectionChanged;     
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //TestDriveMap();
            //TestDriveUnMap();
            LoadDrives();

            loading = false;
        }

        private void DriveSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loading)
            {
                return;
            }

            
        }


        public void TestDriveMap()
        {
            var drives = DriveMaster.GenerateDriveLetters();
            DriveMaster.MapDrive(drives[24], @"C:\Virtual Drives");
        }

        public void TestDriveUnMap()
        {
            var drives = DriveMaster.GenerateDriveLetters();
            DriveMaster.UnMapDrive(drives[24]);
        }

        public void LoadDrives()
        {
            //var drives = DriveMaster.GenerateDriveLetters();
            var drives = DriveMaster.AvailableDrives(false);
            //var drives = DriveMaster.AvailableDrives(true);
            drives.ForEach(d => DriveSelectionComboBox.Items.Add(d));
        }


    }
}
