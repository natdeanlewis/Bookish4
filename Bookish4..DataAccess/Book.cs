using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookish4
{
    public class Book
    {
        public List<string> Authors = new();
        public int BookId;
        public DateTime DueDate;
        public string FirstName;
        public string Isbn;
        public string LastName;
        public string Title;
        public int Copies;
        public int MemberId;
        public string Username;

        public override string ToString()
        {
            if (Authors.Count == 1) return $"{Title} by {Authors.First()}";
            return $"{Title} by {string.Join(" and ", Authors)}";
        }
    }
}