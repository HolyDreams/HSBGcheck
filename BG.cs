using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace newHSBGcheck
{
    internal class BG
    {
        string sqlQuery;
        /// <summary>
        /// Server - на каком сервере происходит проверка и его запись. 0 - EU, 1 - US;
        /// </summary>
        /// <param name="server"></param>
        internal void Check(int server)
        {
            if (server > 1 || server < 0)
                return;
            string srv = server == 0 ? "EU" : server == 1 ? "US" : "";
            try
            {
                int page = 1;
                Logs.Log(srv + " начал проверку.");
                List<BGstruct> bgRankList = new List<BGstruct>();
                while (bgRankList.Count < 250)
                {
                    int seasonID = 9;
                    var json = new WebClient().DownloadString($"https://hearthstone.blizzard.com/en-us/api/community/leaderboardsData?region={srv.ToUpper()}&leaderboardId=battlegrounds&seasonId=9&page={page}");
                    var Rows = JObject.Parse(json)["leaderboard"]["rows"];
                    foreach (var item in Rows)
                    {
                        var text = item.ToString();
                        var result = JsonSerializer.Deserialize<BGstruct>(text);

                        if (result.rank > 250)
                        {
                            page = 0;
                            bgRankList.Clear();
                            break;
                        }
                        if (bgRankList.Any(a => a.accountid == result.accountid))
                            result.accountid = result.accountid + " (1)";

                        bgRankList.Add(result);
                    }
                    page++;
                }
                sqlQuery = $@"SELECT *
                              FROM hsbg_{srv.ToLower()}";
                var res = SQLRequest.PostgreSQL(sqlQuery);
                bool haveRank = false;
                if (res.Rows.Count != 200)
                {
                    if (res.Rows.Count > 0)
                    {
                        sqlQuery = $@"DELETE FROM hsbg_{srv}";
                        SQLRequest.PostgreSQL(sqlQuery);
                    }
                }
                else
                    haveRank = true;
                foreach (var item in bgRankList.Where(a => a.rank <= 200).ToList())
                {
                    sqlQuery = $@"{(haveRank ? $@"UPDATE hsbg_{srv.ToLower()}
                                  SET name = '{item.accountid}',
                                      rating = {item.rating}
                                  WHERE rank = {item.rank};" : $@"INSERT INTO hsbg_{srv} (name, rating, rank)
                                  VALUES ('{item.accountid}',
                                          {item.rating},
                                          {item.rank});")}

                                  INSERT INTO hsbg_{srv}_changes (name, rating, date)
                                  VALUES ('{item.accountid}',
                                          {item.rating},
                                          NOW()::timestamp);";
                    SQLRequest.PostgreSQL(sqlQuery);
                }
                foreach (var item in bgRankList.Where(a => a.rank > 200).ToList())
                {
                    sqlQuery = $@"INSERT INTO hsbg_{srv}_changes (name, rating, date)
                                  VALUES ('{item.accountid}',
                                          {item.rating},
                                          NOW()::timestamp);";
                    SQLRequest.PostgreSQL(sqlQuery);
                }
                Logs.Log(srv + " закончил проверку");
            }
            catch (Exception ex)
            {
                Logs.Log(ex.Message);
                return;
            }
        }
    }
}
