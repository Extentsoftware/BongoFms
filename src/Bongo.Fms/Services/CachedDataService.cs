using Bongo.Client;
using Bongo.Domain.Models;

namespace Bongo.Fms.Services
{
    public class CachedDataService(IBongoApiService client) : ICachedDataService
    {
        private const string SprintsFile = "sprints.json";

        public async Task<List<SprintCoreId>> GetSprintsAsync(CancellationToken cancellationToken)
        {
            var v = await ApiHelper.ExecuteAsync(() => client.GetSprintsAsync(cancellationToken), SprintsFile);
            return v?.Sprints ?? new List<SprintCoreId>();
        }

        public async Task<Sprint?> GetSprintAsync(Guid id, CancellationToken cancellationToken)
        {
            var v = await ApiHelper.ExecuteAsync(() => client.GetSprintAsync(id, cancellationToken));
            return v?.Sprint;
        }
    }
}
