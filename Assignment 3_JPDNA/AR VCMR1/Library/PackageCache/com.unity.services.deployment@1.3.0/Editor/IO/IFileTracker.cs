using System;

namespace Unity.Services.Deployment.Editor.IO
{
    interface IFileTracker
    {
        DateTime GetLastWriteTime(string path);
    }
}
