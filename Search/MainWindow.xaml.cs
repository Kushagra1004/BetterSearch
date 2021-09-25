using Search.Business;
using static Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;

namespace Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FileInfo> allfiles;
        public ISearchFiles searchFiles;
        public String searchTextG;
        public Boolean isPathSelectedG;
        public Boolean isProcessed = true;
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
                Utils.logText("starting ScanDirectoriesAsync");
                allfiles = searchFiles.GetFiles();
                Utils.logText("scanning done");
                Thread findText = new Thread(new ThreadStart(FindTextInThread));
                findText.Start();
                Dispatcher.Invoke(() =>
                {
                    statusLabel.Text = allfiles.Count.ToString() + " Objects";
                    FileListName.ItemsSource = allfiles;
                    //SearchBtn.IsEnabled = true;
                    SearchText.IsEnabled = true;
                });
            });
            Utils.logText("end ScanDirectoriesAsync");
        }


        private  void SearchBtn_Click(object sender, RoutedEventArgs e)  // not required now
        {
            FindText();
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utils.logText("SearchText_TextChanged start");
            searchTextG = SearchText.Text;
            isPathSelectedG = Path.IsSelected;
            isProcessed = false;
            Utils.logText("SearchText_TextChanged end");
        }

        private void FindTextInThread()
        {
            Utils.logText("FindTextInThread initialize");
            while (true)
            {
                if (isProcessed)
                {
                    Thread.Sleep(5);
                    continue;
                }
                String searchText_ = searchTextG;
                Boolean isPathSelected_ = isPathSelectedG;
                Utils.logText("FindTextAsync start");
                Utils.logText("async search start");
                long startTime = DateTime.Now.Ticks;
                IEnumerable<FileInfo> listFiles = searchFiles.FindText(allfiles, searchText_, isPathSelected_);
                long endTime = DateTime.Now.Ticks;
                Utils.logText("async search end");
                Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
                Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));
                String totalItems_ = listFiles.Count().ToString();
                Dispatcher.Invoke(() =>
                {
                    Utils.logText("updating ui");
                    if (isPathSelected_)
                    {
                        FileListPath.ItemsSource = listFiles;
                    }
                    else
                    {
                        Utils.logText("updating ui2");
                        FileListName.ItemsSource = listFiles;
                        Utils.logText("updating ui3");
                    }
                     lblTotalFilter.Text = "Total files found for " + searchText_ + ": ";

                    lblTotalFilesFoundFilter.Text = totalItems_; // totalItems_;
                    Utils.logText("updating ui end for " + searchText_);
                });
                if (searchTextG == searchText_ && isPathSelectedG == isPathSelected_)
                {
                    isProcessed = true; // rare race condition, fix later
                }
                Utils.logText("FindTextAsync end");
            }
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
