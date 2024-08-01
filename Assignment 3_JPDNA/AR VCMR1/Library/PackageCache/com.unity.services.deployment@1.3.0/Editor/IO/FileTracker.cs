using System;
using System.IO;

namespace Unity.Services.Deployment.Editor.IO
{
    class FileTracker : IFileTracker
    {
        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }
    }
}
