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
using Search.Interface;
using Search.Model;

namespace Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<FileSystemInfo> allfiles;
        public SearchNode node;
        public ISearchFiles searchFiles;
        //public IWatchFileChanges watcher;
        public IWatchTreeChanges watcher;
        public ISearchRepository searchRepository;
        public IMapper mapper;
        public string searchTextG;
        public bool isPathSelectedG;
        public bool isProcessed = true;
        public MainWindow()
        {
            searchFiles = new SearchFiles();
            watcher = new WatchFileChanges();
            searchRepository = new SearchRepository();
            mapper = new Mapper();
            InitializeComponent();
            ScanDirectoriesAsync();

            //WatchChanges();
        }

        


        private async void ScanDirectoriesAsync()
        {
            allfiles = new List<FileSystemInfo>();
            await Task.Run(() =>
            {
                logText("starting ScanDirectoriesAsync");
                //allfiles = searchFiles.GetFiles();
                node = searchRepository.GetAllTree();
                var vmAllFiles = mapper.ConvertToList(node);
                logText("scanning done");
                Thread findText = new Thread(new ThreadStart(FindTextInThread));
                findText.Start();
                Dispatcher.Invoke(() =>
                {
                    statusLabel.Text = vmAllFiles.Count.ToString() + " Objects";
                    FileListName.ItemsSource = vmAllFiles;
                    //SearchBtn.IsEnabled = true;
                    SearchText.IsEnabled = true;

                    //watcher.WatchChanges(allfiles);
                    watcher.WatchChanges(node);
                });
            });
            logText("end ScanDirectoriesAsync");
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
            logText("FindTextInThread initialize");
            while (true)
            {
                if (isProcessed)
                {
                    Thread.Sleep(5);
                    continue;
                }
                string searchText_ = searchTextG;
                bool isPathSelected_ = isPathSelectedG;
                logText("FindTextAsync start");
                logText("async search start");
                long startTime = DateTime.Now.Ticks;
                //IEnumerable<FileSystemInfo> listFiles = searchFiles.FindText(allfiles, searchText_, isPathSelected_);

                var listFiles = searchRepository.Search(searchText_, node, "");
                //var listFiles = mapper.ConvertToList(fileList);
                long endTime = DateTime.Now.Ticks;
                logText("async search end");
                Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
                Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));
                string totalItems_ = listFiles.Count().ToString();
                Dispatcher.Invoke(() =>
                {
                    logText("updating ui");
                    if (isPathSelected_)
                    {
                        FileListPath.ItemsSource = listFiles;
                    }
                    else
                    {
                        logText("updating ui2");
                        FileListName.ItemsSource = listFiles;
                        logText("updating ui3");
                    }
                    lblTotalFilter.Text = "Total files found for " + searchText_ + ": ";

                    lblTotalFilesFoundFilter.Text = totalItems_; // totalItems_;
                    logText("updating ui end for " + searchText_);
                });
                if (searchTextG == searchText_ && isPathSelectedG == isPathSelected_)
                {
                    isProcessed = true; // rare race condition, fix later
                }
                logText("FindTextAsync end");
            }
        }

        private void FindText()
        {
            long startTime = DateTime.Now.Ticks;
            IEnumerable<FileSystemInfo> listFiles = searchFiles.FindText(allfiles, SearchText.Text, Path.IsSelected);
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
            //FindText();
            searchTextG = SearchText.Text;
            isPathSelectedG = Path.IsSelected;
            isProcessed = false;
        }


        private void GetSelectedPath_Click(object sender, MouseButtonEventArgs e)
        {
            ListView listViewToBePopulated = Path.IsSelected ? FileListPath : FileListName;

            string filepath = ((ViewModel)listViewToBePopulated.SelectedItems[0]).Path;
            
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
