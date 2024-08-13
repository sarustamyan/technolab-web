namespace Technolab.OnlineLibrary.Web.Models
{
    public class FileBasedLibraryDbContextFactory : ILibraryDbContextFactory
    {
        public FileBasedLibraryDbContextFactory(string directoryPath)
        {
            this.DirectoryPath = directoryPath;
        }


        public ILibraryDbContext Create()
        {
            return new FileBasedLibraryDbContext(DirectoryPath);
        }

        public string DirectoryPath { get; }
    }
}
