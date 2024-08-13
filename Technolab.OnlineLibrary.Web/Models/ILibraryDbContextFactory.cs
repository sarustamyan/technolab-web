namespace Technolab.OnlineLibrary.Web.Models
{
    public interface ILibraryDbContextFactory
    {
        public ILibraryDbContext Create();
    }
}