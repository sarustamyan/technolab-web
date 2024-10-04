namespace Technolab.OnlineLibrary.Web.Models
{
    public class SqlBasedLibraryDbContextFactory : ILibraryDbContextFactory
    {
        public SqlBasedLibraryDbContextFactory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public ILibraryDbContext Create()
        {
            return new SqlBasedLibraryDbContext(ConnectionString);
        }

        public string ConnectionString { get; }
    }
}