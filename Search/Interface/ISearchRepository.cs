using Search.Model;
using System.Collections.Generic;
using System.IO;

namespace Search.Interface
{
    public interface ISearchRepository
    {
        SearchNode GetAllTree();
        void ObjectChange(FileSystemInfo fileSystemInfo, FileOperations fileOperations);
        List<string> Search(string searchText, SearchNode tree, string currentPath, List<string> foundPaths = null);
    }
}
