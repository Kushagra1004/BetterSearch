using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search.Interface
{
    public interface IWatchFileChanges
    {
        void WatchChanges(List<FileSystemInfo> allfiles);
    }
}
