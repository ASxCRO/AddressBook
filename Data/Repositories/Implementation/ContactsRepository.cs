using AddressBook.Data.Entities;
using AddressBook.Data.Repositories.Abstraction;
using Dapper;

namespace AddressBook.Data.Repositories.Implementation
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ContactsRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(Contact item)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Execute("INSERT INTO Contacts (FirstName, Email, LastName) VALUES (@FirstName, @Email, @LastName)", item);
            }
        }

        public int CountAll()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Contacts");
            }
        }

        public IEnumerable<Contact> FindAll(int pageNumber, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.Query<Contact>("SELECT * FROM Contacts ORDER BY ID OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY", new { offset = (pageNumber - 1) * pageSize, pageSize });
            }
        }

        public Contact FindByID(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.QuerySingleOrDefault<Contact>("SELECT * FROM Contacts WHERE ID = @ID", new { ID = id });
            }
        }

        public void Remove(int id)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Execute("DELETE FROM Contacts WHERE ID = @ID", new { ID = id });
            }
        }

        public void Update(Contact item)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                item.ModifiedAt = DateTime.Now;
                connection.Execute("UPDATE Contacts SET FirstName = @FirstName, Email = @Email, LastName = @LastName, ModifiedAt = @ModifiedAt WHERE ID = @ID", item);
            }
        }

        public IEnumerable<Contact> FindAllByTerm (string term, int pageNumber, int pageSize)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.Query<Contact>("SELECT * FROM Contacts WHERE FirstName LIKE @term OR LastName LIKE @term OR Email LIKE @term ORDER BY ID OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY", new { term = $"%{term}%", offset = (pageNumber - 1) * pageSize, pageSize });
            }
        }

        public int  CountByTerm(string term)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.ExecuteScalar<int>("SELECT Count(*) FROM Contacts WHERE FirstName LIKE @term OR LastName LIKE @term OR Email LIKE @term", new { term = $"%{term}%"});
            }
        }
    }
}
