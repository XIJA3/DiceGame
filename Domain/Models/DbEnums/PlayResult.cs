using Domain.Contracts;

namespace Domain.Models.DbEnums
{
    // Db Enum Example
    public class PlayResult(long id, string name) : DbEnum(id, name)
    {
        public static readonly PlayResult Win = new(1, nameof(Win));
        public static readonly PlayResult Lose = new(2, nameof(Lose));
        public static readonly PlayResult Draw = new(3, nameof(Draw));
    }
}
