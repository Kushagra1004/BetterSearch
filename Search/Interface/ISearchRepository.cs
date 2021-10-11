using Search.Model;
using System.Collections.Generic;
using System.IO;

namespace Search.Interface
{
    public interface ISearchRepository
    {
        SearchNode GetAllTree();
        SearchNode GetAllTreeFake();
        List<FileModel> GetAllTreeFakeList();
        void ObjectChange(FileModel fileModel, FileOperations fileOperations, string oldName = "");
        List<ViewModel> Search(string searchText, SearchNode node, string currentPath, List<ViewModel> foundPaths = null);
    }
}
