using AppManageTasks.Data.Models;
using AppManageTasks.DTOs;
using AutoMapper;

namespace AppManageTasks.Mapping
{
    public class UserTaskMappingProfile : Profile
    {
        public UserTaskMappingProfile() 
        {
            CreateMap<UserTask, UserTaskSummary>();
            CreateMap<UserTask, UserTaskDetail>();
            CreateMap<UserTaskInput, UserTask>();
        }
    }
}
