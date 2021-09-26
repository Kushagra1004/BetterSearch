using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search
{
    public interface ISearchFiles
    {
        List<FileSystemInfo> GetFiles();
        IEnumerable<FileSystemInfo> FindText(List<FileSystemInfo> allfiles, string searchText, bool searchInPath = false);
        void OpenFileInExplorer(string path, bool isOpenPath = false);

    }
}
