using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.Primitives;
using ApplicationTemplate.Server.Common.Security;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authentication;

namespace Web.Helpers;

// Todo: Move cache in it's own middleware 

public static class MiddlewareExtensions
{
    public async static Task<bool> RequiresAuthorization(HttpContext context, MethodInfo? hubMethodInfo = null)
    {
        //// Check if the context is in negotiation
        //if (context.GetEndpoint()!.DisplayName!.Equals("/gameHub/negotiate"))
        //{
        //    return false;
        //}

        // Check if the endpoint itself has an AuthorizeAttribute
        var endpoint = context.GetEndpoint();
        var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
        if (authorizeAttribute != null)
        {
            return true;
        }

        // Check if the endpoint is a hub method and if the method has AuthorizeAttribute
        if (hubMethodInfo != null)
        {
            var hubMethodAuthorizeAttribute = hubMethodInfo.GetCustomAttribute<AuthorizeAttribute>();
            if (hubMethodAuthorizeAttribute != null)
            {
                return true;
            }
        }

        // If the endpoint is a controller action, check the controller type for AuthorizeAttribute
        var controllerActionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (controllerActionDescriptor != null)
        {
            var controllerType = controllerActionDescriptor.ControllerTypeInfo;
            if (controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0)
            {
                return true;
            }

            // Check if the action method has AuthorizeAttribute
            var actionMethodInfo = controllerActionDescriptor.MethodInfo;
            if (actionMethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0)
            {
                return true;
            }
        }

        return false;
    }

 
    //public static async Task ValidateRateLimit(HttpContext context)
    //{
    //    // Check if it has rate limit attribute at all
    //    if (!context.HasRateLimitAttribute(out var decorator))
    //        return;

    //    // If the request has a rate limit, we will fetch the customer's consumption data
    //    var consumptionData = await _cache.GetCustomerConsumptionDataFromContextAsync(context);
    //    if (consumptionData is not null)
    //    {
    //        // If the customer has consumption data, we will check if he has reached the rate limit
    //        if (consumptionData.HasConsumedAllRequests(decorator!.TimeWindowInSeconds, decorator!.MaxRequests))
    //        {
    //            // Upon reaching the rate limit, we return too many requests status
    //            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

    //            throw new Exception();
    //        }

    //        // However, if the customer has not reached the limit, we will increase their consumption
    //        consumptionData.IncreaseRequests(decorator!.MaxRequests);
    //    }

    //    // Finally, let's update the customer's consumption data
    //    await _cache.SetCacheValueAsync(context.GetCustomerKey(), consumptionData);
    //}


    public static AuthenticateResult GetAnonymousAccessTicket()
    {
        var identity = new ClaimsIdentity();

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, "Anonymous");

        return AuthenticateResult.Success(ticket);
    }


    public static string GetErrorMessageForStatusCode(HttpStatusCode statusCode, Exception exception = null)
    {
        string defaultMessage = GetDefaultMessageForStatusCode(statusCode);

        return string.IsNullOrWhiteSpace(exception.Message) ? exception.Message : defaultMessage;
    }

    private static string GetDefaultMessageForStatusCode(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Resource not found",
            HttpStatusCode.MethodNotAllowed => "Method not allowed",
            HttpStatusCode.NotAcceptable => "Not Acceptable",
            HttpStatusCode.RequestTimeout => "Request Timeout",
            HttpStatusCode.Conflict => "Conflict",
            HttpStatusCode.Gone => "Gone",
            HttpStatusCode.LengthRequired => "Length Required",
            HttpStatusCode.PreconditionFailed => "Precondition Failed",
            HttpStatusCode.RequestEntityTooLarge => "Request Entity Too Large",
            HttpStatusCode.RequestUriTooLong => "Request Uri Too Long",
            HttpStatusCode.UnsupportedMediaType => "Unsupported Media Type",
            HttpStatusCode.RequestedRangeNotSatisfiable => "Requested Range Not Satisfiable",
            HttpStatusCode.ExpectationFailed => "Expectation Failed",
            HttpStatusCode.UpgradeRequired => "Upgrade Required",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            HttpStatusCode.NotImplemented => "Not Implemented",
            HttpStatusCode.BadGateway => "Bad Gateway",
            HttpStatusCode.ServiceUnavailable => "Service Unavailable",
            HttpStatusCode.GatewayTimeout => "Gateway Timeout",
            HttpStatusCode.HttpVersionNotSupported => "HTTP Version Not Supported",
            HttpStatusCode.TooManyRequests => "Too Many Requests. Please try again later.",
            _ => "An error occurred."
        };
    }
}

