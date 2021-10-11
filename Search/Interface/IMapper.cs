using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search
{
    public interface IMapper
    {
        //SearchNode ConvertToTree(List<FileSystemInfo> fileList);
        List<ViewModel> ConvertToList(SearchNode node, List<ViewModel> viewModels = null, string currentPath = "");

        List<ViewModel> ConvertToList(List<string> paths);

    }
}
