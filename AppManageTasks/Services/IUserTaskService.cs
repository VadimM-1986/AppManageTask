using AppManageTasks.Data.Models;
using AppManageTasks.DTOs;
using AppManageTasks.DTOs.PageQueryParams;

namespace AppManageTasks.Services
{
    public interface IUserTaskService
    {
        Task<UserTask> CreateUserTaskAsync(UserTaskInput userTaskInput);
        Task<PagedResult<UserTaskSummary>> GetPagedUserTasksAsync(TaskQueryParams queryParams);
        Task<List<UserTaskSummary>> GetAllUserTasksAsync();
        Task<UserTaskDetail?> GetUserTaskByIdAsync(Guid id);
        Task<bool> UpdateUserTaskAsync(UserTaskInput userTaskInput);
        Task<bool> DeleteUserTaskAsync(Guid id);
    }
}
