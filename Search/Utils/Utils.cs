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

}