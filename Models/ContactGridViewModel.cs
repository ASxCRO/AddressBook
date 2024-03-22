using AddressBook.Data.Entities;

namespace AddressBook.Models
{
    public class ContactGridViewModel : GridPager
    {
        public IEnumerable<Contact> Contacts { get; set; }
    }
}
