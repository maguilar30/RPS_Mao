using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IChampionshipService
    {

        Player GetPlayerByName(string name);

        bool UpdatePlayer(Player player);

        List<Player> GetPlayers();

    }
}
