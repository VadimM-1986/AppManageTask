using AppManageTasks.Enums;

namespace AppManageTasks.Data.Models
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskProgress Status { get; set; }
    }
}
