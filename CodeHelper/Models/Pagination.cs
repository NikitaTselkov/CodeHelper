namespace CodeHelper.Models
{
    public class Pagination
    {
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public Pagination(int page, int pagesCount)
        {
            CurrentPage = page;
            StartPage = page - 2 < 2 ? 2 : page - 2;
            EndPage = page + 4 >= pagesCount ? pagesCount : page + 4;
            PageCount = pagesCount;
        }
    }
}
