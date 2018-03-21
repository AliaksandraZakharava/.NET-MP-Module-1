using System.Collections.Generic;
using System.IO;
using NETMP.Module1.FileSystemVisitor.Entities;
using NETMP.Module1.FileSystemVisitor.Interfaces;

namespace NETMP.Module1.FileSystemVisitor
{
    public class FileSystemEntitiesProvider : IFileSystemEntitiesProvider
    {
        public IEnumerable<VisitedFileSystemEntity> GetFileSystemEntities(string rootDirectoryPath)
        {
            var paths = Directory.GetFileSystemEntries(rootDirectoryPath, "*", SearchOption.AllDirectories);

            var result = new List<VisitedFileSystemEntity>();

            foreach (var path in paths)
            {
                result.Add(new VisitedFileSystemEntity
                {
                    EntityPath = path,
                    EntityType = IsDirectory(path) ? VisitedFileSystemEntityType.Directory : VisitedFileSystemEntityType.File
                });
            }

            return result;
        }

        private bool IsDirectory(string fileSystemEntityPath)
        {
            return (File.GetAttributes(fileSystemEntityPath) & FileAttributes.Directory) == FileAttributes.Directory;
        }
    }
}
