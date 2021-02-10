using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BookMvc.Extension;
using BookMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookMvc.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient client;
        public BookService(HttpClient _client)
        {
            client = _client;
        }

        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks() {
            var response = await client.GetAsync("api/v1/Book");
            return await response.ReadContentAs<List<Book>>();

        }
        public async Task<ActionResult<Book>> GetBookById(long id)
        {
            var response = await client.GetAsync($"api/v1/Book/{id}");
            return await response.ReadContentAs<Book>();
        }
        public async Task<bool> CreateBook(Book book)
        {
            var response = await client.PostAsJson($"api/v1/Book", book);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
        public async Task<bool> UpdateBook(Book book)
        {
            var response = await client.PutAsJson($"api/v1/Book/{book.Id}", book);
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> DeleteBook(long id)
        {
            var response = await client.DeleteAsync($"api/v1/Book/{id}");
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
