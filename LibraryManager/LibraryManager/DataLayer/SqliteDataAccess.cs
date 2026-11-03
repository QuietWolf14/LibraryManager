using Dapper;
using System.Data;
using System.Data.SQLite;
using LibraryManager.Models;
using static System.Configuration.ConfigurationManager;

namespace LibraryManager.DataLayer
{
    public class SqliteDataAccess
    {
        private readonly string _connectionString;
        public SqliteDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("Default").Value.ToString()!;
        }
        
        public async Task<List<Book>> LoadBooks()
        {
            string sql = @"select b.ID, b.Title, a.ID, a.Name 
                           from Book as b
                           left join Author as a
                               on b.AuthorID = a.ID";

            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                List<Book> books = (await cnn.QueryAsync<Book, Author, Book>(sql,
                    (book, author) => {
                        book.BookAuthor = author;
                        return book;
                    },
                    splitOn: ("ID,ID")
                )).ToList<Book>();

                return books;
            }
        }

        public async Task<Book> GetBook(int id)
        {
            string sql = @"select * from Book where ID = @ID";

            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return (await cnn.QueryAsync<Book>(sql, new { ID = id })).FirstOrDefault<Book>();
            }
        }

        public void EditBook(Book book)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute("update Kit set Title = @Title, AuthorID = @AuthorId where ID = @ID", book);
            }
        }

        public void AddBook(Book book)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute("insert into Book (Title, AuthorID) values (@Title, @AuthorId)", book);
            }
        }
        
        public void DeleteBook(int bookId)
        {
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute("delete Book where ID = @ID", bookId);
            }
        }

        //private static string LoadConnectionString(string id = "Default")
        //{
        //    IConfiguration configuration;
        //    return configuration.GetConnectionString(id)!;
        //}
    }
}
