namespace NETMP.Module1.FileSystemVisitor.Entities
{
    public enum VisitedFileSystemEntityType
    {
        Directory,
        File
    }

    public class VisitedFileSystemEntity
    {
        public VisitedFileSystemEntityType EntityType { get; set; }

        public string EntityPath { get; set; }
    }
}
