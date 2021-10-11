using Search.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

public class Utils
{
    public static void logText(String msg)
    {
        string timestamp = DateTime.Now.ToString("hh:mm:ss.fff tt");
        Debug.WriteLine(msg + " at " + timestamp);
    }

    // drives = 5
    // folders = 300,000
    //files =  3000,000
    // factor = 10
    // avrg name length 6-12 bytes

    static int flMxf = 3;
    static int drMx = 12;
    static int mxLvl = 5;
    static DateTime writeTime = new DateTime();

    private static Random random = new Random(14);
    public static List<FileModel> loadFakeData()
    {
        
        List<FileModel> fileList = new List<FileModel>();
        
        string[] drives = {"C:\\", "D:\\", "E:\\", "F:\\", "G:\\"};
        foreach (var drive in drives)
        {
            fileList.AddRange(getList(drive, 0));
            
        }

        return fileList;
    }

    public static string generateRandomNameFile()
    {
        
        const string chars = "qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int length = random.Next(1, 30);
        return (new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray()) + ".txt");
    }

    

    public static string generateRandomNameFolder()
    {
        const string chars = "qwertyuiopasdfghjklzxcvbnmABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int length = random.Next(1, 15);
        return (new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray()));
    }

    public static List<FileModel> getList(string folder, int level)
    {
        
        List<FileModel> fList = new List<FileModel>();
        
        int folderCount = random.Next(0,drMx);
        int fileCount = random.Next(0,flMxf * level);
        if (level > mxLvl)
        {
            folderCount = 0;
        }

        for (int i = 0; i < fileCount; i++)
        {
            string path = folder +  generateRandomNameFile();
            fList.Add(new FileModel(path, writeTime, 23));
        }

        for (int i = 0; i < folderCount; i++)
        {
            string path = folder + generateRandomNameFolder();
            fList.Add(new FileModel(path, writeTime, -1));
            fList.AddRange(getList(path + "\\", level + 1));
        }

        return fList;
    }

}