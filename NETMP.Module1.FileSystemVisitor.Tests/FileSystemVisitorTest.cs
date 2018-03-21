using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using NETMP.Module1.FileSystemVisitor.Interfaces;
using Xunit;
using FluentAssertions;
using FluentAssertions.Events;
using NETMP.Module1.FileSystemVisitor.Entities;

namespace NETMP.Module1.FileSystemVisitor.Tests
{
    public class FileSystemVisitorTest
    {
        private readonly IFileSystemEntitiesProvider _fileSystemEntitiesProvider;

        private readonly List<VisitedFileSystemEntity> _filesAndDirectories;
        private readonly string _rootDirectoryPath;

        public FileSystemVisitorTest()
        {
            _rootDirectoryPath = @"C:\";

            _filesAndDirectories = new List<VisitedFileSystemEntity>
            {
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.Directory, EntityPath = @"D:\TestFolder"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.File, EntityPath = @"D:\TestFolder\TestFile1.txt"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.File, EntityPath = @"D:\TestFolder\TestFile2.jpg"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.Directory, EntityPath = @"D:\TestFolder\TestSubfolder"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.Directory, EntityPath = @"D:\TestFolder\TestSubfolder\TestsubSubfolder"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.File, EntityPath = @"D:\TestFolder\TestSubfolder\TestFile1.txt"},
                new VisitedFileSystemEntity {EntityType = VisitedFileSystemEntityType.File, EntityPath = @"D:\TestFolder\TestSubfolder\TestFile2.jpg"}
            };

            _fileSystemEntitiesProvider = A.Fake<IFileSystemEntitiesProvider>();

            A.CallTo(() => _fileSystemEntitiesProvider.GetFileSystemEntities(A<string>._)).Returns(_filesAndDirectories);
        }

        public static IEnumerable<object[]> InvalidPathTestData()
        {
            yield return new object[] { string.Empty };
            yield return new object[] { @"C:\Directory*?" };
            yield return new object[] { @"C:\Not exising directory" };
        }

        [Theory]
        [MemberData(nameof(InvalidPathTestData))]
        public void GetResultOfBypassingDirectories_InvalidFilePath_ThrowsArgumentException(string path)
        {
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemEntitiesProvider);

            Assert.Throws<ArgumentException>(() => fileSystemVisitor.GetResultOfBypassingDirectories(path).ToList());
        }

        [Fact]
        public void GetResultOfBypassingDirectories_NoFilter_ReturnsVisitedEntityIEnumerable()
        {
            var fileSystemVisitor = new FileSystemVisitor(_fileSystemEntitiesProvider);
            var monitored = fileSystemVisitor.Monitor();

            var actualResult = fileSystemVisitor.GetResultOfBypassingDirectories(_rootDirectoryPath).ToList();

            Assert.NotEmpty(actualResult);
            Assert.Equal(actualResult.Count, _filesAndDirectories.Count);
            ShouldRaiseAllEventsExcept(monitored);
        }

        [Fact]
        public void GetResultOfBypassingDirectories_FilterIsSet_EntitiesFittingFilterExist_ReturnsVisitedEntityIEnumerable()
        {
            Func<VisitedFileSystemEntity, bool> filter = visitedEntity => visitedEntity.EntityPath.Contains(".txt");

            var fileSystemVisitor = new FileSystemVisitor(_fileSystemEntitiesProvider, filter);
            var monitored = fileSystemVisitor.Monitor();

            var result = fileSystemVisitor.GetResultOfBypassingDirectories(_rootDirectoryPath).ToList();

            Assert.NotEmpty(result);
            ShouldRaiseAllEventsExcept(monitored, "FilteredDirectoryFound");
        }

        [Fact]
        public void GetResultOfBypassingDirectories_FilterIsSet_EntitiesFittingFilterDoNotExist__ReturnsEmptyVisitedEntityIEnumerable()
        {
            Func<VisitedFileSystemEntity, bool> filter = visitedEntity => visitedEntity.EntityPath.Length == 0;

            var fileSystemVisitor = new FileSystemVisitor(_fileSystemEntitiesProvider, filter);
            var monitored = fileSystemVisitor.Monitor();

            var result = fileSystemVisitor.GetResultOfBypassingDirectories(_rootDirectoryPath).ToList();

            Assert.Empty(result);
            ShouldRaiseAllEventsExcept(monitored, "FilteredFileFound", "FilteredDirectoryFound");
        }

        private void ShouldRaiseAllEventsExcept(IMonitor<FileSystemVisitor> monitored, params string[] exceptEvents)
        {
            var events = new []
            {
                "StartBypassing", "FinishBypassing",
                "FileFound", "DirectoryFound",
                "FilteredFileFound", "FilteredDirectoryFound"
            };

            foreach (var e in events.Except(exceptEvents))
            {
                monitored.Should().Raise(e);
            }

            foreach (var e in exceptEvents)
            {
                monitored.Should().NotRaise(e);
            }
        }
    }
}
