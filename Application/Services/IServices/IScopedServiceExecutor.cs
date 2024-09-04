namespace ApplicationTemplate.Server.Services.IServices
{
    public interface IScopedServiceExecutor
    {
        Task ExecuteAsync<TService>(Func<TService, Task> action) where TService : class;
        Task ExecuteWithScopeAsync(Func<IServiceProvider, Task> action);
        void ExecuteWithScope(Action<IServiceProvider> action);
        void Execute<TService>(Action<TService> action) where TService : class;
    }
}
