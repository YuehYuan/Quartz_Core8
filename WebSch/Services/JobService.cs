using Quartz;

public interface IJobService
{
    Task RegisterJobAsync<T>(string jobName, string cronExpression) where T : IJob;
}

public class JobService : IJobService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public JobService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task RegisterJobAsync<T>(string jobName, string cronExpression) where T : IJob
    {
        // 1. 取得排程器實例
        var scheduler = await _schedulerFactory.GetScheduler();

        // 2. 定義 Job
        var jobKey = new JobKey(jobName);
        var jobDetail = JobBuilder.Create<T>()
            .WithIdentity(jobKey)
            .StoreDurably() // 持久化儲存
            .Build();

        // 3. 定義 Trigger
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{jobName}-trigger")
            .ForJob(jobKey)
            .WithCronSchedule(cronExpression) // 使用 Cron 表達式，更彈性
            .Build();

        // 4. 檢查是否已存在，存在則更新，不存在則新增
        if (await scheduler.CheckExists(jobKey))
        {
            await scheduler.RescheduleJob(trigger.Key, trigger);
        }
        else
        {
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}