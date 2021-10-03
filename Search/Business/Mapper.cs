using Search.Interface;
using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search.Business
{
    public class Mapper : IMapper
    {
        private readonly SearchNode _parentNode;
        public ISearchRepository searchRepository;

        public Mapper()
        {
            searchRepository = new SearchRepository();
            _parentNode = SearchNode.GetTree();
        }
        public SearchNode ConvertToTree(List<FileSystemInfo> fileList)
        {
            fileList.ForEach(x => searchRepository.ObjectChange(x, FileOperations.Add));
            return _parentNode;
        }

        public List<ViewModel> ConvertToList(SearchNode node, List<ViewModel> viewModels = null, string currentPath = "")
        {
            if (viewModels == null)
            {
                viewModels = new List<ViewModel>();
            }
            currentPath += node.Name + "/";
            ViewModel vm = new ViewModel
            {
                Name = node.Name,
                Path = currentPath,
            };
            viewModels.Add(vm);

            node.FileList.ForEach(x =>
            {
                ViewModel vm = new ViewModel
                {
                    Name = x.Name,
                    Path = currentPath + x.Name,
                    Size = x.Size
                };
                viewModels.Add(vm);
            });

            node.DirectoryList.ForEach(x => { viewModels = ConvertToList(x, viewModels, currentPath); });

            return viewModels;

        }

        public List<ViewModel> ConvertToList(List<string> paths)
        {
            var viewModels = new List<ViewModel>();
            foreach (var path in paths)
            {

                FileAttributes attr = File.GetAttributes(path);

                //detect whether its a directory or file
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    ViewModel vm = new ViewModel
                    {
                        Name = di.Name,
                        Path = di.FullName
                    };
                    viewModels.Add(vm);
                }
                else
                {
                    FileInfo fi = new FileInfo(path);
                    ViewModel vm = new ViewModel
                    {
                        Name = fi.Name,
                        Path = fi.FullName,
                        Size = fi.Length
                    };
                    viewModels.Add(vm);
                }
            }

            return viewModels;
        }
    }
}
