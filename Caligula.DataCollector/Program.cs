using Caligula.Web.ApiClients;
using Quartz;
using Quartz.Impl;
using System.Reflection.Metadata;

public class Program
{
    public static async Task Main(string[] args)
    {
        IJobDetail job = JobBuilder.Create<MatchHistoryJob>()
            .WithIdentity("matchHistoryJob", "group1")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("matchHistoryTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();

        // Schedule the job using a scheduler
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();
        await scheduler.ScheduleJob(job, trigger);
    }
}

public class MatchHistoryJob : IJob
{
    private readonly MatchHistoryApiClient _apiClient;

    public MatchHistoryJob(MatchHistoryApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _apiClient.RunDailyMatchHistoryUpdateAsync();
    }
}
