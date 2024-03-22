using System.ComponentModel.DataAnnotations;

namespace AddressBook.Data.Entities
{
    public class Contact : BaseEntity<int>
    {
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(100)]
        [Required]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
    }
}
