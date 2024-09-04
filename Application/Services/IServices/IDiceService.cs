using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Services.IServices
{
    public interface IDiceService
    {
        Task<int> RollDiceSet(DiceSetTypes diceSetTypes);
    }
}
