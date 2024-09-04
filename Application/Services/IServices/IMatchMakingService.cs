namespace ApplicationTemplate.Server.Services.IServices
{
    public interface IMatchMakingService
    {
        Task JoinRoom(IUser user);
        Task Rematch(IUser user, long sessionId);
        Task CancelRematch(IUser user, long sessionId);
        Task LeaveRoom(IUser user, long sessionId);
        Task RemovePlayer(IUser user);
        Task Ready(IUser user);
        Task NotReady(IUser user);
    }
}
