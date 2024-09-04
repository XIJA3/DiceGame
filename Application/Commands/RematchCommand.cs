namespace ApplicationTemplate.Server.Commands
{
    public record RematchCommand(long SessionId, IUser User) : IRequest { }
}

