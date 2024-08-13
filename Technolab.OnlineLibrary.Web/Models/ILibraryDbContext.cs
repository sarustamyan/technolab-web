namespace Technolab.OnlineLibrary.Web.Models
{
    public interface ILibraryDbContext : IDisposable
    {
        List<User> Users { get; set; }

        List<Book> Books { get; set; }

        void SaveChanges();
    }
}