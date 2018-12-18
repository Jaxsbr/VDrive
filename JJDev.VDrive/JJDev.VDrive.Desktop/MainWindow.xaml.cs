using JJDev.VDrive.Core;
using JJDev.VDrive.Core.Bundling;
using Microsoft.Win32;
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
        private readonly string _letter = "K";


        public MainWindow()
        {
            InitializeComponent();
            MountButton.Click += MountButton_Click;
            DismountButton.Click += DismountButton_Click;
            TestButton.Click += TestButton_Click;
            EncodeButton.Click += EncodeButton_Click;
            DecodeButton.Click += DecodeButton_Click;
        }

        private async void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            var sut = new BundleEngine();
            var cipher = CipherKeys.GetCipher();
            var openFileDialog = new OpenFileDialog()
            {
              Filter = "Ecoded File |*.enc",
              Title = "Selected encoded file"
            };
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
              Description = "Select a output path to decode data to"
            };
            
            var openResult = openFileDialog.ShowDialog();
            if (!(bool)openResult) { return; }

            var folderResult = folderBrowserDialog.ShowDialog();
            if (folderResult != System.Windows.Forms.DialogResult.OK) { return; }

            await sut.Decompress(openFileDialog.FileName, folderBrowserDialog.SelectedPath, cipher);

            MessageBox.Show("Data decoded successfully!");
          }

        private async void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
          var sut = new BundleEngine();
          var cipher = CipherKeys.GetCipher();
          var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
          {
            Description = "Select a path to the source data"
          };
          var saveFileDialog = new SaveFileDialog()
          {
            Filter = "Ecoded File |*.enc",
            Title = "Select and output path for encoded data"
          };          

          var folderResult = folderBrowserDialog.ShowDialog();
          if (folderResult != System.Windows.Forms.DialogResult.OK) { return; }

          var saveResult = saveFileDialog.ShowDialog();
          if (!(bool)saveResult) { return; }


          // TODO:
          // Compress async
          // Progress updates

          var result = await sut.Compress(folderBrowserDialog.SelectedPath, saveFileDialog.FileName, cipher);
            
          if (System.IO.File.Exists(saveFileDialog.FileName)) { MessageBox.Show("Data encoded successfully!"); }
          else { MessageBox.Show("Data encoding failed!"); }
    }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            var result = DriveMaster.IsDriveInUse(_letter);
            MessageBox.Show($"{_letter}  in use: {result}");
        }        


        private void DismountButton_Click(object sender, RoutedEventArgs e)
        {
            DriveMaster.Dismount(_letter);
            MessageBox.Show($"{_letter}  Dismounted!");
        }

        private void MountButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
              Description = "Select a physical path to map your virtual drive to"
            };
            var folderResult = folderBrowserDialog.ShowDialog();
            if (folderResult != System.Windows.Forms.DialogResult.OK) { return; }
            var unc = UNCHelper.GetUNCPathFromPath(folderBrowserDialog.SelectedPath);
            DriveMaster.Mount(_letter, unc);
            MessageBox.Show($"{_letter}  Mounted!");
        }
    }
}
