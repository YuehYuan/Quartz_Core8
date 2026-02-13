using Microsoft.EntityFrameworkCore;
using Quartz;
using System;
using WebSch.Context;

var builder = Host.CreateApplicationBuilder(args);

// 1. 設定資料庫 (EF Core)
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. 設定 Quartz 基礎服務
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
});

// 3. 設定 Quartz 託管服務 (負責生命週期)
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

// 4. 註冊我們的自動化初始化服務
builder.Services.AddHostedService<QuartzInitializerService>();

var host = builder.Build();
host.Run();