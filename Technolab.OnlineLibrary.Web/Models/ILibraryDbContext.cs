using Microsoft.EntityFrameworkCore;

namespace Technolab.OnlineLibrary.Web.Models
{
    public interface ILibraryDbContext : IDisposable
    {
        DbSet<User> Users { get; set; }

        DbSet<Book> Books { get; set; }

        int SaveChanges();
    }
}