using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookGrpcServer;
//using BookGrpcServer.Mapper;
using BookGrpcServer.Models;
using BookGrpcService;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookGrpcServer.Services
{
    public class BookService : BookServices.BookServicesBase
    {
        private dbBooksContext bookContext;
        //private BookMapper mapper;
        //private DbContextOptions<dbBooksContext> dbContextOptions;
        private ILogger<BookService> logger;

        public BookService(
            dbBooksContext _bookContext,
            //BookMapper _mapper,
            //DbContextOptions<dbBooksContext> _dbContextOptionns,
            ILogger<BookService> _logger)
        {
            bookContext = _bookContext ?? throw new ArgumentNullException(nameof(bookContext));
            //mapper = _mapper;
            //dbContextOptions = _dbContextOptionns ?? throw new ArgumentNullException(nameof(dbContextOptions));
            logger = _logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<CustomerListBookResponse> GetAllBooks(VoidRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for all books", context.Method);
            //using ( var dbcontext = new dbBooksContext(dbContextOptions))
            //{
                CustomerListBookResponse response = new CustomerListBookResponse();
                var datas = bookContext.Books;

                foreach (var item in datas)
                {
                    response.Books.Add(new CustomerBookDataReponse
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,
                        Category = item.Category,
                        Author = item.Author,
                    });
                }

                return Task.FromResult(response);

            //}
        }

        public override async Task<CustomerBookDataReponse> GetBookById(CustomerGetOrDeleteBookRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for book id {Id}", context.Method, request.Id);

            //using (var dbcontext = new dbBooksContext(dbContextOptions))
            //{
                var data = await bookContext.Books.FindAsync(request.Id);

                if (data != null)
                {
                    context.Status = new Status(StatusCode.OK, $"Book with id {request.Id} do exist");

                    return MapToCustomerBooktResponse(data);
                }
                else
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.Id} do not exist");
                }

                return new CustomerBookDataReponse();
            //}
        }

        public override async Task<CustomerBoolBookResponse> CreateBook(CustomerCreateOrUpdateBookRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for book Id {Id}", context.Method, request.BookData.Id);

            //using (var dbcontext = new dbBooksContext(dbContextOptions))
            //{

                var existeBook = await bookContext.Books.FindAsync(request.BookData.Id);

                if (existeBook != null)
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.BookData.Id} do not exist");
                    return new CustomerBoolBookResponse { Error = false };
                }

                var book = MapToModelBook(request);

                await bookContext.AddAsync(book);

                try
                {
                    await bookContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!BookExists(bookContext, book.Id))
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with Name {request.BookData.Id} do not create");
                    return new CustomerBoolBookResponse { Error = false };
                }

                context.Status = new Status(StatusCode.OK, $"Book with Name {request.BookData.Id} do create");

                return new CustomerBoolBookResponse { Error = true };
            //}

        }

        public override async Task<CustomerBoolBookResponse> UpdateBook(CustomerCreateOrUpdateBookRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for book id {Id}", context.Method, request.BookData.Id);

            //using (var dbcontext = new dbBooksContext(dbContextOptions))
            //{
                var currentBook = await bookContext.Books.FindAsync(request.BookData.Id);

                if (currentBook == null)
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.BookData.Id} do not exist");
                    return new CustomerBoolBookResponse { Error = false };
                }

                currentBook.Name = request.BookData.Name;
                currentBook.Category = request.BookData.Category;
                currentBook.Author = request.BookData.Author;
                currentBook.Price = request.BookData.Price;

            bookContext.Books.Update(currentBook);

                try
                {
                    await bookContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!BookExists(bookContext, currentBook.Id))
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.BookData.Id} do not update");
                    return new CustomerBoolBookResponse { Error = false };
                }

                context.Status = new Status(StatusCode.OK, $"Book with id {request.BookData.Id} do update");

                return new CustomerBoolBookResponse { Error = true };
            //}

        }

        public override async Task<CustomerBoolBookResponse> DeleteBook(CustomerGetOrDeleteBookRequest request, ServerCallContext context)
        {
            logger.LogInformation("Begin grpc call from method {Method} for book id {Id}", context.Method, request.Id);

            //using (var dbcontext = new dbBooksContext(dbContextOptions))
            //{
                var currentBook = await bookContext.Books.FindAsync(request.Id);

                if (currentBook == null)
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.Id} do not exist");
                    return new CustomerBoolBookResponse { Error = false };
                }

            bookContext.Books.Remove(currentBook);

                try
                {
                    await bookContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    context.Status = new Status(StatusCode.NotFound, $"Book with id {request.Id} do not deelete");
                    return new CustomerBoolBookResponse { Error = false };
                }

                context.Status = new Status(StatusCode.OK, $"Book with id {request.Id} do delete");

                return new CustomerBoolBookResponse { Error = true };
            //}
        }

        private CustomerBookDataReponse MapToCustomerBooktResponse(Book book)
        {
            var reponse = new CustomerBookDataReponse();

            reponse.Id = book.Id;
            reponse.Name = book.Name;
            reponse.Price = book.Price;
            reponse.Category = book.Category;
            reponse.Author = book.Author;

            return reponse;
        }

        private Book MapToModelBook(CustomerCreateOrUpdateBookRequest request)
        {
            var book = new Book();
            book.Id = request.BookData.Id;
            book.Name = request.BookData.Name;
            book.Price = request.BookData.Price;
            book.Category = request.BookData.Category;
            book.Author = request.BookData.Author;

            return book;
        }

        private bool BookExists(dbBooksContext bookContext, long id) =>
            bookContext.Books.Any(e => e.Id == id);
    }


}
