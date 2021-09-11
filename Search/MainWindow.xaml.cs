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
        public List<string> allfolders;

        public MainWindow()
        {
            InitializeComponent();
            var startTime = DateTime.Now.Ticks;
            allfolders = new List<string>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    allfolders.AddRange(Directory.EnumerateFiles(drive.Name, "*", new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = true
                    }).ToList());
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

            IEnumerable<string> listFiles = allfolders.Where(x => x.Contains(SearchText.Text));
            var endTime = DateTime.Now.Ticks;

            List<FileModel> files = listFiles.Select(x => new FileModel(x)).ToList();

            FileList.ItemsSource = files;
            Debug.WriteLine("Time Taken to filter: " + ((endTime - startTime) / 10000) + "ms");
            Debug.WriteLine("Time Taken to filter ticks Diff: " + (endTime - startTime));

            lblTotalFilter.Text = "Total files found: ";
            lblTotalFilesFoundFilter.Text = files.Count.ToString();
            GC.Collect();
        }

        private void GetSelectedPath_Click(object sender, MouseButtonEventArgs e)
        {
            string filepath = ((FileModel)FileList.SelectedItems[0]).FilePath;
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
                catch (Exception exc)
                {
                    _ = MessageBox.Show("Unable to open path - " + directoryPath);
                }
            }
        }


    }
    public class FileModel
    {
        public FileModel(string path)
        {
            FilePath = path;
            int dirIndex = FilePath.LastIndexOf(@"\");
            FileName = FilePath[(dirIndex + 1)..];
        }
        public string FilePath { get; set; }

        public string FileName { get; set; }

    }
}
