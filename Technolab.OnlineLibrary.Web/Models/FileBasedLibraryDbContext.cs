using System.Text.Json;

namespace Technolab.OnlineLibrary.Web.Models
{
    public class FileBasedLibraryDbContext : ILibraryDbContext
    {
        public FileBasedLibraryDbContext(string dataDirectoryPath)
        {
            this.DataDirectoryPath = dataDirectoryPath;
            Users = Load<User>(UsersFilePath);
            Books = Load<Book>(BooksFilePath);
        }

        #region ILibraryDbContext Implementation

        public List<User> Users { get; set; }
        public List<Book> Books { get; set; }

        public void SaveChanges()
        {
            File.WriteAllText(UsersFilePath, JsonSerializer.Serialize(Users));
            File.WriteAllText(BooksFilePath, JsonSerializer.Serialize(Books));
        }

        public void Dispose()
        {
        }

        #endregion

        public string DataDirectoryPath { get; }

        public string UsersFilePath => Path.Combine(DataDirectoryPath, "users.json");

        public string BooksFilePath => Path.Combine(DataDirectoryPath, "books.json");

        private List<T> Load<T>(string filePath)
        {
            if (!Directory.Exists(DataDirectoryPath))
            {
                Directory.CreateDirectory(DataDirectoryPath);
            }

            if (!File.Exists(filePath))
            {
                // File.Create keeps a lock on the file after creation if not disposed properly
                using (var file = File.Create(filePath)) 
                { 
                }
            }

            var contents = File.ReadAllText(filePath);
            return string.IsNullOrEmpty(contents) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(contents);
        }


    }
}