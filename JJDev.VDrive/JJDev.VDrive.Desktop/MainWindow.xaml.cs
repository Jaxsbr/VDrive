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

namespace JJDev.VDrive.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MountButton.Click += MountButton_Click;
            DismountButton.Click += DismountButton_Click;
            TestButton.Click += TestButton_Click;
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var result = DriveMaster.IsDriveInUse("z:");
            MessageBox.Show($"Z  in use: {result}");
        }

        private void DismountButton_Click(object sender, RoutedEventArgs e)
        {
            DriveMaster.Dismount("z:");
            MessageBox.Show("Z  Dismounted!");
        }

        private void MountButton_Click(object sender, RoutedEventArgs e)
        {
            DriveMaster.Mount("z:", "C:\test");
            MessageBox.Show("Z  Mounted!");
        }
    }
}
