using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Api.Storage.Model
{
    public class BookItem
    {
        public string BookId { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
    }
}
