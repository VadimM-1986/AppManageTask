using AppManageTasks.Repository;
using AppManageTasks.Services;

namespace AppManageTasks.Module
{
    public class AppManageTasksModule
    {
        public static void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUserTaskService, UserTaskService>();
            services.AddTransient<IUserTaskRepository, UserTaskRepository>();

            services.AddHttpClient<ICurrencyService, CurrencyService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
        }
    }
}
