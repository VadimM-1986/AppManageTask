using AppManageTasks.Enums;

namespace AppManageTasks.DTOs
{
    public class UserTaskSummary
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskProgress Status { get; set; }
    }
}
