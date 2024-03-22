namespace AddressBook.Models
{
    public abstract class GridPager
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsNumber { get; set; }
        public string SearchTerm { get; set; }
        public string Sort { get; set; }
    }
}