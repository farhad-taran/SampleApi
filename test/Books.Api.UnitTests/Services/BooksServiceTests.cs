using System;
using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Services;
using Books.Api.Storage;
using Books.Api.Storage.Model;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Books.Api.UnitTests.Services
{
    public class BooksServiceTests
    {
        private readonly Mock<IBooksRepository> _booksRepositoryMock;
        private readonly BooksService _sut;
        public BooksServiceTests()
        {
            _booksRepositoryMock = new Mock<IBooksRepository>();
            _sut = new BooksService(_booksRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateBookAsync_WhenStoredInStorage_ReturnsSuccess()
        {
            //Arrange
            var expectedBookModel = new BookModel
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            //strict matching to make sure mapping is done correctly
            BookItem storedBookItem = null;
            _booksRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<BookItem>()))
                .Callback<BookItem>(x => storedBookItem = x)
                .ReturnsAsync(Result.Success<Unit, Error>(Unit.Instance));

            //Act
            var result = await _sut.CreateBookAsync(expectedBookModel);

            //Assert
            result.IsSuccess.Should().BeTrue();
            storedBookItem.Should().BeEquivalentTo(expectedBookModel);
        }

        [Fact]
        public async Task CreateBookAsync_WhenFailsToStoreInStorage_ReturnsFailure()
        {
            //Arrange
            var expectedBookModel = new BookModel
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            _booksRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<BookItem>()))
                .ReturnsAsync(Result.Failure<Unit, Error>(ErrorTypes.InternalServerError));

            //Act
            var result = await _sut.CreateBookAsync(expectedBookModel);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(ErrorTypes.InternalServerError);
        }

        [Fact]
        public async Task UpdateBookAsync_WhenUpdatesInStorage_ReturnsSuccess()
        {
            //Arrange
            var expectedBookId = Guid.NewGuid().ToString();
            var expectedBookModel = new BookModel
            {
                BookId = expectedBookId,
                AuthorId = "1",
                Name = "1"
            };
            //strict matching to make sure mapping is done correctly
            BookItem updatedBookItem = null;
            _booksRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BookItem>()))
                .Callback<BookItem>(x => updatedBookItem = x)
                .ReturnsAsync(Result.Success<Unit, Error>(Unit.Instance));

            //Act
            var result = await _sut.UpdateBookAsync(expectedBookId, expectedBookModel);

            //Assert
            result.IsSuccess.Should().BeTrue();
            updatedBookItem.Should().BeEquivalentTo(expectedBookModel, config => config.Excluding(bi => bi.BookId));
            updatedBookItem.BookId.Should().BeEquivalentTo(expectedBookId);
        }

        [Fact]
        public async Task UpdateBookAsync_WhenFailsToUpdateInStorage_ReturnsFailure()
        {
            //Arrange
            var expectedBookModel = new BookModel
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            _booksRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<BookItem>()))
                .ReturnsAsync(Result.Failure<Unit, Error>(ErrorTypes.InternalServerError));

            //Act
            var result = await _sut.UpdateBookAsync("1", expectedBookModel);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(ErrorTypes.InternalServerError);
        }

        [Fact]
        public async Task CreateBookAsync_WhenBookIsInStorage_ReturnsSuccess()
        {
            //Arrange
            var expectedBookItem = new BookItem
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            //strict matching to make sure mapping is done correctly
            _booksRepositoryMock.Setup(x => x.LoadAsync("1"))
                .ReturnsAsync(Result.Success<BookItem, Error>(expectedBookItem));

            //Act
            var result = await _sut.GetBookAsync("1");

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedBookItem);
        }

        [Fact]
        public async Task GetBookAsync_WhenBookIsNotInStorage_ReturnsFailure()
        {
            //Arrange
            _booksRepositoryMock.Setup(x => x.LoadAsync(It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<BookItem, Error>(ErrorTypes.InternalServerError));

            //Act
            var result = await _sut.GetBookAsync(Guid.NewGuid().ToString());

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(ErrorTypes.InternalServerError);
        }

        [Fact]
        public async Task DeleteBookAsync_WhenDeletedFromStorage_ReturnsSuccess()
        {
            //Arrange
            _booksRepositoryMock.Setup(x => x.DeleteAsync("1"))
                .ReturnsAsync(Result.Success<Unit, Error>(Unit.Instance));

            //Act
            var result = await _sut.DeleteBookAsync("1");

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(Unit.Instance);
        }

        [Fact]
        public async Task DeleteBookAsync_WhenNotAbleToDelete_ReturnsFailure()
        {
            //Arrange
            _booksRepositoryMock.Setup(x => x.LoadAsync(It.IsAny<string>()))
                .ReturnsAsync(Result.Failure<BookItem, Error>(ErrorTypes.InternalServerError));

            //Act
            var result = await _sut.GetBookAsync(Guid.NewGuid().ToString());

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(ErrorTypes.InternalServerError);
        }
    }
}
