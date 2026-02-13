using Microsoft.EntityFrameworkCore;
using Quartz;
using WebSch.Context;

var connectionString = "Server=TWTPEN0311027B;;UID=sa;PWD=Aa123456;Database=Dimension;TrustServerCertificate=True;";

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// 1. 註冊 Quartz 基本服務 (不需在這邊寫 AddJob, 因為我們要動態加)
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});

// 2. 註冊 Quartz 的託管服務 (負責管理 Scheduler 生命週期)
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// 3. 註冊我們自訂的初始化服務 (負責從資料庫讀取並排程)
builder.Services.AddHostedService<QuartzInitializerService>();

var host = builder.Build();
host.Run();

// --- Job 實作類別 ---
public class MyJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"任務執行中: {DateTime.Now}");
        return Task.CompletedTask;
    }
}