using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Technolab.OnlineLibrary.Web.Models;

namespace Technolab.OnlineLibrary.Web.BackgroundServices
{
    public class BookAvailabilityService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BookAvailabilityService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ILibraryDbContextFactory>().Create();

                    var books = context.Books.ToList();
                    foreach (var book in books)
                    {
                        if (book.Count > 0)
                        {
                            var pendingHold = context.Holds
                                .Where(h => h.BookId == book.Id && h.Status == HoldStatus.Pending)
                                .OrderBy(h => h.CreatedOn)
                                .FirstOrDefault();

                            if (pendingHold != null)
                            {
                                pendingHold.Status = HoldStatus.Completed;
                                book.Count -= 1;
                            }
                        }
                    }

                    context.SaveChanges();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
            }
        }
    }
}
