using System.Collections.Generic;
using NETMP.Module1.FileSystemVisitor.Entities;

namespace NETMP.Module1.FileSystemVisitor.Interfaces
{
    public interface IFileSystemEntitiesProvider
    {
        IEnumerable<VisitedFileSystemEntity> GetFileSystemEntities(string rootDirectoryPath);
    }
}
