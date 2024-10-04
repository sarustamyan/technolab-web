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
            throw new NotImplementedException();
            //return new FileBasedLibraryDbContext(DirectoryPath);
        }

        public string DirectoryPath { get; }
    }
}