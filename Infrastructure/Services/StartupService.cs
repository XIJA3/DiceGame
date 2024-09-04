using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Infrastructure.Services
{
    public class StartupService(EnumInitializationService enumService) : IHostedService
    {
        private readonly EnumInitializationService _enumService = enumService;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _enumService.InitializeEnums();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // No cleanup needed in this example
            return Task.CompletedTask;
        }
    }
}
