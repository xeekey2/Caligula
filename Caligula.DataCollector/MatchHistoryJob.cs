using Quartz;
using Caligula.Web.ApiClients;
using System.Threading.Tasks;

namespace Caligula.DataCollector
{
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

}
