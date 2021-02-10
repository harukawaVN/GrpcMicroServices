using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookGrpcClient.Models;
using BookGrpcClient.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookGrpcClient.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BookController : Controller
    {
        // GET: /<controller>/
        private readonly BookService bookService;

        public BookController(BookService _bookService)
        {
            bookService = _bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAsync()
        {
            var allBooks = await bookService.GetAllBooksAsync();
            var books = allBooks.ToList();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await bookService.GetBookByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            if (ModelState.IsValid)
            {
                var result = await bookService.CreateBookAsync(book);
                if (!result)
                    return BadRequest();
                return Ok(book);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(long id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await bookService.UpdateBookAsync(book);
                    if (!result)
                        return BadRequest();
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
                return Ok(book);
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            if (id == null)
                return NotFound();

            var book = await bookService.GetBookByIdAsync(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            var result = await bookService.DeleteBookAsync(id.Value);
            if (!result)
                return BadRequest();

            return Ok();
        }
    }
}
