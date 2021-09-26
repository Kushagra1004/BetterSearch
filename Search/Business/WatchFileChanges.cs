using Search.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Search.Business
{
    public class WatchFileChanges: IWatchFileChanges
    {
        public void WatchChanges(List<FileInfo> allfiles)
        {
            foreach(var drive in DriveInfo.GetDrives())
            {
                try
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(drive.Name)
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
        private void Watcher_Changed(object sender, FileSystemEventArgs e, List<FileInfo> allfiles)
        {
            var oldFileInfo = allfiles.FirstOrDefault(x => x.FullName == e.FullPath);
            allfiles.Remove(oldFileInfo);
            var newfileInfo = new FileInfo(e.FullPath);
            allfiles.Add(newfileInfo);
            Utils.logText(string.Format("Change: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Created(object sender, FileSystemEventArgs e, List<FileInfo> allfiles)
        {
            var newfileInfo = new FileInfo(e.FullPath);
            allfiles.Add(newfileInfo);
            Utils.logText(string.Format("Created: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Deleted(object sender, FileSystemEventArgs e, List<FileInfo> allfiles)
        {
            var oldFileInfo = allfiles.FirstOrDefault(x => x.FullName == e.FullPath);
            allfiles.Remove(oldFileInfo);
            Utils.logText(string.Format("Deleted: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
        private void Watcher_Renamed(object sender, RenamedEventArgs e, List<FileInfo> allfiles)
        {
            var oldFileInfo = allfiles.FirstOrDefault(x => x.FullName == e.OldFullPath);
            allfiles.Remove(oldFileInfo);
            var newfileInfo = new FileInfo(e.FullPath);
            allfiles.Add(newfileInfo);
            Utils.logText(string.Format("Renamed: {0}, File: {1}", e.ChangeType, e.FullPath));
        }
    }
}
