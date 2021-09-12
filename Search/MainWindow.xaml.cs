using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public List<string> allfolders;
        public List<FileInfo> allfiles;

        public MainWindow()
        {
            InitializeComponent();
            ScanDirectoriesAsync();
        }

        private List<FileInfo> GetFiles()
        {
            
            List<FileInfo> _allFiles = new List<FileInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                var startTime = DateTime.Now.Ticks;
                List<FileInfo> _currentDirFiles = new List<FileInfo>();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(drive.Name);
                    Debug.WriteLine("Scanning: " + drive.Name);
                    _currentDirFiles = di.EnumerateFiles("*", new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = true
                    }).ToList();
                    _allFiles.AddRange(_currentDirFiles);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                var endTime = DateTime.Now.Ticks;
                Debug.WriteLine("Scanned drive " + drive.Name + " with fileCount : " + _currentDirFiles.Count.ToString() + 
                                " in ms " + ((endTime - startTime) / 10000) + "ms");
            }
            _allFiles.Sort((x, y) => string.Compare(x.Name, y.Name));            
            return _allFiles;
        }

        private async void ScanDirectoriesAsync()
        {
            allfiles = new List<FileInfo>();
            await Task.Run(() =>
            {
                allfiles = GetFiles();
                Dispatcher.Invoke(() =>
                {
                    statusLabel.Text = allfiles.Count.ToString() + " Objects";
                    FileListName.ItemsSource = allfiles;
                    //SearchBtn.IsEnabled = true;
                    SearchText.IsEnabled = true;
                });
            });
        }


        private void SearchBtn_Click(object sender, RoutedEventArgs e)  // not required now
        {
            FindText();
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            FindText();
        }

        private void FindText(bool searchInPath = false)
        {
            var startTime = DateTime.Now.Ticks;
            IEnumerable<FileInfo> listFiles = new List<FileInfo>();
            if (searchInPath)
            {
                listFiles = allfiles.Where(x => x.FullName.Contains(SearchText.Text));
                FileListPath.ItemsSource = listFiles;
            }
            else
            {
                listFiles = allfiles.Where(x => x.Name.Contains(SearchText.Text));
                FileListName.ItemsSource = listFiles;
            }


            var endTime = DateTime.Now.Ticks;
            
            Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
            Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));

            lblTotalFilter.Text = "Total files found: ";
            lblTotalFilesFoundFilter.Text = listFiles.Count().ToString();
            GC.Collect();
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Name.IsSelected)
            {
                FindText();
            }
            else if (Path.IsSelected)
            {
                FindText(true);
            }
        }


        private void GetSelectedPath_Click(object sender, MouseButtonEventArgs e)
        {
            string filepath;
            if (Name.IsSelected)
            {
                filepath = ((FileInfo)FileListName.SelectedItems[0]).FullName;
            }
            else
            {
                filepath = ((FileInfo)FileListPath.SelectedItems[0]).FullName;
            }
            int dirIndex = filepath.LastIndexOf(@"\");
            string directoryPath = filepath.Substring(0, dirIndex);
            try
            {
                _ = Process.Start("explorer.exe", filepath);
            }
            catch (Exception ex)
            {
                try
                {
                    _ = Process.Start("explorer.exe", directoryPath);
                }
                catch (Exception)
                {
                    _ = MessageBox.Show("Unable to open path - " + directoryPath);
                }
            }
        }

       
    }
    
}
