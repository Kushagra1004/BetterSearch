using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Model
{
    public class SearchNode
    {
        private static SearchNode _parentNode;
        public static SearchNode GetTree() 
        {
            if (_parentNode == null)
            {
                _parentNode = new SearchNode
                {
                    Name = "My Computer",
                    DirectoryList = new List<SearchNode>(),
                    FileList = new List<FileDetails>()
                };
            }
            return _parentNode;
        }
        public string Name { get; set; }
        public List<FileDetails> FileList { get; set; }
        public List<SearchNode> DirectoryList { get; set; }
        public DateTime LastModified { get; set; }
    }

    public struct FileDetails
    {
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}
