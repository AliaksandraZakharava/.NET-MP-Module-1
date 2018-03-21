using System;

namespace NETMP.Module1.FileSystemVisitor
{
    public class FileSystemVisitorEventArgs : EventArgs
    {
        public bool ShouldStopSearch { get; set; }

        public bool ShouldExcludeResult { get; set; }
    }
}
