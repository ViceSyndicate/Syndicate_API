using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syndicate_API.Models;

namespace Syndicate_API.Controllers
{
    [ApiController]
    [Route("Api/TodoItems")]
    public class TodoItemsController : Controller
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<TodoItem>> GetItems()
        {
            List<TodoItem> todos = await _context.TodoItems.ToListAsync();
            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetItems(int id)
        {
            TodoItem? item = await _context.TodoItems
                .Where(x => x.Id == id).FirstOrDefaultAsync();

            // item != null return item : else return NotFound();
            return item != null ? item : NotFound();
        }

        [HttpPost(Name = "PostTodoItem")]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
