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

        /// <summary>
        /// Executes the background service loop that periodically checks for overdue tasks.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token to stop the service gracefully.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = _configuration.GetValue("BackgroundServices:OverdueTaskCheckEnabled", true);
            if (!enabled)
            {
                _logger.LogInformation("Фоновая служба просроченных задач отключена.");
                return;
            }

            var intervalMinutes = _configuration.GetValue("BackgroundServices:OverdueTaskCheckIntervalMinutes", 1);
            var checkInterval = TimeSpan.FromMinutes(intervalMinutes);

            _logger.LogInformation("Запущена фоновая служба просроченных задач. Интервал проверки: {Interval}", checkInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndMarkOverdueTasksAsync();
                    await Task.Delay(checkInterval, stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Ошибка в службе просроченных задач");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        /// <summary>
        /// Checks all tasks in the database for overdue status and updates them if necessary.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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
                    _logger.LogInformation("Задача {TaskId} помечена как просроченная. Срок погашения: {DueDate}",
                        task.Id, task.DueDate);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Задача {Count} помечена как просроченная", overdueTasks.Count);
            }
        }

        /// <summary>
        /// Stops the background service and logs shutdown information.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to stop the service gracefully.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Фоновая служба просроченных задач останавливается");
            await base.StopAsync(cancellationToken);
        }
    }
}
