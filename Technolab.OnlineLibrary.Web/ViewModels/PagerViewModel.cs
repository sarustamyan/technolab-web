namespace Technolab.OnlineLibrary.Web.ViewModels
{
    public class PagerViewModel
    {
        public int TotalItems { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }

        public int TotalPages { get; }
        public int StartPage { get; }
        public int EndPage { get; }

        public PagerViewModel(int totalItems, int page, int pageSize = 10)
        {
            TotalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);
            CurrentPage = page;
            TotalItems = totalItems;
            PageSize = pageSize;

            int startPage = CurrentPage - 5;
            int endPage = CurrentPage + 4;
            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > TotalPages)
            {
                endPage = TotalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}