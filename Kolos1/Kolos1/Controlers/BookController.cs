using Kolos1.Models.DTOs;
using Kolos1.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolos1.Controlers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookRepository _bookRepository;

    public BooksController(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    [Route("{id}/authors")]
    public async Task<IActionResult> getBook(int id)
    {
        if (!await _bookRepository.doesBookExists(id))
        {
            return NotFound($"Book with that given id - {id} does not exist");
        }

        var book = await _bookRepository.getBook(id);
        return Ok(book);
    }

    [HttpPost]
    
    public async Task<CreatedResult> addBook([FromBody] NewBookDTO newBookDTO)
    {
        await _bookRepository.addBook(newBookDTO);
        return Created("", null);
    }
}