using AddressBook.Data.Entities;

namespace AddressBook.Data.Repositories.Abstraction
{
    public interface IContactsRepository : IRepository<Contact>
    {
        IEnumerable<Contact> FindAllByTerm(string term, int pageNumber, int pageSize);
        int CountByTerm(string term);
    }
}
