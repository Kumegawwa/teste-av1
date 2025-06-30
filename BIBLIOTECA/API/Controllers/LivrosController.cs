using API.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/livros")]
public class LivrosController : ControllerBase
{
    private readonly BibliotecaDbContext _context;

    public LivrosController(BibliotecaDbContext context)
    {
        _context = context;
    }

    // POST: api/livros
    [HttpPost]
    public async Task<ActionResult<Livro>> AdicionarLivro([FromBody] Livro livro)
    {
        if (livro.Titulo.Length < 3)
        {
            return BadRequest(new { message = "Título deve ter no mínimo 3 caracteres." });
        }
        if (livro.Autor.Length < 3)
        {
            return BadRequest(new { message = "Autor deve ter no mínimo 3 caracteres." });
        }

        var categoria = await _context.Categorias.FindAsync(livro.CategoriaId);
        if (categoria == null)
        {
            return BadRequest(new { message = "Categoria inválida. O ID da categoria fornecido não existe." });
        }

        livro.Categoria = categoria;
        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(BuscarLivroPorId), new { id = livro.Id }, livro);
    }

    // GET: api/livros
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Livro>>> ListarLivros()
    {
        return await _context.Livros.Include(l => l.Categoria).ToListAsync();
    }

    // GET: api/livros/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> BuscarLivroPorId(int id)
    {
        var livro = await _context.Livros.Include(l => l.Categoria).FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null)
        {
            return NotFound(new { message = $"Livro com ID {id} não encontrado." });
        }

        return Ok(livro);
    }

    // PUT: api/livros/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarLivro(int id, [FromBody] Livro livroAtualizado)
    {
        if (id != livroAtualizado.Id)
        {
            return BadRequest();
        }

        if (livroAtualizado.Titulo.Length < 3)
        {
            return BadRequest(new { message = "Título deve ter no mínimo 3 caracteres." });
        }
        if (livroAtualizado.Autor.Length < 3)
        {
            return BadRequest(new { message = "Autor deve ter no mínimo 3 caracteres." });
        }

        var categoria = await _context.Categorias.FindAsync(livroAtualizado.CategoriaId);
        if (categoria == null)
        {
            return BadRequest(new { message = "Categoria inválida. O ID da categoria fornecido não existe." });
        }

        var livroExistente = await _context.Livros.FindAsync(id);
        if (livroExistente == null)
        {
            return NotFound(new { message = $"Livro com ID {id} não encontrado para atualização." });
        }

        livroExistente.Titulo = livroAtualizado.Titulo;
        livroExistente.Autor = livroAtualizado.Autor;
        livroExistente.CategoriaId = livroAtualizado.CategoriaId;
        livroExistente.Categoria = categoria;

        _context.Entry(livroExistente).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Livros.Any(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(livroExistente);
    }

    // DELETE: api/livros/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoverLivro(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null)
        {
            return NotFound(new { message = $"Livro com ID {id} não encontrado para remoção." });
        }

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}