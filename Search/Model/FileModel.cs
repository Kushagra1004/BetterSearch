using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Search.Model
{
    public class FileModel
    {
        public string FullName { get; set; }
        public DateTime LastWriteTimeUtc { get; set; }
        public long Length { get; set; }

        public FileModel(string fullName, DateTime lastTime, long length)
        {
            FullName = fullName;
            LastWriteTimeUtc = lastTime;
            Length = length;
        }

        public FileModel(FileSystemInfo fs)
        {
            FullName = fs.FullName;
            LastWriteTimeUtc = fs.LastWriteTimeUtc;
            Length = (fs is DirectoryInfo) ? -1 : ((FileInfo)fs).Length;
        }

        public bool isDirectory()
        {
            return this.Length < 0;
        }
    }

   
}
