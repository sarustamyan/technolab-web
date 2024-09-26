using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [AllowAnonymous]
    public class BooksController : Controller
    {
        public BooksController(ILibraryDbContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        public ILibraryDbContextFactory ContextFactory { get; }

        public IActionResult Index(int pageNumber = 1)
        {

            var pageSizeCookie = Request.Cookies["PageSize"];
            int pageSize = string.IsNullOrEmpty(pageSizeCookie) ? 10 : int.Parse(pageSizeCookie);

            pageSize = Math.Max(1, Math.Min(pageSize, 100));

            using var context = ContextFactory.Create();

            var totalBooks = context.Books.Count();
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            var books = context.Books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BookViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Author = x.Author
                }).ToList();

            var model = new BookSearchViewModel
            {
                Books = books,
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            ViewBag.PageSize = pageSize;

            return View(model);
        }


        public IActionResult SetPageSize(int pageSize)
        {
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("The page size should be between 1 and 100.gi t");
            }

            Response.Cookies.Append("PageSize", pageSize.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = true,
                Secure = true
            });

            return RedirectToAction("Index", new { pageNumber = 1 });
        }
    }
}
