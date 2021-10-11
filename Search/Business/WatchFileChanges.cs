using Search.Interface;
using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Search.Business
{
    public class WatchFileChanges : IWatchFileChanges, IWatchTreeChanges
    {
        public ISearchRepository searchRepository = new SearchRepository();
        public void WatchChanges(List<FileSystemInfo> allfiles)
        {
            string[] a = { @"C:\Publish", @"C:\ImageRight" };
            //foreach (var drive in DriveInfo.GetDrives())
            foreach (var drive in a)
            {
                try
                {
                    
                    //FileSystemWatcher watcher = new FileSystemWatcher(drive.Name)
                    FileSystemWatcher watcher = new FileSystemWatcher(drive)
                    {
                        IncludeSubdirectories = true
                    };
                    watcher.Changed += new FileSystemEventHandler((s, e) => Watcher_Changed(s, e, allfiles));
                    watcher.Created += new FileSystemEventHandler((s, e) => Watcher_Created(s, e, allfiles));
                    watcher.Deleted += new FileSystemEventHandler((s, e) => Watcher_Deleted(s, e, allfiles));
                    watcher.Renamed += new RenamedEventHandler((s, e) => Watcher_Renamed(s, e, allfiles));
                    watcher.Filter = "*.*";
                    watcher.NotifyFilter = NotifyFilters.LastAccess |
                                 NotifyFilters.LastWrite |
                                 NotifyFilters.FileName |
                                 NotifyFilters.DirectoryName;
                    watcher.EnableRaisingEvents = true;
                }
                catch(Exception ex)
                {
                    Utils.logText(ex.StackTrace);
                }
            }
            

        }
        private void Watcher_Changed(object sender, FileSystemEventArgs e, List<FileSystemInfo> allfiles)
        {
            var oldFileSystemInfo = allfiles.FirstOrDefault(x => x.FullName == e.FullPath);
            allfiles.Remove(oldFileSystemInfo);
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            
            allfiles.Add(newFileSystemInfo);
            Utils.logText(string.Format("Change: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Created(object sender, FileSystemEventArgs e, List<FileSystemInfo> allfiles)
        {
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            allfiles.Add(newFileSystemInfo);
            Utils.logText(string.Format("Created: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Deleted(object sender, FileSystemEventArgs e, List<FileSystemInfo> allfiles)
        {
            var oldFileSystemInfo = allfiles.FirstOrDefault(x => x.FullName == e.FullPath);
            allfiles.Remove(oldFileSystemInfo);
            Utils.logText(string.Format("Deleted: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Renamed(object sender, RenamedEventArgs e, List<FileSystemInfo> allfiles)
        {
            var oldFileSystemInfo = allfiles.FirstOrDefault(x => x.FullName == e.OldFullPath);
            allfiles.Remove(oldFileSystemInfo);
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            allfiles.Add(newFileSystemInfo);
            Utils.logText(string.Format("Renamed: {0}, File: {1}", e.ChangeType, e.FullPath));
        }



        public void WatchChanges(SearchNode node)
        {
            string[] a = { @"C:\Publish", @"C:\ImageRight" };
            //foreach (var drive in DriveInfo.GetDrives())
            foreach (var drive in a)
            {
                try
                {

                    //FileSystemWatcher watcher = new FileSystemWatcher(drive.Name)
                    FileSystemWatcher watcher = new FileSystemWatcher(drive)
                    {
                        IncludeSubdirectories = true
                    };
                    watcher.Changed += new FileSystemEventHandler((s, e) => Watcher_Changed(s, e, node));
                    watcher.Created += new FileSystemEventHandler((s, e) => Watcher_Created(s, e, node));
                    watcher.Deleted += new FileSystemEventHandler((s, e) => Watcher_Deleted(s, e, node));
                    watcher.Renamed += new RenamedEventHandler((s, e) => Watcher_Renamed(s, e, node));
                    watcher.Filter = "*.*";
                    watcher.NotifyFilter = NotifyFilters.LastAccess |
                                 NotifyFilters.LastWrite |
                                 NotifyFilters.FileName |
                                 NotifyFilters.DirectoryName;
                    watcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    Utils.logText(ex.StackTrace);
                }
            }


        }
        private void Watcher_Changed(object sender, FileSystemEventArgs e, SearchNode node)
        {
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            searchRepository.ObjectChange(new FileModel(newFileSystemInfo), FileOperations.Remove);
            searchRepository.ObjectChange(new FileModel(newFileSystemInfo), FileOperations.Add);
            Utils.logText(string.Format("Change: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Created(object sender, FileSystemEventArgs e, SearchNode node)
        {
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            searchRepository.ObjectChange(new FileModel(newFileSystemInfo), FileOperations.Add);
        }
        private void Watcher_Deleted(object sender, FileSystemEventArgs e, SearchNode node)
        {
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo oldFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var oldFileInfo = new DirectoryInfo(e.FullPath);
                oldFileSystemInfo = (FileSystemInfo)oldFileInfo;
            }
            else
            {
                var oldFileInfo = new FileInfo(e.FullPath);
                oldFileSystemInfo = (FileSystemInfo)oldFileInfo;
            }
            searchRepository.ObjectChange(new FileModel(oldFileSystemInfo), FileOperations.Remove);
            Utils.logText(string.Format("Deleted: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Renamed(object sender, RenamedEventArgs e, SearchNode node)
        {
            var fileAttr = File.GetAttributes(e.FullPath);
            FileSystemInfo newFileSystemInfo;
            if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var newFileInfo = new DirectoryInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            else
            {
                var newFileInfo = new FileInfo(e.FullPath);
                newFileSystemInfo = (FileSystemInfo)newFileInfo;
            }
            searchRepository.ObjectChange(new FileModel(newFileSystemInfo), FileOperations.Rename, e.OldName);
            Utils.logText(string.Format("Renamed: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
    }
}
