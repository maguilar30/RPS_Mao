using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Services
{
    public class ChampionshipService : IChampionshipService
    {
        private ChampionshipEntities2 db = new ChampionshipEntities2();

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

        public List<Player> Tournament(List<Player> championship)
        {
            List<Player> result = new List<Player>();
            while (championship.Count > 0)
            {
                if (championship.Count > 2)
                {
                    championship.Add(Winner(championship[0], championship[1]));
                }
                else
                {
                    Player p = Winner(championship[0], championship[1]);
                    if (p.name == championship[0].name)
                    {
                        championship[0].score = 3;
                        championship[1].score = 1;
                        result.Add(championship[0]);
                        result.Add(championship[1]);
                    }
                    else
                    {
                        championship[1].score = 3;
                        championship[0].score = 1;
                        result.Add(championship[1]);
                        result.Add(championship[0]);
                    }
                }

                championship.Remove(championship[0]);
                championship.Remove(championship[0]);
            }

            return result;
        }

        private Player Winner(Player a, Player b)
        {
            string op = a.option + b.option;
            Player winner = new Player();
            switch (op)
            {
                case "RS":
                    winner = a;
                    break;
                case "SR":
                    winner = b;
                    break;
                case "RP":
                    winner = b;
                    break;
                case "PR":
                    winner = a;
                    break;
                case "SP":
                    winner = a;
                    break;
                case "PS":
                    winner = b;
                    break;
                default:
                    break;
            }

            return winner;
        }

        public List<Player> GetPlayersList(string data)
        {
            string[] lis = data.Split('"');
            List<Player> players = new List<Player>();
            int count = 0;
            foreach (string p in lis)
            {
                if (!(p.Contains("[") || p.Contains("]") || p.Contains(",") || p.Contains("\\")))
                {
                    if (count == 0)
                    {
                        if (!p.Equals(string.Empty))
                        {
                            Player pl = new Player()
                            {
                                name = p
                            };
                            players.Add(pl);
                            count = 1;
                        }
                        else
                            throw new Exception("Input Error: Invalid Format!");

                    }
                    else if (count == 1)
                    {
                        if (p.Length == 1)
                        {
                            Player pl = players[players.Count - 1];
                            pl.option = p;
                            count = 0;
                        }
                        else
                            throw new Exception("Input Error: Invalid Format!");
                    }
                }

            }
            return players;

        }
    }
}