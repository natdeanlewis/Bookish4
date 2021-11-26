

using System.Collections.Generic;
using System.Linq;
using Bookish4._DataAccess;

namespace Bookish4.Web2.Models
{
    public class BookViewModel
    {
        public string Title;
        public List<string> Authors = new();

        public override string ToString()
        {
            if (Authors.Count == 1) return $"{Title} by {Authors.First()}";
            return $"{Title} by {string.Join(" and ", Authors)}";
        }

        public BookViewModel(Book b)
        {
            Title = b.Title;
            Authors = b.Authors;
        }
        
    }
}