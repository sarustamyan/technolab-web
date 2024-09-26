namespace Technolab.OnlineLibrary.Web.ViewModels
{
    public class BookSearchViewModel
    {
        public List<BookViewModel> Books { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
