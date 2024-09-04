using Microsoft.AspNetCore.Components;
using System.Net;

namespace Client.Middlewares
{
    public class ClientMiddleware : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;
        public Func<HttpRequestMessage?, Task> ReadSessionAsyncFunc;

        public ClientMiddleware(HttpMessageHandler innerHandler, Func<HttpRequestMessage?, Task> readSessionFunc, NavigationManager navigationManager)
        {
            ReadSessionAsyncFunc = readSessionFunc;
            InnerHandler = innerHandler;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                await ReadSessionAsyncFunc.Invoke(request);

                cancellationToken.ThrowIfCancellationRequested();

                var response = await base.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode is
                        HttpStatusCode.InternalServerError or
                        HttpStatusCode.TooManyRequests or
                        HttpStatusCode.Unauthorized or
                        HttpStatusCode.BadRequest)
                    {
                        _navigationManager.NavigateTo($"/error?message={Uri.EscapeDataString(errorMessage)}");
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _navigationManager.NavigateTo($"/error?message={Uri.EscapeDataString(ex.Message)}");
                throw;
            }
        }
    }
}
