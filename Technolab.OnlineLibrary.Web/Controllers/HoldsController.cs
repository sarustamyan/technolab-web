using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using System;

public class HoldsController : Controller
{
    private readonly ILibraryDbContextFactory _dbContextFactory;

    public HoldsController(ILibraryDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IActionResult PlaceHold(int bookId)
    {
        using (var context = _dbContextFactory.Create())
        {
            var holdId = context.Holds.Any() ? context.Holds.Max(h => h.HoldId) + 1 : 1;

            var hold = new Hold
            {
                HoldId = holdId,
                UserId = 1, // Change to the current user's ID
                BookId = bookId,
                CreatedOn = DateTime.Now,
                Status = HoldStatus.Pending
            };

            context.Holds.Add(hold);
            context.SaveChanges();
        }

        return RedirectToAction("HoldConfirmation");
    }

    public IActionResult HoldConfirmation()
    {
        ViewBag.Message = "You have successfully placed a hold on this book.";
        return View();
    }

    public IActionResult HoldFailure()
    {
        ViewBag.Message = "There was an error processing your hold request.";
        return View();
    }
}
