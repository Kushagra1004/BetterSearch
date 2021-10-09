using Search.Model;
using System.Collections.Generic;
using System.IO;

namespace Search.Interface
{
    public interface ISearchRepository
    {
        SearchNode GetAllTree();
        void ObjectChange(FileSystemInfo fileSystemInfo, FileOperations fileOperations, string oldName = "");
        List<ViewModel> Search(string searchText, SearchNode node, string currentPath, List<ViewModel> foundPaths = null);
    }
}
