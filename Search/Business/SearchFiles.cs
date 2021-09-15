using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Search.Business
{
    public class SearchFiles : ISearchFiles
    {

        public List<FileInfo> GetFiles()
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
                long endTime = DateTime.Now.Ticks;
                Debug.WriteLine("Scanned drive " + drive.Name + " with fileCount : " + _currentDirFiles.Count.ToString() +
                                " in ms " + ((endTime - startTime) / 10000) + "ms");
            }
            _allFiles.Sort((x, y) => string.Compare(x.Name, y.Name));
            return _allFiles;
        }

        public IEnumerable<FileInfo> FindText(List<FileInfo> allfiles, string searchText, bool searchInPath = false)
        {
            IEnumerable<FileInfo> listFiles = new List<FileInfo>();

            listFiles = searchInPath ? allfiles.Where(x => x.FullName.Contains(searchText)) : allfiles.Where(x => x.Name.Contains(searchText));

            return listFiles;
        }

        public void OpenFileInExplorer(string path, bool isOpenPath = false)
        {
            string _pathToOpen = path;
            if (isOpenPath)
            {
                int dirIndex = path.LastIndexOf(@"\");
                _pathToOpen = path.Substring(0, dirIndex);
            }
            _ = Process.Start("explorer.exe", _pathToOpen);
        }
    }
}
