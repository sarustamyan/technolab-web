using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [AllowAnonymous]
    public class BooksController : Controller
    {
        private readonly ILibraryDbContextFactory _contextFactory;

        public BooksController(ILibraryDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public IActionResult Index(string searchTerm)
        {
            using var context = _contextFactory.Create();

            List<Book> books = context.Books.ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTerms = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                books = books.Where(book => searchTerms.Any(term => book.Title.Contains(term) || book.Author.Contains(term))).ToList();
            }

            var model = new BookSearchViewModel
            {
                Books = books.Take(10).Select(x => new BookViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author
                }).ToList(),
                SearchTerm = searchTerm
            };

            return View(model);
        }
    }
}
