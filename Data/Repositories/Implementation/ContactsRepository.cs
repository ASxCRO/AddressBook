using AddressBook.Data.Entities;
using AddressBook.Data.Repositories.Abstraction;
using Dapper;
using System.Drawing;

namespace AddressBook.Data.Repositories.Implementation
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ContactsRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public int Add(Contact item)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.QuerySingle<int>(
                    "INSERT INTO Contacts (FirstName, Email, LastName) VALUES (@FirstName, @Email, @LastName); SELECT SCOPE_IDENTITY()",
                    item
                );
            }
        }

        public int CountAll()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Contacts");
            }
        }

        public IEnumerable<Contact> FindAll(int pageNumber, int pageSize, string sortField, string term)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                string order = sortField switch
                {
                    "FirstNameAsc" => "FirstName asc",
                    "FirstNameDesc" => "FirstName desc",
                    "LastNameAsc" => "LastName asc",
                    "LastNameDesc" => "LastName desc",
                    "EmailAsc" => "Email asc",
                    "EmailDesc" => "Email desc",
                    _ => "ID"
                };

                var where = "WHERE FirstName LIKE @term OR LastName LIKE @term OR Email LIKE @term";
                where = !string.IsNullOrWhiteSpace(term) ? where : string.Empty;

                var query = $"SELECT * FROM Contacts {where} ORDER BY {order} OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

                return connection.Query<Contact>(query, new { term = $"%{term}%", offset = (pageNumber - 1) * pageSize, pageSize });
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

        public int  CountByTerm(string term)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.ExecuteScalar<int>("SELECT Count(*) FROM Contacts WHERE FirstName LIKE @term OR LastName LIKE @term OR Email LIKE @term", new { term = $"%{term}%"});
            }
        }
    }
}
