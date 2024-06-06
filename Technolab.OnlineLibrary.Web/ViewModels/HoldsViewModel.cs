using Technolab.OnlineLibrary.Web.Models;
namespace Technolab.OnlineLibrary.Web.ViewModels

{
      public class HoldsViewModel
    {
        public List<Book> Books { get; set; }
        public int Id { get; set; }
        public int HoldId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime CreatedOn { get; set; }
        public HoldStatus Status { get; set; }
    }

    public enum HoldStatus
    {
        Pending,
        Completed
    }
}

