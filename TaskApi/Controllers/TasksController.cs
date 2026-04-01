using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
namespace TaskApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static List<TaskItem> tasks = new()
    {
        new TaskItem {
            Id = 1,
            Title = "Изучить ASP.NET Core",
            Priority = "High",
            IsCompleted = true
        },
        new TaskItem {
            Id = 2,
            Title = "Сделать лабораторную №28",
            Priority = "High",
            IsCompleted = false
        },
        new TaskItem {
            Id = 3,
            Title = "Написать README",
            Priority = "Normal",
            IsCompleted = false
        }
    };

    private static int _nextId = 4;

    // GET /api/tasks
    // GET /api/tasks?completed=true
    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll([FromQuery] bool? completed = null)
    {
        var result = tasks.AsEnumerable();

        // Фильтрация по статусу, если параметр передан
        if (completed.HasValue)
            result = result.Where(t => t.IsCompleted == completed.Value);

        return Ok(result);
    }

    // GET /api/tasks/1
    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task is null)
            return NotFound(new { Message = $"Задача с id={id} не найдена" });
        return Ok(task);
    }

    // POST /api/tasks
    [HttpPost]
    public ActionResult<TaskItem> Create([FromBody] CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest(new { Message = "Поле Title обязательно для заполнения" });

        var newTask = new TaskItem
        {
            Id = _nextId++,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        tasks.Add(newTask);
        return CreatedAtAction(nameof(GetById), new { id = newTask.Id }, newTask);
    }

    // PUT /api/tasks/1
    [HttpPut("{id}")]
    public ActionResult<TaskItem> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null)
            return NotFound(new { Message = $"Задача с id={id} не найдена" });

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest(new { Message = "Поле Title не может быть пустым" });

        // Обновляем поля
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.IsCompleted = dto.IsCompleted;
        task.Priority = dto.Priority;

        return Ok(task);
    }

    // DELETE /api/tasks/1
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null)
            return NotFound(new { Message = $"Задача с id={id} не найдена" });

        tasks.Remove(task);
        return NoContent(); // 204 – успешно, но возвращать нечего
    }

    // PATCH /api/tasks/1/complete
    [HttpPatch("{id}/complete")]
    public ActionResult<TaskItem> MarkComplete(int id) {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null)
            return NotFound(new { Message = $"Задача с id={id} не найдена" });

        task.IsCompleted = !task.IsCompleted; // toggle: true ⇔ false
        return Ok(task);
    }
    
    // GET /api/tasks/search?query=ASP
    [HttpGet("search")]
    public ActionResult<IEnumerable<TaskItem>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { Message = "Параметр query не может быть пустым" });

        var results = tasks
            .Where(t => t.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                    || t.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(results);
    }

    // GET /api/tasks/priority/High
    [HttpGet("priority/{level}")]
    public ActionResult<IEnumerable<TaskItem>> GetByPriority(string level)
    {
        var allowed = new[] { "Low", "Normal", "High" };

        if (!allowed.Contains(level, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { Message = "Допустимые значения: Low, Normal, High" });

        var results = tasks
            .Where(t => t.Priority.Equals(level, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(results);
    }

    // GET /api/tasks/stats
    [HttpGet("stats")]
    public ActionResult GetStats()
    {
        var total = tasks.Count;
        var completed = tasks.Count(t => t.IsCompleted);
        var pending = total - completed;

        var stats = new
        {
            Total = total,
            Completed = completed,
            Pending = pending,
            CompletionPct = total > 0 ? Math.Round((double)completed / total * 100, 1) : 0,
            ByPriority = new
            {
                High = tasks.Count(t => t.Priority == "High"),
                Normal = tasks.Count(t => t.Priority == "Normal"),
                Low = tasks.Count(t => t.Priority == "Low")
            }
        };

        return Ok(stats);
    }

    // GET /api/tasks/sorted?by=priority&desc=true
    [HttpGet("sorted")]
    public ActionResult<IEnumerable<TaskItem>> GetSorted(
        [FromQuery] string by = "id",
        [FromQuery] bool desc = false)
    {
        IEnumerable<TaskItem> sorted = by.ToLower() switch
        {
            "title" => tasks.OrderBy(t => t.Title),
            "priority" => tasks.OrderBy(t => t.Priority),
            "createdAt" => tasks.OrderBy(t => t.CreatedAt),
            _ => tasks.OrderBy(t => t.Id), // по умолчанию — по id
        };

        if (desc)
            sorted = sorted.Reverse();

        return Ok(sorted);
    }
}