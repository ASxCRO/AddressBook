using AddressBook.Data.Entities;

namespace AddressBook.Models
{
    public class ContactGridViewModel
    {
        public IEnumerable<Contact> Contacts { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemsNumber { get; set; }
        public string SearchTerm { get; set; }
    }
}
