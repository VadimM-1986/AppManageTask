using AppManageTasks.Data.Models;
using AppManageTasks.DTOs;
using AppManageTasks.DTOs.PageQueryParams;
using AppManageTasks.Enums;

namespace AppManageTasks.Repository
{
    public interface IUserTaskRepository
    {
        Task<UserTask> CreateUserTaskAsync(UserTaskInput userTaskInput);
        Task<PagedResult<UserTaskSummary>> GetPagedUserTasksAsync(int page, int pageSize, TaskProgress? status);
        Task<List<UserTaskSummary>> GetAllUserTaskAsync();
        Task<UserTaskDetail?> GetUserTaskByIdAsync(Guid id);
        Task<bool> UpdateUserTaskAsync(UserTaskInput userTaskInput);
        Task<bool> DeleteUserTask(Guid id);
    }
}
