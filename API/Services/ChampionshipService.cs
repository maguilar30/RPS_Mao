using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Services
{
    public class ChampionshipService : IChampionshipService
    {
        private ChampionshipEntities1 db = new ChampionshipEntities1();

        public Player GetPlayerByName(string name)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePlayer(Player player)
        {
            try
            {
                Player first = db.Players.Where(p => p.name == player.name).FirstOrDefault();
                if (first != null)
                {
                    first.score += player.score;
                    first.option = player.option != null ? player.option : first.option;
                }
                else
                    db.Players.Add(player);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public List<Player> GetPlayers()
        {
            return db.Players.ToList();
        }
    }
}