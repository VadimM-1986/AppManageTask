using AppManageTasks.Mapping;

namespace AppManageTasks.Module
{
    public static class MapperProfile
    {
        public static Type[] Profiles { get; } =
        {
            typeof(UserTaskMappingProfile)
        };
    }
}
