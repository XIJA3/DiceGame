using System.Net;

namespace Client.Middlewares
{
    public class ClientMiddleware : DelegatingHandler
    {
        public Func<HttpRequestMessage?, Task> ReadSessionAsyncFunc;

        public ClientMiddleware(HttpMessageHandler innerHandler, Func<HttpRequestMessage?, Task> readSessionFunc)
        {
            ReadSessionAsyncFunc = readSessionFunc;
            InnerHandler = innerHandler;
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
                        //throw new HandledException(errorMessage);
                        throw new Exception(errorMessage);
                }

                return response;
            }
            catch (Exception ex)
            {
                //throw new HandledException(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
