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
    public ActionResult<TaskItem> MarkComplete(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);

        if (task is null)
            return NotFound(new { Message = $"Задача с id={id} не найдена" });

        task.IsCompleted = !task.IsCompleted; // toggle: true ⇔ false
        return Ok(task);
    }
}