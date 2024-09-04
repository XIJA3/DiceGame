//using Azure.Core;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Middleware;
//using System.Security.Claims;

//namespace Web.Middlewares;

//public class HttpContextAccessorMiddleware(IHttpContextAccessor httpContextAccessor) : IFunctionsWorkerMiddleware
//{
//    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
//    {
//        var request = await context.GetHttpRequestDataAsync();

//        if (request != null)
//        {
//            var claimsPrincipal = new ClaimsPrincipal(request.Identities);

//            if (httpContextAccessor.HttpContext is not null)
//                httpContextAccessor.HttpContext.User = claimsPrincipal;
//        }

//        await next(context);
//    }
//}