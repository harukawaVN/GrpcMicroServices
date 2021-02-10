using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using BookGrpcService;
using Grpc.Net.Client;
using BookGrpcClient.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BookGrpcClient.Services
{
    public class BookService : BookServices.BookServicesClient
    {
        public readonly HttpClient httpClient;

        public BookService(HttpClient _httpClient)
        {
            httpClient = _httpClient;
        }

        private BookServices.BookServicesClient client()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress(httpClient.BaseAddress.ToString());

            return new BookServices.BookServicesClient(channel);
        }

        public async Task<IList<Book>> GetAllBooksAsync()
        {
            List<Book> listBooks = new List<Book>();
            CustomerListBookResponse response = await client().GetAllBooksAsync(new VoidRequest());
            foreach(var item in response.Books)
            {
                var book = new Book
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = item.Category,
                    Author = item.Author,
                    Price = item.Price
                };
                listBooks.Add(book);
            }
            return listBooks;
        }

        public async Task<Book> GetBookByIdAsync(long id)
        {
            var response = await client().GetBookByIdAsync(new CustomerGetOrDeleteBookRequest { Id = id });
            return MapToBooktData(response);
        }

        public async Task<bool> UpdateBookAsync([FromBody] Book currentBook)
        {
            var request = MapToCustomerRequest(currentBook);
            var response = await client().UpdateBookAsync(request);
            return response.Error;
        }

        public async Task<bool> CreateBookAsync([FromBody] Book newBook)
        {
            var request = MapToCustomerRequest(newBook);
            var response = await client().CreateBookAsync(request);
            return response.Error;
        }

        public async Task<bool> DeleteBookAsync(long id)
        {
            var response = await client().DeleteBookAsync(new CustomerGetOrDeleteBookRequest { Id = id});
            return response.Error;
        }

        Book MapToBooktData(CustomerBookDataReponse response)
        {
            var book = new Book();
            book.Id = response.Id;
            book.Name = response.Name;
            book.Price = response.Price;
            book.Category = response.Category;
            book.Author = response.Author;

            return book;
        }

        CustomerCreateOrUpdateBookRequest MapToCustomerRequest(Book newBook)
        {
            var request = new CustomerCreateOrUpdateBookRequest();

            var bookData = new CustomerBookDataReponse();

            bookData.Id = newBook.Id;
            bookData.Name = newBook.Name;
            bookData.Price = newBook.Price;
            bookData.Category = newBook.Category;
            bookData.Author = newBook.Author;
            request.BookData = bookData;
            return request;
        }
    }
}
