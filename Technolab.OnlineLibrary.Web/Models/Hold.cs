namespace Technolab.OnlineLibrary.Web.Models
{
    public class Hold
    {
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
