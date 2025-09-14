using AppManageTasks.Data;
using Microsoft.EntityFrameworkCore;
using AppManageTasks.Enums;

namespace AppManageTasks.Services.BackgroundServices
{
    public class OverdueTaskBackgroundService : BackgroundService
    {
        private readonly ILogger<OverdueTaskBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public OverdueTaskBackgroundService(
            ILogger<OverdueTaskBackgroundService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = _configuration.GetValue("BackgroundServices:OverdueTaskCheckEnabled", true);
            if (!enabled)
            {
                _logger.LogInformation("Overdue Task Background Service is disabled.");
                return;
            }

            var intervalMinutes = _configuration.GetValue("BackgroundServices:OverdueTaskCheckIntervalMinutes", 1);
            var checkInterval = TimeSpan.FromMinutes(intervalMinutes);

            _logger.LogInformation("Overdue Task Background Service started. Check interval: {Interval}", checkInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndMarkOverdueTasksAsync();
                    await Task.Delay(checkInterval, stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error in Overdue Task Service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task CheckAndMarkOverdueTasksAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var now = DateTime.Now;
            var overdueTasks = await context.UserTasks
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate < now &&
                           t.Status != TaskProgress.Completed &&
                           t.Status != TaskProgress.Overdue)
                .ToListAsync();

            if (overdueTasks.Any())
            {
                foreach (var task in overdueTasks)
                {
                    task.Status = TaskProgress.Overdue;
                    _logger.LogInformation("Task {TaskId} marked as overdue. Due date: {DueDate}",
                        task.Id, task.DueDate);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Marked {Count} tasks as overdue", overdueTasks.Count);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Overdue Task Background Service is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}