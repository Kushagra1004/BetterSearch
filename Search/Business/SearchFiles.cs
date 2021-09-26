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

        public List<FileSystemInfo> GetFiles()
        {

            List<FileSystemInfo> _allFiles = new List<FileSystemInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                var startTime = DateTime.Now.Ticks;
                List<FileSystemInfo> _currentDirFiles = new List<FileSystemInfo>();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(drive.Name);
                    //DirectoryInfo di = new DirectoryInfo("E://");
                    Debug.WriteLine("Scanning: " + drive.Name);
                    _currentDirFiles = di.EnumerateFileSystemInfos("*", new EnumerationOptions
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
                string fileCount = _currentDirFiles.Where(x => x is FileInfo).Count().ToString();
                string folderCount = _currentDirFiles.Where(x => x is DirectoryInfo).Count().ToString();
                string totalCount = _currentDirFiles.Count().ToString();
                Debug.WriteLine("Scanned drive " + drive.Name + 
                                " with fileCount : " + fileCount +
                                " with folderCount : " + folderCount +
                                " with totalCount : " + totalCount +
                                " in ms " + ((endTime - startTime) / 10000) + "ms");
                //break;
            }
            _allFiles.Sort((x, y) => string.Compare(x.Name, y.Name));
            string fileCount2 = _allFiles.Where(x => x is FileInfo).Count().ToString();
            string folderCount2 = _allFiles.Where(x => x is DirectoryInfo).Count().ToString();
            string totalCount2 = _allFiles.Count().ToString();
            Debug.WriteLine("Scanned all " +
                                " with fileCount : " + fileCount2 +
                                " with folderCount : " + folderCount2 +
                                " with totalCount : " + totalCount2);
            return _allFiles;
        }

        public IEnumerable<FileSystemInfo> FindText(List<FileSystemInfo> allfiles, string searchText, bool searchInPath = false)
        {
            IEnumerable<FileSystemInfo> listFiles = new List<FileSystemInfo>();

            listFiles = searchInPath ? allfiles.Where(x => x.FullName.ToLower().Contains(searchText.ToLower())) : 
                                       allfiles.Where(x => x.Name.ToLower().Contains(searchText.ToLower()));

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
