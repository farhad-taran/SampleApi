namespace Books.Api.Controllers
{
    public static class ApiRoutes
    {
        public static class Books
        {
            public const string CreateBook = "books";
            public const string GetBook = "books/{bookId}";
            public const string UpdateBook = "books/{bookId}";
            public const string DeleteBook = "books/{bookId}";
        }
    }
}