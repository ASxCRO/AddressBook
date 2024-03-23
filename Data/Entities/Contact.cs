using System.ComponentModel.DataAnnotations;

namespace AddressBook.Data.Entities
{
    public class Contact : BaseEntity<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
