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
            var startTime = DateTime.Now.Ticks;
            List<string> allfolders = new List<string>();
            allfiles = new List<FileInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    allfolders.AddRange(Directory.EnumerateFiles(drive.Name, "*", new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = true
                    }).ToList());

                    foreach (var file in allfolders)
                    {
                        allfiles.Add(new FileInfo(file));
                        // Do something with the Folder or just add them to a list via nameoflist.add();
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            var endTime = DateTime.Now.Ticks;
            Debug.WriteLine("Time Taken to Init: " + ((endTime - startTime) / 10000) + "ms");
            lblTotalFilesFound.Text = allfolders.Count.ToString();
            GC.Collect();
            //var arraylength = allfolders.Sum(x => x.Length);

            //Debug.WriteLine("Total file character lengths: " + arraylength);
            //long size = 0;
            //object o = allfolders;
            //using (Stream s = new MemoryStream())
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    formatter.Serialize(s, o);
            //    size = s.Length;
            //}
            //Debug.WriteLine("Total files size: " + size);
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            var startTime = DateTime.Now.Ticks;

            IEnumerable<FileInfo> listFiles1 = allfiles.Where(x => x.FullName.Contains(SearchText.Text));

            //IEnumerable<string> listFiles = allfolders.Where(x => x.Contains(SearchText.Text));
            var endTime = DateTime.Now.Ticks;

            //List<FileModel> files = listFiles.Select(x => new FileModel(x)).ToList();

            FileList.ItemsSource = listFiles1;
            Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
            Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));

            lblTotalFilter.Text = "Total files found: ";
            lblTotalFilesFoundFilter.Text = listFiles1.Count().ToString();
            GC.Collect();
        }

        private void GetSelectedPath_Click(object sender, MouseButtonEventArgs e)
        {
            string filepath = ((FileInfo)FileList.SelectedItems[0]).FullName;
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
