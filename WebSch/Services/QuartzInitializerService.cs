using Quartz;
using WebSch.Context;

public class QuartzInitializerService : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuartzInitializerService> _logger;

    public QuartzInitializerService(
        ISchedulerFactory schedulerFactory,
        IServiceProvider serviceProvider,
        ILogger<QuartzInitializerService> logger)
    {
        _schedulerFactory = schedulerFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("正在從資料庫載入排程任務...");

        // 1. 取得排程器
        var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

        // 2. 建立 Scope 以使用 DbContext (如果是 Scoped 服務)
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            // 假設您的資料表叫做 SchedularJob
            var jobSettings = dbContext.SchedularJob.ToList();

            foreach (var setting in jobSettings)
            {
                try
                {
                    // 根據資料庫中的 JobClassName 字串動態取得 Type
                    var jobType = Type.GetType(setting.Type);
                    if (jobType == null) continue;

                    var jobKey = new JobKey(setting.GroupId, "DatabaseGroup");

                    // 定義 Job
                    var jobDetail = JobBuilder.Create(jobType)
                        .WithIdentity(jobKey)
                        .WithDescription(setting.Region)
                        .Build();

                    // 定義 Trigger (使用資料庫裡的 Cron 表達式)
                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"{setting.Id}-trigger")
                        .WithCronSchedule(setting.CronSchedule)
                        .Build();

                    // 註冊進排程器
                    await scheduler.ScheduleJob(jobDetail, trigger, stoppingToken);

                    _logger.LogInformation("已載入任務: {JobName}", setting.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "載入任務 {JobName} 失敗", setting.Id);
                }
            }
        }

        // 啟動排程器
        await scheduler.Start(stoppingToken);
    }
}