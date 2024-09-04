using Enums.Enums;

namespace DataTransferModels.DTO
{
    public class DeviceInfo(string ip, string deviceName, DeviceType type, string userAgent)
    {
        public string Ip { get; set; } = ip;
        public string DeviceName { get; set; } = deviceName;
        public DeviceType Type { get; set; } = type;
        public string UserAgent { get; set; } = userAgent;
    }
}