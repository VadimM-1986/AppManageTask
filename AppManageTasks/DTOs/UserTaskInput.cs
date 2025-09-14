using AppManageTasks.Validation;
using AppManageTasks.Enums;
using System.ComponentModel.DataAnnotations;

namespace AppManageTasks.DTOs
{
    public class UserTaskInput
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [FutureDate(ErrorMessage = "Due date must be in the future")]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public TaskProgress Status { get; set; }
    }
}
