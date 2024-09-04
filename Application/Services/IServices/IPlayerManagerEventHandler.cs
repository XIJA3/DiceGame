using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.Server.Services.IServices
{
    public interface IPlayerManagerEventHandler
    {
        void PlayerAdded(GamePlayer player);
        void PlayerRemoved(GamePlayer player);
        void PlayerDisconnected(GamePlayer player);
        void PlayerReconnected(GamePlayer player);
    }
}
