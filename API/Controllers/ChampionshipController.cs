using API.Models;
using API.Services;
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
                string data = datas.data.Replace(" ", "");
                //var championship = StringToCompetitors(data, out errorMessage);
                try
                {
                    List<Player> championship = cs.GetPlayersList(data);

                    if ((championship.Count % 2) == 0)
                    {
                        List<Player> result = cs.Tournament(championship);
                        if (result.Count == 2)
                        {
                            foreach (Player p in result)
                            {
                                cs.UpdatePlayer(p);                                
                            }
                            response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{winner: ['" + result[0].name + "','" + result[0].option + "']}");
                        }
                        else
                        {
                            response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{Error: Invalid Format}");
                        }
                    }
                    else
                    {
                        response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{Error: Invalid Format}");
                    }
                }
                catch (Exception ex)
                {
                    response = Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, "{Error: " + ex.Message + "}");
                    return response;
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
    }
}
