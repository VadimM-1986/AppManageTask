using AppManageTasks.Data.Models;
using AppManageTasks.DTOs;
using AppManageTasks.DTOs.PageQueryParams;
using AppManageTasks.Repository;
using AppManageTasks.Enums;

namespace AppManageTasks.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly ILogger<UserTaskService> _logger;

        public UserTaskService(IUserTaskRepository userTaskRepository, ILogger<UserTaskService> logger)
        {
            _userTaskRepository = userTaskRepository;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user task.
        /// </summary>
        /// <param name="userTaskInput">The input data for the new task.</param>
        /// <returns>The created <see cref="UserTask"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown if title is empty or due date is in the past.</exception>
        public async Task<UserTask> CreateUserTaskAsync(UserTaskInput userTaskInput)
        {
            if (string.IsNullOrWhiteSpace(userTaskInput.Title))
                throw new ArgumentException("Title cannot be empty");

            if (userTaskInput.DueDate.HasValue && userTaskInput.DueDate < DateTime.Now)
                throw new ArgumentException("Due date cannot be in the past");

            return await _userTaskRepository.CreateUserTaskAsync(userTaskInput);
        }

        /// <summary>
        /// Retrieves a paged list of user tasks.
        /// </summary>
        /// <param name="queryParams">Paging and filtering parameters.</param>
        /// <returns>A <see cref="PagedResult{UserTaskSummary}"/> containing tasks.</returns>
        public async Task<PagedResult<UserTaskSummary>> GetPagedUserTasksAsync(TaskQueryParams queryParams)
        {
            if (queryParams.Page < 1) queryParams.Page = 1;
            if (queryParams.PageSize < 1) queryParams.PageSize = 10;
            if (queryParams.PageSize > 100) queryParams.PageSize = 100;

            return await _userTaskRepository.GetPagedUserTasksAsync(
                queryParams.Page,
                queryParams.PageSize,
                queryParams.Status
            );
        }

        /// <summary>
        /// Retrieves all user tasks.
        /// </summary>
        /// <returns>A list of <see cref="UserTaskSummary"/> objects.</returns>
        public async Task<List<UserTaskSummary>> GetAllUserTasksAsync()
        {
            var tasks = await _userTaskRepository.GetAllUserTaskAsync();

            return tasks;
        }

        /// <summary>
        /// Retrieves a user task by its unique identifier.
        /// </summary>
        /// <param name="id">The task ID.</param>
        /// <returns>The <see cref="UserTaskDetail"/> object if found.</returns>
        /// <exception cref="ArgumentException">Thrown if the ID is empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the task is not found.</exception>
        public async Task<UserTaskDetail?> GetUserTaskByIdAsync(Guid id)
        {
            _logger.LogDebug("Получение задачи по ID: {TaskId}", id);

            if (id == Guid.Empty)
            {
                _logger.LogWarning("Попытка получить задачу с пустым GUID");
                throw new ArgumentException("Invalid task ID");
            }

            var task = await _userTaskRepository.GetUserTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Задача не найдена: {TaskId}", id);
                throw new KeyNotFoundException($"Task with ID {id} not found");
            }

            _logger.LogInformation("Задача получена: {TaskId}, Название: {TaskTitle}",
                id, task.Title);

            return task;
        }

        /// <summary>
        /// Updates an existing user task.
        /// </summary>
        /// <param name="userTaskInput">The task data to update.</param>
        /// <returns>True if the update is successful.</returns>
        /// <exception cref="ArgumentException">Thrown if task ID is empty or title is empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if task is not found.</exception>
        public async Task<bool> UpdateUserTaskAsync(UserTaskInput userTaskInput)
        {
            if (userTaskInput.Id == Guid.Empty)
                throw new ArgumentException("Invalid task ID");

            if (string.IsNullOrWhiteSpace(userTaskInput.Title))
                throw new ArgumentException("Title cannot be empty");

            var existingTask = await _userTaskRepository.GetUserTaskByIdAsync(userTaskInput.Id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Task with ID {userTaskInput.Id} not found");

            if (userTaskInput.Status == TaskProgress.Completed && !userTaskInput.DueDate.HasValue)
            {
                userTaskInput.DueDate = DateTime.Now;
            }

            return await _userTaskRepository.UpdateUserTaskAsync(userTaskInput);
        }

        /// <summary>
        /// Deletes a user task by its identifier.
        /// </summary>
        /// <param name="id">The task ID.</param>
        /// <returns>True if deletion is successful.</returns>
        /// <exception cref="ArgumentException">Thrown if task ID is empty.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if task is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the task is in progress and cannot be deleted.</exception>
        public async Task<bool> DeleteUserTaskAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid task ID");

            var existingTask = await _userTaskRepository.GetUserTaskByIdAsync(id);
            if (existingTask == null)
                throw new KeyNotFoundException($"Task with ID {id} not found");

            if (existingTask.Status == TaskProgress.InProgress)
                throw new InvalidOperationException("Cannot delete task in progress");

            return await _userTaskRepository.DeleteUserTask(id);
        }


    }
}