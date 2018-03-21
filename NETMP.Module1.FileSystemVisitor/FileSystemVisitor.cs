using System;
using System.Collections.Generic;
using System.IO;
using NETMP.Module1.FileSystemVisitor.Entities;
using NETMP.Module1.FileSystemVisitor.Interfaces;

namespace NETMP.Module1.FileSystemVisitor
{
    public class FileSystemVisitor : IFileSystemVisitor
    {
        private readonly Func<VisitedFileSystemEntity, bool> _bypassingFilter;

        private readonly IFileSystemEntitiesProvider _fileSystemEntitiesProvider;

        public FileSystemVisitor(IFileSystemEntitiesProvider fileSystemEntitiesProvider) : this(fileSystemEntitiesProvider, data => true)
        {
        }

        public FileSystemVisitor(IFileSystemEntitiesProvider fileSystemEntitiesProvider, Func<VisitedFileSystemEntity, bool> bypassingFilter)
        {
            _fileSystemEntitiesProvider = fileSystemEntitiesProvider;

            _bypassingFilter = bypassingFilter;
        }

        public event EventHandler StartBypassing;
        public event EventHandler FinishBypassing;

        public event EventHandler<FileSystemVisitorEventArgs> FileFound;
        public event EventHandler<FileSystemVisitorEventArgs> DirectoryFound;

        public event EventHandler<FileSystemVisitorEventArgs> FilteredFileFound;
        public event EventHandler<FileSystemVisitorEventArgs> FilteredDirectoryFound;
        
        public IEnumerable<VisitedFileSystemEntity> GetResultOfBypassingDirectories(string rootDirectoryPath)
        {
            if (string.IsNullOrEmpty(rootDirectoryPath) || rootDirectoryPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || !Directory.Exists(rootDirectoryPath))
            {
                throw new ArgumentException($"Passed rootDirectoryPath {rootDirectoryPath} is not valid.");
            }

            OnStartBypassing();

            foreach (var visitedEntity in _fileSystemEntitiesProvider.GetFileSystemEntities(rootDirectoryPath))
            {
                var args = new FileSystemVisitorEventArgs();

                OnEntityFound(visitedEntity, args);

                if (_bypassingFilter(visitedEntity))
                {
                    OnFilteredEntityFound(visitedEntity, args);

                    if(args.ShouldStopSearch) { break; }

                    if(args.ShouldExcludeResult) { continue; }

                    yield return visitedEntity;
                }
            }

            OnFinishBypassing();
        }

        private void OnStartBypassing()
        {
            var handler = StartBypassing;

            handler?.Invoke(this, null);
        }

        private void OnFinishBypassing()
        {
            var handler = FinishBypassing;

            handler?.Invoke(this, null);
        }
        
        private void OnEntityFound(VisitedFileSystemEntity fileSystemEntity, FileSystemVisitorEventArgs args)
        {
            if (fileSystemEntity.EntityType == VisitedFileSystemEntityType.Directory)
            {
                OnDirectoryFound(args);
            }
            else
            {
                OnFileFound(args);
            }
        }
        
        private void OnFilteredEntityFound(VisitedFileSystemEntity fileSystemEntity, FileSystemVisitorEventArgs args)
        {
            if (fileSystemEntity.EntityType == VisitedFileSystemEntityType.Directory)
            {
                OnFilteredDirectoryFound(args);
            }
            else
            {
                OnFilteredFileFound(args);
            }
        }

        private void OnFileFound(FileSystemVisitorEventArgs args)
        {
            var handler = FileFound;

            handler?.Invoke(this, args);
        }

        private void OnDirectoryFound(FileSystemVisitorEventArgs args)
        {
            var handler = DirectoryFound;

            handler?.Invoke(this, args);
        }

        private void OnFilteredDirectoryFound(FileSystemVisitorEventArgs args)
        {
            var handler = FilteredDirectoryFound;

            handler?.Invoke(this, args);
        }

        private void OnFilteredFileFound(FileSystemVisitorEventArgs args)
        {
            var handler = FilteredFileFound;

            handler?.Invoke(this, args);
        }
    }
}
