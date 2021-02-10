using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookMvc.Models;
using BookMvc.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookMvc.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService bookService;

        public BookController(IBookService _bookService)
        {
            bookService = _bookService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(string searchString)
        {
            var allBooks = await bookService.GetAllBooks();
            var books = allBooks.Value.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                books = (List<Models.Book>)books.Where(s => s.Name.Contains(searchString));
            }

            return View(books);
        }

        
        // GET: Books/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await bookService.GetBookById(id.Value);
            if (book == null)
            {
                return NotFound();
            }

            return View(book.Value);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                await bookService.CreateBook(book);
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await bookService.GetBookById(id.Value);
            if (book == null)
            {
                return NotFound();
            }
            return View(book.Value);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await bookService.UpdateBook(book);

                }
                catch (Exception e)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await bookService.GetBookById(id.Value);

            if (book == null)
            {
                return NotFound();
            }

            return View(book.Value);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var book = await bookService.GetBookById(id.Value);
            if (book == null)
            {
                return NotFound();
            }
            await bookService.DeleteBook(id.Value);

            return RedirectToAction(nameof(Index));
        }

    }
}
