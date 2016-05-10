using API.Models;
using API.Services;
using API.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class ChampionshipController : ApiController
    {
        private IChampionshipService cs = new ChampionshipService();

        public ChampionshipController()
        {
            rules = new Rules();
        }

        public HttpResponseMessage Post(string method, ResultModel datas)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            if (method.Equals("result"))
            {
                if (datas.first != null && datas.second != null)
                {
                    Player fp = new Player()
                    {
                        name = datas.first,
                        score = 3,
                        option = "S"
                    };

                    cs.UpdatePlayer(fp);

                    Player sp = new Player()
                    {
                        name = datas.second,
                        score = 1,
                        option = "P"
                    };

                    cs.UpdatePlayer(sp);

                    response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{status: 'success'}");
                }
                else
                {
                    response = Request.CreateResponse<string>(System.Net.HttpStatusCode.BadRequest, "Both Users must have values!");
                }
            }
            else if (method.Equals("new"))
            {
                string errorMessage;
                string data = datas.data.Replace(" ", "");
                var championship = StringToCompetitors(data, out errorMessage);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    Data[] finalists = Solve(championship);
                    Player first = new Player()
                    {
                        name = finalists[0].Name,
                        option = finalists[0].Strategy.ToString(),
                        score = 3
                    };

                    cs.UpdatePlayer(first);

                    Player second = new Player()
                    {
                        name = finalists[1].Name,
                        option = finalists[1].Strategy.ToString(),
                        score = 1
                    };
                    cs.UpdatePlayer(second);
                    response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{winner: " + finalists[0].Name + "," + finalists[0].Strategy.ToString() + "}");
                    //return Ok(new { winner = new string[] { finalists[0].Name, finalists[0].Strategy.ToString() } });
                }
                else
                {
                    response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{Error: " + errorMessage + "}");
                    //return Json(new { status = "Error", message = errorMessage });
                }
               
            }
            else
            {
                response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{Error: Method Not Found}");
            }
            return response;
        }

        public IHttpActionResult Get(string method)
        {
            if (method.Equals("top"))
            {

                return Ok(cs.GetPlayers().OrderByDescending(p => p.score).Take(10));
               
            }
            else
            {
                return NotFound();
            }

        }

        public IHttpActionResult Get(string method,int count)
        {
            if (method.Equals("top"))
            {
                if (count>0)
                    return Ok(cs.GetPlayers().OrderByDescending(p => p.score).Take(count));
                else
                    return Ok(cs.GetPlayers().OrderByDescending(p => p.score).Take(10));

            }
            else
            {
                return NotFound();
            }

        }

        #region Private

        private List<Data> StringToCompetitors(string input, out string errorMessage)
        {
            try
            {
                List<Data> result = new List<Data>();
                int bracketsCount = 0, playerCount = 0, index = 0;
                string name = string.Empty;
                errorMessage = string.Empty;
                StrategyEnum strategy = StrategyEnum.Invalid;
                if (!string.IsNullOrEmpty(input))
                {
                    while (index < input.Length)
                    {
                        if (input[index] == '[')
                            bracketsCount++;
                        else if (input[index] == ']')
                            bracketsCount--;
                        index++;
                        if (bracketsCount > 1)
                        {
                            if (input[index] == '\"' && playerCount == 0)
                            {
                                index++; //Ignores the opening " symbol
                                while (input[index] != '\"') //Name field
                                    name = string.Concat(name, input[index++]);
                                if (!string.IsNullOrEmpty(name)) //Valid name field
                                    playerCount++;
                                else
                                {
                                    errorMessage = "Player name field error";//Throw exception, incomplete player field
                                    return result;
                                }
                                index++; //Ignores the closing " symbol
                            }
                            if (input[index] == '\"' && playerCount == 1)
                            {
                                //Strategy value
                                index++;//Ignores the opening " symbol
                                strategy = ParseStrategy(input[index++]);
                                if (strategy == StrategyEnum.Invalid)
                                {
                                    errorMessage = string.Format("Invalid strategy {0}", input[index - 1]);//Throw exception, invalid strategy field
                                    return result;
                                }
                                else
                                {
                                    result.Add(new Data { Name = name, Strategy = strategy });
                                    strategy = StrategyEnum.Invalid;
                                    name = string.Empty;
                                    playerCount = 0;
                                }
                            }
                        }
                    }
                    if (bracketsCount > 0)
                        errorMessage = "Brackets mismatch";//Throw exception, brackets mismatch  
                }
                if (!IsPowerOfTwo(result.Count))
                    errorMessage = "The amount of competitors is not valid, must be 2^n";
                return result;
            }
            catch
            {
                throw;
            }
        }

        private StrategyEnum ParseStrategy(char input)
        {
            switch (input)
            {
                case 'R':
                    return StrategyEnum.R;
                case 'P':
                    return StrategyEnum.P;
                case 'S':
                    return StrategyEnum.S;
                default:
                    return StrategyEnum.Invalid;
            }
        }

        protected bool IsPowerOfTwo(int number)
        {
            try
            {
                double remainder = number;
                while (remainder > 1)
                {
                    if (remainder % 2 != 0)
                        return false;
                    remainder /= 2;
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        protected Rules rules;

        private Data[] Solve(List<Data> championship)
        {
            Data[] result = new Data[2]; //this will contain the first and second place
            try
            {
                if (championship != null)
                {
                    int playerA_index = 0, playerB_index = 1;
                    while (championship.Count > 1) //Always the last player is the winner
                    {
                        if (playerB_index >= championship.Count)
                        {
                            //The last playerB was the last in the list
                            //The indexes now point the first elementsin the list and runs the second round
                            playerA_index = 0;
                            playerB_index = 1;
                        }
                        Data playerA = championship[playerA_index++],
                                       playerB = championship[playerB_index++];
                        bool playerAWins = rules.GetRuleResult(playerA.Strategy, playerB.Strategy);
                        if (playerAWins) //player A wins
                            championship.Remove(playerB);
                        else //player B wins
                            championship.Remove(playerA);
                        if (championship.Count == 1) //The championship has ended up
                            SetWinners(result, playerAWins, playerA, playerB);
                    }
                }
            }
            catch
            {
                throw;
            }
            return result;
        }


        private void SetWinners(Data[] result, bool playerAWins, Data playerA, Data playerB)
        {
            try
            {
                if (playerAWins)
                {
                    result[0] = playerA;
                    result[1] = playerB;
                }
                else
                {
                    result[0] = playerB;
                    result[1] = playerA;
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}
