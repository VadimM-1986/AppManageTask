using AppManageTasks.DTOs;
using AppManageTasks.DTOs.PageQueryParams;
using AppManageTasks.Services;
using Microsoft.AspNetCore.Mvc;
using AppManageTasks.Enums;

namespace AppManageTasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly ILogger<UserTaskController> _logger;

        public UserTaskController(IUserTaskService userTaskService, ILogger<UserTaskController> logger)
        {
            _userTaskService = userTaskService;
            _logger = logger;
        }

        /// <summary>
        /// Получить задачу по ID
        /// </summary>
        /// <param name="id">Идентификатор задачи (GUID)</param>
        /// <returns>Детальная информация о задаче</returns>
        /// <response code="200">Задача найдена</response>
        /// <response code="400">Неверный идентификатор задачи</response>
        /// <response code="404">Задача не найдена</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserTaskDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserTaskDetail>> GetTaskById(
            [FromRoute] Guid id,
            [FromQuery] bool includeDetails = false)
        {
            _logger.LogInformation("Запрос задачи по ID: {TaskId}, IncludeDetails: {IncludeDetails}",
                id, includeDetails);

            try
            {
                var task = await _userTaskService.GetUserTaskByIdAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Задача не найдена: {TaskId}", id);
                    return NotFound($"Task with ID {id} not found");
                }

                _logger.LogInformation("Задача найдена: {TaskId}, Title: {TaskTitle}, Status: {Status}",
                    id, task.Title, task.Status);

                return Ok(task);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Неверный идентификатор задачи: {TaskId}", id);
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Задача не найдена: {TaskId}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при получении задачи: {TaskId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить задачи с пагинацией и фильтрацией
        /// </summary>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10, максимум 100)</param>
        /// <param name="status">Статус задачи для фильтрации (опционально)</param>
        /// <returns>Пагинированный список задач</returns>
        /// <response code="200">Возвращает пагинированный список задач</response>
        /// <response code="400">Неверные параметры запроса</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResult<UserTaskSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<UserTaskSummary>>> GetPagedTasks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] TaskProgress? status = null)
        {
            try
            {
                var queryParams = new TaskQueryParams
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = status
                };

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userTaskService.GetPagedUserTasksAsync(queryParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новую задачу
        /// </summary>
        /// <param name="userTaskInput">Данные для создания задачи</param>
        /// <returns>Созданная задача</returns>
        /// <response code="201">Задача успешно создана</response>
        /// <response code="400">Неверные данные задачи</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserTaskDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserTaskDetail>> CreateTask(
            [FromBody] UserTaskInput userTaskInput)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdTask = await _userTaskService.CreateUserTaskAsync(userTaskInput);

                return CreatedAtAction(nameof(GetTaskById),
                    new { id = createdTask.Id },
                    createdTask);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить задачу
        /// </summary>
        /// <param name="id">Идентификатор задачи (GUID)</param>
        /// <param name="userTaskInput">Данные для обновления задачи</param>
        /// <response code="204">Задача успешно обновлена</response>
        /// <response code="400">Неверные данные или идентификатор</response>
        /// <response code="404">Задача не найдена</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateTask(
            [FromRoute] Guid id,
            [FromBody] UserTaskInput userTaskInput)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != userTaskInput.Id)
                    return BadRequest("ID in route does not match ID in request body");

                var result = await _userTaskService.UpdateUserTaskAsync(userTaskInput);

                if (!result)
                    return NotFound($"Task with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        /// <param name="id">Идентификатор задачи (GUID)</param>
        /// <response code="204">Задача успешно удалена</response>
        /// <response code="400">Неверный идентификатор задачи</response>
        /// <response code="404">Задача не найдена</response>
        /// <response code="409">Невозможно удалить задачу в текущем статусе</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTask(
            [FromRoute] Guid id)
        {
            try
            {
                var result = await _userTaskService.DeleteUserTaskAsync(id);

                if (!result)
                    return NotFound($"Task with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить задачи по статусу
        /// </summary>
        /// <param name="status">Статус задачи для фильтрации</param>
        /// <returns>Список задач с указанным статусом</returns>
        /// <response code="200">Возвращает отфильтрованный список задач</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(List<UserTaskSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserTaskSummary>>> GetTasksByStatus(
            [FromRoute] TaskProgress status)
        {
            try
            {
                var allTasks = await _userTaskService.GetAllUserTasksAsync();
                var filteredTasks = allTasks.Where(t => t.Status == status).ToList();

                return Ok(filteredTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить просроченные задачи
        /// </summary>
        /// <returns>Список просроченных задач</returns>
        /// <response code="200">Возвращает список просроченных задач</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(List<UserTaskSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserTaskSummary>>> GetOverdueTasks()
        {
            try
            {
                var allTasks = await _userTaskService.GetAllUserTasksAsync();
                var overdueTasks = allTasks
                    .Where(t => t.DueDate.HasValue &&
                               t.DueDate < DateTime.Now &&
                               t.Status != TaskProgress.Completed)
                    .ToList();

                return Ok(overdueTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Получить задачи с истекающим сроком
        /// </summary>
        /// <param name="hours">Количество часов до дедлайна (по умолчанию 24)</param>
        /// <returns>Список задач с истекающим сроком</returns>
        /// <response code="200">Возвращает список задач с истекающим сроком</response>
        /// <response code="500">Произошла внутренняя ошибка сервера</response>
        [HttpGet("upcoming-deadlines")]
        [ProducesResponseType(typeof(List<UserTaskSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserTaskSummary>>> GetUpcomingDeadlines(
            [FromQuery] int hours = 24)
        {
            try
            {
                var allTasks = await _userTaskService.GetAllUserTasksAsync();
                var deadline = DateTime.Now.AddHours(hours);

                var upcomingTasks = allTasks
                    .Where(t => t.DueDate.HasValue &&
                               t.DueDate > DateTime.Now &&
                               t.DueDate <= deadline &&
                               t.Status != TaskProgress.Completed)
                    .ToList();

                return Ok(upcomingTasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}