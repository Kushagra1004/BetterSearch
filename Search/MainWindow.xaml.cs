using Search.Business;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FileInfo> allfiles;
        public ISearchFiles searchFiles;

        public MainWindow()
        {
            searchFiles = new SearchFiles();
            InitializeComponent();
            ScanDirectoriesAsync();
        }


        private async void ScanDirectoriesAsync()
        {
            allfiles = new List<FileInfo>();
            await Task.Run(() =>
            {
                allfiles = searchFiles.GetFiles();
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

        private void FindText()
        {
            long startTime = DateTime.Now.Ticks;
            IEnumerable<FileInfo> listFiles = searchFiles.FindText(allfiles, SearchText.Text, Path.IsSelected);
            long endTime = DateTime.Now.Ticks;

            Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
            Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));
            if (Path.IsSelected)
            {
                FileListPath.ItemsSource = listFiles;
            }
            else
            {
                FileListName.ItemsSource = listFiles;
            }
            lblTotalFilter.Text = "Total files found: ";
            lblTotalFilesFoundFilter.Text = listFiles.Count().ToString();
            GC.Collect();
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            FindText();
        }


        private void GetSelectedPath_Click(object sender, MouseButtonEventArgs e)
        {
            ListView listViewToBePopulated = Path.IsSelected ? FileListPath: FileListName;

            string filepath = ((FileInfo)listViewToBePopulated.SelectedItems[0]).FullName;
            
            try
            {
                searchFiles.OpenFileInExplorer(filepath, Path.IsSelected);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Unable to open path - " + filepath);
            }
        }

       
    }
    
}
