namespace ApplicationTemplate.Server.Commands
{
    public record CancelRematchCommand(long SessionId, IUser User) : IRequest { }
}

