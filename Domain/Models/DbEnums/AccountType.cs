using Domain.Contracts;

namespace Domain.Models.DbEnums
{
    // Db Enum Example
    public class AccountType(long id, string name) : DbEnum(id, name)
    {
        public static readonly AccountType Bet = new(1, nameof(Bet));
        public static readonly AccountType Win = new(2, nameof(Win));
        public static readonly AccountType GiveFreeSpin = new(3, nameof(GiveFreeSpin));
    }
}
