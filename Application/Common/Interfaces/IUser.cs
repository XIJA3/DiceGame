using DataTransferModels.Clients;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace ApplicationTemplate.Server.Common.Interfaces
{
    public interface IUser
    {
        string HubConnectionId { get; }

        bool IsAuthenticated { get; }

        DeviceInfo? DeviceInfo { get; }

        IPAddress? IpAddress { get; }

        IApplicationClient Client { get; }

        int Id { get; }

        string UserName { get; }
    }
}

