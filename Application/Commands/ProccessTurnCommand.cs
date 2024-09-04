namespace ApplicationTemplate.Server.Commands
{
    public record ProccessTurnCommand(IUser User, long SessionId, long SessionPlayId,DiceSetTypes DiceSetType) : IRequest { }
}

