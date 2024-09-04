using ApplicationTemplate.Server.Services.IServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Services
{
    public class ScopedServiceExecutor : IScopedServiceExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopedServiceExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Executes an asynchronous action with a scoped service
        public async Task ExecuteAsync<TService>(Func<TService, Task> action) where TService : class
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<TService>();

            try
            {
                await action(service);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("An error occurred while executing the action.", ex);
            }
        }

        // Executes an asynchronous action with a scoped service provider
        public async Task ExecuteWithScopeAsync(Func<IServiceProvider, Task> action)
        {
            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                await action(serviceProvider);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("An error occurred while executing the action with the service provider.", ex);
            }
        }

        // Executes a synchronous action with a scoped service
        public void Execute<TService>(Action<TService> action) where TService : class
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<TService>();

            try
            {
                action(service);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("An error occurred while executing the action.", ex);
            }
        }

        // Executes a synchronous action with a scoped service provider
        public void ExecuteWithScope(Action<IServiceProvider> action)
        {
            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            try
            {
                action(serviceProvider);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new InvalidOperationException("An error occurred while executing the action with the service provider.", ex);
            }
        }
    }
}
