﻿using Search.Interface;
using Search.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Search.Business
{
    public class SearchRepository : ISearchRepository
    {
        private readonly SearchNode _parentNode;

        public SearchRepository()
        {
            _parentNode = SearchNode.GetTree();
        }

        public SearchNode GetAllTree()
        {
            SearchNode node = _parentNode;

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                var startTime = DateTime.Now.Ticks;
                List<FileSystemInfo> _currentDirFiles = new List<FileSystemInfo>();
                try
                {
                    DirectoryInfo di = new DirectoryInfo(drive.Name);
                    //DirectoryInfo di = new DirectoryInfo("E://");
                    Utils.logText("Scanning: " + drive.Name);
                    _currentDirFiles = di.EnumerateFileSystemInfos("*", new EnumerationOptions
                    {
                        IgnoreInaccessible = true,
                        RecurseSubdirectories = true
                    }).ToList();
                    _currentDirFiles.ForEach(x => ObjectChange(x, FileOperations.Add));
                }
                catch (Exception e)
                {
                    Utils.logText(e.StackTrace);
                }
                long endTime = DateTime.Now.Ticks;
                string fileCount = _currentDirFiles.Where(x => x is FileInfo).Count().ToString();
                string folderCount = _currentDirFiles.Where(x => x is DirectoryInfo).Count().ToString();
                string totalCount = _currentDirFiles.Count().ToString();
                Utils.logText("Scanned drive " + drive.Name +
                                " with fileCount : " + fileCount +
                                " with folderCount : " + folderCount +
                                " with totalCount : " + totalCount +
                                " in ms " + ((endTime - startTime) / 10000) + "ms");
                //break;
            }

            return node;
        }
        
        public void ObjectChange(FileSystemInfo fileSystemInfo, FileOperations operation, string oldName = "")
        {
            SearchNode localParent = _parentNode;

            string filePath = fileSystemInfo.FullName;
            List<string> splitPath = filePath.Split("\\").ToList();

            bool isFolder = fileSystemInfo is DirectoryInfo;
            string lastItem = splitPath.LastOrDefault();
            
            if (!isFolder)
            {
                _ = splitPath.Remove(splitPath.Last());
            }
            if (operation == FileOperations.Add)
            {
                foreach (string obj in splitPath)
                {
                    localParent = Add(localParent, obj, true, 0, fileSystemInfo.LastWriteTimeUtc);
                }
                if (!isFolder)
                {
                    localParent = Add(localParent, lastItem, false, ((FileInfo)fileSystemInfo).Length, fileSystemInfo.LastWriteTimeUtc);
                }
            }
            else if(operation == FileOperations.Remove)
            {
                Remove(localParent, filePath, isFolder);
            }
            else if (operation == FileOperations.Rename)
            {
                Rename(localParent, filePath, oldName, isFolder);
            }
        }

        public List<ViewModel> Search(string searchText, SearchNode node, string currentPath, List<ViewModel> foundPaths = null)
        {
            if (foundPaths == null)
            {
                foundPaths = new List<ViewModel>();
            }
            if (node.Name == "My Computer")
            {
                currentPath = "";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(currentPath))
                {
                    currentPath = node.Name;
                }
                else
                {
                    currentPath = currentPath + "\\" + node.Name;
                }
            }
            if (currentPath.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
            {
                var vm = new ViewModel
                {
                    Name = node.Name,
                    Path = currentPath
                };
                if (!(string.IsNullOrWhiteSpace(currentPath) || foundPaths.Contains(vm)))
                {
                    foundPaths.Add(vm);
                }
            }
            string currentFilePath = "";
            node.FileList.ForEach(fl => {
                currentFilePath = currentPath + "\\" + fl.Name;
                if (currentFilePath.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    var vm1 = new ViewModel
                    {
                        Name = node.Name,
                        Path = currentFilePath,
                        Size = fl.Size
                    };
                    if (!(string.IsNullOrWhiteSpace(currentFilePath) || foundPaths.Contains(vm1)))
                    {
                        foundPaths.Add(vm1);
                    }
                }
            }
            );

            node.DirectoryList.ForEach(dl => { _ = Search(searchText, dl, currentPath, foundPaths); });

            return foundPaths;
        }

        public SearchNode Add(SearchNode a, string path, bool isFolder, long size = 0, DateTime? lastModified = null)
        {
            if (isFolder)
            {
                SearchNode tempNode = a.DirectoryList.FirstOrDefault(x => x.Name == path);
                if (tempNode != null)
                {
                    return tempNode;
                }
                else
                {
                    SearchNode newDirectory = new SearchNode
                    {
                        Name = path,
                        DirectoryList = new List<SearchNode>(),
                        FileList = new List<FileDetails>(),
                        LastModified = lastModified.Value
                    };
                    a.DirectoryList.Add(newDirectory);
                    return newDirectory;
                }
            }
            else
            {
                var tempFile = a.FileList.Where(x => x.Name == path);
                if (tempFile.Count() == 0)
                {
                    FileDetails newFileDetails = new FileDetails
                    {
                        Name = path,
                        Size = size,
                        LastModified = lastModified.Value
                    };
                    a.FileList.Add(newFileDetails);
                }
                return a;
            }
        }

        public void Remove(SearchNode a, string path, bool isFolder)
        {
            SearchNode currentNode = a;
            List<string> splitPath = path.Split("\\").ToList();
            for (int i = 0; i < splitPath.Count(); i++)
            {
                if (i == splitPath.Count() - 1)
                {
                    if (isFolder)
                    {
                        var itemToBeRemoved = currentNode.DirectoryList.FirstOrDefault(x => x.Name == splitPath[i]);
                        currentNode.DirectoryList.Remove(itemToBeRemoved);
                    }
                    else
                    {
                        var itemToBeRemoved = currentNode.FileList.FirstOrDefault(x => x.Name == splitPath[i]);
                        currentNode.FileList.Remove(itemToBeRemoved);
                    }
                }
                else
                {
                    currentNode = currentNode.DirectoryList.FirstOrDefault(x => x.Name == splitPath[i]);
                }
            }
        }

        public void Rename(SearchNode a, string path, string oldName, bool isFolder)
        {
            SearchNode currentNode = a;
            List<string> splitPath = path.Split("\\").ToList();
            for (int i = 0; i < splitPath.Count(); i++)
            {
                if (i == splitPath.Count() - 1)
                {
                    if (isFolder)
                    {
                        var itemToBeRenamed = currentNode.DirectoryList.FirstOrDefault(x => x.Name == oldName);
                        itemToBeRenamed.Name = splitPath.Last();
                    }
                    else
                    {
                        var itemToBeRenamed = currentNode.FileList.FirstOrDefault(x => x.Name == oldName);
                        itemToBeRenamed.Name = splitPath.Last();
                    }
                }
                else
                {
                    currentNode = currentNode.DirectoryList.FirstOrDefault(x => x.Name == splitPath[i]);
                }
            }
        }
    }
}
