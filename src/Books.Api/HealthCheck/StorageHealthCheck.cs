using System;
using System.Threading;
using System.Threading.Tasks;
using Books.Api.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Books.Api.HealthCheck
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IBooksRepository _booksRepository;

        public StorageHealthCheck(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
               var loadResult = await _booksRepository.LoadAsync("healthcheck");

               if (loadResult.IsFailure)
               {
                   return HealthCheckResult.Unhealthy($"{loadResult.Error.Code} - {loadResult.Error.Message}");
               }

               return HealthCheckResult.Healthy("Healthy");
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy("An exception occured", e);
            }
        }
    }
}
