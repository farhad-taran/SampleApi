using Books.Api.Contracts.Common;
using Books.Api.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Books.Api.UnitTests.Validation
{
    public class CreateBookRequestValidatorTests
    {
        private readonly BookModelValidator _sut;

        public CreateBookRequestValidatorTests()
        {
            _sut = new BookModelValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Validate_WhenBookIdIsNullOrEmpty_IsInvalid(string value)
        {
            var request = new BookModel
            {
                BookId = value
            };

            _sut.ShouldHaveValidationErrorFor(x => x.BookId, request);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Validate_WhenBookNameIsNullOrEmpty_IsInvalid(string value)
        {
            var request = new BookModel
            {
                Name = value
            };

            _sut.ShouldHaveValidationErrorFor(x => x.Name, request);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Validate_WhenAuthorIdIsNullOrEmpty_IsInvalid(string value)
        {
            var request = new BookModel
            {
                AuthorId = value
            };

            _sut.ShouldHaveValidationErrorFor(x => x.AuthorId, request);
        }
    }
}
