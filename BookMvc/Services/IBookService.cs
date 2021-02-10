using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookMvc.Services
{
    public interface IBookService
    {
        public Task<ActionResult<IEnumerable<Book>>> GetAllBooks();
        public Task<ActionResult<Book>> GetBookById(long id);
        public Task<bool> CreateBook(Book book);
        public Task<bool> UpdateBook(Book book);
        public Task<bool> DeleteBook(long id);
    }
}
