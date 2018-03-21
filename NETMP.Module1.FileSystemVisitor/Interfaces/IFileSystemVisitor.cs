using System;
using System.Collections.Generic;
using NETMP.Module1.FileSystemVisitor.Entities;

namespace NETMP.Module1.FileSystemVisitor.Interfaces
{
    public interface IFileSystemVisitor
    {
        IEnumerable<VisitedFileSystemEntity> GetResultOfBypassingDirectories(string rootDirectoryPath);
		
		event EventHandler StartBypassing;
        event EventHandler FinishBypassing;

        event EventHandler<FileSystemVisitorEventArgs> FileFound;
        event EventHandler<FileSystemVisitorEventArgs> DirectoryFound;

        event EventHandler<FileSystemVisitorEventArgs> FilteredFileFound;
        event EventHandler<FileSystemVisitorEventArgs> FilteredDirectoryFound;
    }
}
