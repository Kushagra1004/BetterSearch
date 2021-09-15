using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search
{
    public interface ISearchFiles
    {
        List<FileInfo> GetFiles();
        IEnumerable<FileInfo> FindText(List<FileInfo> allfiles, string searchText, bool searchInPath = false);
        void OpenFileInExplorer(string path, bool isOpenPath = false);
    }
}
