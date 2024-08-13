using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [AllowAnonymous]
    public class BooksController : Controller
    {
        public BooksController(ILibraryDbContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        public IActionResult Index()
        {
            using var context = ContextFactory.Create();

            var model = new BookSearchViewModel
            {
                Books = context.Books.Take(10).Select(x => new BookViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author
                }).ToList()
            };

            return View(model);
        }

        public ILibraryDbContextFactory ContextFactory { get; }
    }
}
