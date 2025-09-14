using AppManageTasks.Data;
using AppManageTasks.Data.Models;
using AppManageTasks.DTOs;
using AppManageTasks.DTOs.PageQueryParams;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AppManageTasks.Enums;

namespace AppManageTasks.Repository
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserTaskRepository> _logger;


        public UserTaskRepository(AppDbContext AppDbContext, IMapper mapper, ILogger<UserTaskRepository> logger)
        {
            _context = AppDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserTask> CreateUserTaskAsync(UserTaskInput userTaskInput)
        {
            var userTask = _mapper.Map<UserTask>(userTaskInput);
            _context.UserTasks.Add(userTask);
            await _context.SaveChangesAsync();
            return userTask;
        }

        public async Task<PagedResult<UserTaskSummary>> GetPagedUserTasksAsync(int page, int pageSize, TaskProgress? status)
        {
            var query = _context.UserTasks.AsQueryable();

            // Фильтрация по статусу
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            // Получаем общее количество записей
            var totalCount = await query.CountAsync();

            // Применяем пагинацию
            var items = await query
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue) // Сортировка по дате выполнения
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<UserTaskSummary>
            {
                Items = _mapper.Map<List<UserTaskSummary>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<UserTaskSummary>> GetAllUserTaskAsync()
        {
            var userTasks = await _context.UserTasks.ToListAsync();
            return _mapper.Map<List<UserTaskSummary>>(userTasks);
        }

        public async Task<UserTaskDetail?> GetUserTaskByIdAsync(Guid id)
        {
            _logger.LogDebug("Запрос к БД: получение задачи по ID: {TaskId}", id);

            var userTask = await _context.UserTasks.FindAsync(id);

            if (userTask == null)
            {
                _logger.LogDebug("Задача не найдена в БД: {TaskId}", id);
                return null;
            }

            _logger.LogDebug("Задача найдена в БД: {TaskId}, маппинг в DTO", id);
            return _mapper.Map<UserTaskDetail>(userTask);
        }

        public async Task<bool> UpdateUserTaskAsync(UserTaskInput userTaskInput)
        {
            var userTask = await _context.UserTasks.FindAsync(userTaskInput.Id);

            if (userTask == null)
                return false;

            userTask.Title = userTaskInput.Title;
            userTask.Description = userTaskInput.Description;
            userTask.DueDate = userTaskInput.DueDate;
            userTask.Status = userTaskInput.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserTask(Guid id)
        {
            var userTask = await _context.UserTasks
                .FirstOrDefaultAsync(u => u.Id == id);

            if (userTask == null)
                return false;

            _context.UserTasks.Remove(userTask);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
