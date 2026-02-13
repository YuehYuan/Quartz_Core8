using Quartz;
using System;
using WebSch.Context;

public class QuartzInitializerService : BackgroundService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IServiceProvider _serviceProvider;

    public QuartzInitializerService(ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider)
    {
        _schedulerFactory = schedulerFactory;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(stoppingToken);

        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            var tasks = db.SchedularJob.ToList();

            foreach (var task in tasks)
            {
                var jobKey = new JobKey(task.Hospital, "SPGroup");

                var jobDetail = JobBuilder.Create<SqlSpJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("SpName", task.Type) // 關鍵：將 SP 名稱傳入 Job
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{task.Hospital}-trigger")
                    .WithCronSchedule(task.CronSchedule)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger, stoppingToken);
            }
        }

        await scheduler.Start(stoppingToken);
    }
}