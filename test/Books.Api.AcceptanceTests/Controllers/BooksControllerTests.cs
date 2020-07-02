using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Books.Api.AcceptanceTests.Helpers;
using Books.Api.AcceptanceTests.Infrastructure;
using Books.Api.Contracts.Common;
using Books.Api.Storage.Model;
using FluentAssertions;
using Xunit;

namespace Books.Api.AcceptanceTests.Controllers
{
    public class BooksControllerTests : AcceptanceTestFixture<StorageFacade>, IDisposable
    {
        [Fact]
        public async Task GetBook_WhenBookExists_ReturnsOk()
        {
            //Arrange
            var storedBook = new BookItem
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            await Facade.InsertBookAsync(storedBook);
            var uri = "api/books/1";

            //Act
            var httpResponse = await HttpClient.GetAsync(uri);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await httpResponse.ReadContentAs<BookModel>();
            responseContent.Should().NotBeNull();
            responseContent.Should().BeEquivalentTo(storedBook);
        }

        [Fact]
        public async Task GetBook_WhenBookDoesNotExist_ReturnsNotFound()
        {
            //Arrange
            var uri = "api/books/xxx";

            //Act
            var httpResponse = await HttpClient.GetAsync(uri);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await httpResponse.ReadContentAs<ErrorResponse>();
            responseContent.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateBook_WhenValidModelIsSent_ReturnsOk()
        {
            //Arrange
            var model = new BookModel
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            var stringContent = new StringContent(model.MapToJson(), Encoding.UTF8, "application/json");
            var uri = "api/books";

            //Act
            var httpResponse = await HttpClient.PostAsync(uri, stringContent);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await httpResponse.ReadContentAs<BookModel>();
            responseContent.Should().NotBeNull();
            var storedBookResult = await Facade.LoadBookAsync(responseContent.BookId);
            storedBookResult.Value.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MemberData(nameof(InvalidBookModels))]
        public async Task CreateBook_WhenInValidModelIsSent_ReturnsBadRequest(BookModel model)
        {
            //Arrange
            var stringContent = new StringContent(model.MapToJson(), Encoding.UTF8, "application/json");
            var uri = "api/books";

            //Act
            var httpResponse = await HttpClient.PostAsync(uri, stringContent);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateBook_WhenValidModelIsSent_ReturnsOk()
        {
            //Arrange
            var storedBook = new BookItem
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            await Facade.InsertBookAsync(storedBook);
            var model = new BookModel
            {
                BookId = "1",
                AuthorId = "000",
                Name = "000"
            };
            var stringContent = new StringContent(model.MapToJson(), Encoding.UTF8, "application/json");
            var uri = "api/books/1";

            //Act
            var httpResponse = await HttpClient.PutAsync(uri, stringContent);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await httpResponse.ReadContentAs<BookModel>();
            responseContent.Should().NotBeNull();
            var storedBookResult = await Facade.LoadBookAsync(model.BookId);
            storedBookResult.Value.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async Task UpdateBook_WhenBookDoesNotExist_ReturnsNotFound()
        {
            var model = new BookModel
            {
                BookId = "xxx",
                Name = "xxx",
                AuthorId = "xxx"
            };
            var uri = "api/books/xxx";

            //Act
            var httpResponse = await HttpClient.PutAsync(uri, new StringContent(model.MapToJson(), Encoding.UTF8, "application/json"));

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await httpResponse.ReadContentAs<ErrorResponse>();
            responseContent.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(InvalidBookModels))]
        public async Task UpdateBook_WhenInValidModelIsSent_ReturnsBadRequest(BookModel model)
        {
            //Arrange
            var stringContent = new StringContent(model.MapToJson(), Encoding.UTF8, "application/json");
            var uri = "api/books/xxx";

            //Act
            var httpResponse = await HttpClient.PutAsync(uri, stringContent);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteBook_WhenBookDoesNotExist_ReturnsNotFound()
        {
            var model = new BookModel
            {
                BookId = "xxx",
                Name = "xxx",
                AuthorId = "xxx"
            };
            var uri = "api/books/xxx";

            //Act
            var httpResponse = await HttpClient.DeleteAsync(uri);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await httpResponse.ReadContentAs<ErrorResponse>();
            responseContent.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteBook_WhenBookExists_ReturnsOk()
        {
            //Arrange
            var storedBook = new BookItem
            {
                BookId = "1",
                AuthorId = "1",
                Name = "1"
            };
            await Facade.InsertBookAsync(storedBook);
            var uri = "api/books/1";

            //Act
            var httpResponse = await HttpClient.DeleteAsync(uri);

            //Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var storedBookResult = await Facade.LoadBookAsync(storedBook.BookId);
            storedBookResult.IsSuccess.Should().BeFalse();
            storedBookResult.Error.Code.Should().Be("errors.storage.item.notfound");
        }
        public static IEnumerable<object[]> InvalidBookModels()
        {
            yield return new object[] { null };
            yield return new object[] { new BookModel
            {
                BookId = "",
                AuthorId = "",
                Name = ""
            }};
        }

        public void Dispose()
        {
            Facade.CleanUp();
        }

        public BooksControllerTests(CompositionRoot compositionRoot) : base(compositionRoot)
        {
        }
    }
}
