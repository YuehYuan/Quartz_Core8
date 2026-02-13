using System;
using System.Collections.Generic;

namespace WebSch.Context.Entities;

public partial class SchedularJob
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string CronSchedule { get; set; } = null!;

    public string Hospital { get; set; } = null!;

    public string Region { get; set; } = null!;

    public string DbConnection { get; set; } = null!;

    public string GroupId { get; set; } = null!;

    public string? ServerName { get; set; }
}
