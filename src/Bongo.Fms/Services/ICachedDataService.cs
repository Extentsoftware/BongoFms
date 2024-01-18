using Bongo.Client;
using Bongo.Domain.Models;
using Newtonsoft.Json;

namespace Bongo.Fms.Services
{
    public interface ICachedDataService
    {
        Task<List<SprintCoreId>> GetSprintsAsync(CancellationToken cancellationToken);
        Task<Sprint> GetSprintAsync(Guid id, CancellationToken cancellationToken);
    }
}
