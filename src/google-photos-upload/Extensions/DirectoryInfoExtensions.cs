using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace google_photos_upload.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static long GetDirectorySize(this DirectoryInfo directoryInfo)
        {
            return directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }
    }
}
