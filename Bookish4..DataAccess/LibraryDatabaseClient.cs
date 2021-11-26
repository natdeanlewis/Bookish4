using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Dapper;

namespace Bookish4._DataAccess
{
    public static class LibraryDatabaseClient
    {
        public static List<Book> GetAllBooks()
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();
                var sql = "SELECT BookId, ISBN FROM Book ORDER BY Title";

                var result = db.Query<Book>(sql);
                result = result.GroupBy(x => x.Isbn).Select(x => x.First()).ToList();

                var output = new List<Book>();

                foreach (var book in result)
                {
                    Book b = GetBookInfo(book.BookId);
                    b.Isbn = book.Isbn;
                    output.Add(b);
                }

                return output;
            }
        }


        public static List<Book> GetAllLoans(int memberId)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var parameters = new {MemberId = memberId};
                var sql =
                    "SELECT b.BookId, bm.DueDate FROM Book b " +
                    "INNER JOIN BookMember bm ON b.BookId = bm.BookId " +
                    "INNER JOIN Member m ON bm.MemberId = m.MemberId " +
                    "WHERE m.MemberId = @MemberId ORDER BY bm.DueDate";

                var result = db.Query<Book>(sql, parameters);

                var output = new List<Book>();

                foreach (var book in result)
                {
                    Book b = GetBookInfo(book.BookId);
                    b.Isbn = book.Isbn;
                    b.DueDate = book.DueDate;
                    output.Add(b);
                }

                return output;

            }
        }

        public static List<Book> Search(string request)
        {
            var parameters = new {Request = request};

            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var sql = "SELECT b.BookId, b.ISBN FROM Book b " +
                          "INNER JOIN BookAuthor ba ON b.BookId = ba.BookId " +
                          "INNER JOIN Author a ON ba.AuthorId = a.AuthorId " +
                          "WHERE b.Title LIKE '%'+ @Request + '%' " +
                          "OR a.LastName LIKE '%'+ @Request + '%' " +
                          "OR a.FirstName LIKE '%'+ @Request + '%'";

                var result = db.Query<Book>(sql, parameters);

                result = result.GroupBy(x => x.Isbn).Select(x => x.First()).ToList();

                var output = new List<Book>();

                foreach (var book in result)
                {
                    Book b = GetBookInfo(book.BookId);
                    b.Isbn = book.Isbn;
                    output.Add(b);
                }

                return output;
            }
        }

        private static Book GetBookInfo(int bookId)
        {
            var book = new Book();
            book.Authors = GetBookAuthors(bookId);
            book.Title = GetBookTitle(bookId);
            return book;
        }

        private static string GetBookTitle(int bookId)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var parameters = new {BookId = bookId};
                var sql =
                    "SELECT b.Title FROM Book b " +
                    "WHERE b.BookId = @BookId";

                var result = db.Query<string>(sql, parameters).First();

                return result;
            }
        }

        private static List<string> GetBookAuthors(int bookId)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var parameters = new {BookId = bookId};
                var sql =
                    "SELECT a.FirstName, a.LastName FROM Book b " +
                    "INNER JOIN BookAuthor ba ON b.BookId = ba.BookId " +
                    "INNER JOIN Author a ON ba.AuthorId = a.AuthorId " +
                    "WHERE b.BookId = @BookId";

                var result = db.Query<Author>(sql, parameters);

                var authors = new List<string>();
                foreach (var author in result) authors.Add($"{author.FirstName} {author.LastName}");
                return authors;
            }
        }

        public static List<string> CheckAvailability(int isbn)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var parameters = new {Isbn = isbn};
                var sql =
                    "SELECT b.BookId FROM Book b " +
                    "WHERE b.ISBN = @Isbn";

                var result = db.Query<int>(sql, parameters);
                List<string> output = null;
                output.Add($"We have {result.Count()} copy(ies) of this title in the catalogue:");
                foreach (var bookId in result) output.Add(GetLoanInfo(bookId));
                return output;
            }
        }

        public static string GetLoanInfo(int bookId)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;Database=Bookish;Trusted_Connection=True;"))
            {
                db.Open();

                var parameters = new {BookId = bookId};
                var sql =
                    "SELECT bm.MemberId, bm.DueDate, m.Username FROM Book b " +
                    "INNER JOIN BookMember bm ON b.BookId = bm.BookId " +
                    "INNER JOIN Member m ON bm.MemberId = m.MemberId " +
                    "WHERE b.BookId = @BookId";

                var result = db.Query<Book>(sql, parameters);

                if (result.Count() == 0)
                {
                    return "Available to loan";
                }
                else
                {
                    var book = result.First();
                    var date = book.DueDate.ToString("d", CultureInfo.CreateSpecificCulture("en-GB"));
                    return $"Borrowed by {book.Username}, due back on {date}";
                }
            }
        }
    }
}