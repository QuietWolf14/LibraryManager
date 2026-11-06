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
            string sql = @"select b.ID, b.Title, b.AuthorID, a.ID, a.Name 
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
        
        public async Task<List<Author>> LoadAuthors()
        {
            string sql = @"select ID, Name from Author";

            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                return (await cnn.QueryAsync<Author>(sql)).ToList<Author>();
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
        
        public void AddBook(Book book)
        {
            string title = book.Title;
            int authorId = book.AuthorID;
            
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute("insert into Book (Title, AuthorID) values (@Title, @AuthorID)", new {@Title = title, @AuthorID = authorId});
            }
        }

        public void EditBook(Book book)
        {
            int id = book.ID;
            string title = book.Title;
            int authorId = book.AuthorID;
            string sql = "update Book set Title = @Title, AuthorID = @AuthorID where ID = @ID";
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute(sql, new {@ID = id, @Title = title, @AuthorID = authorId});
            }
        }
        
        public void DeleteBook(int bookId)
        {
            string sql = "delete from Book where ID = @ID";
            using (IDbConnection cnn = new SQLiteConnection(_connectionString))
            {
                cnn.Execute(sql, new { @ID = bookId });
            }
        }

        //private static string LoadConnectionString(string id = "Default")
        //{
        //    IConfiguration configuration;
        //    return configuration.GetConnectionString(id)!;
        //}
    }
}
