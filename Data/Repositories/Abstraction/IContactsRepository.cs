using AddressBook.Data.Entities;

namespace AddressBook.Data.Repositories.Abstraction
{
    public interface IContactsRepository : IRepository<Contact>
    {
        int CountByTerm(string term);
    }
}
