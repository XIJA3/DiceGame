using DataTransferModels.DTO;
using DeviceDetectorNET;
using Enums.Enums;
using Microsoft.AspNetCore.Http.Features;

namespace Web.Helpers
{
    public static class DeviceDetectorHelper
    {
        public static DeviceInfo Detect(string userAgent)
        {
            var dd = new DeviceDetector(userAgent);
            dd.Parse();
            var deviceType = dd.IsMobile() ? DeviceType.Mobile : dd.IsDesktop() ? DeviceType.Desktop :
                dd.IsTablet() ? DeviceType.Tablet : DeviceType.Unknown;

            return new DeviceInfo("Unknown", dd.GetDeviceName(), deviceType, userAgent);
        }

        public static DeviceInfo Detect(this IHttpConnectionFeature feature, HttpContext context)
        {
            var remoteIpAddress = feature.RemoteIpAddress?.ToString() ?? "Unknown";

            var userAgent = context.Request.Headers["User-Agent"].ToString();

            if (userAgent != null)
            {
                var result = Detect(userAgent);

                result.Ip = remoteIpAddress;

                return result;
            }

            return new DeviceInfo(remoteIpAddress, "Unknown", DeviceType.Unknown, "Unknown");
        }
    }
}
