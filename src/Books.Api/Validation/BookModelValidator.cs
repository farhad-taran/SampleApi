using Books.Api.Contracts.Common;
using FluentValidation;

namespace Books.Api.Validation
{
    public class BookModelValidator : AbstractValidator<BookModel>
    {
        public BookModelValidator()
        {
            RuleFor(x => x).NotNull();

            When(x => x != null, () => {
                RuleFor(x => x.BookId).NotEmpty();
                RuleFor(x => x.AuthorId).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
            });
        }
    }
}
