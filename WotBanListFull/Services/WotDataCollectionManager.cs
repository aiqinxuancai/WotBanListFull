using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WotBanListFull.Services
{
    internal class WotDataCollectionManager
    {
        public static async Task<int> GetZDL(string name)
        {
            try
            {
                var url = $"http://wotbox.ouj.com/wotbox/index.php?r=default%2Findex&pn={name}";
                var body = await url.GetStringAsync();
                var regex = new Regex("<em\\sclass=\"name\">战斗力<\\/em>\\s*<span\\sclass=\"num\">(\\d+)<\\/span>");
                var match = regex.Match(body);

                if (match.Success)
                {
                    var v = match.Groups[1].ToString();
                    if (int.TryParse(v, out var result))
                    {
                        return result;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return -1 ;

        }

        

        public static async Task<(DateTime createTime, int day)> GetWgUserInfoCreateTime(string id, string name)
        {
            try
            {
                var url = $"https://wotgame.cn/zh-cn/community/accounts/{id}-{name}/?utm_source=global-nav&utm_medium=link&utm_campaign=wot-portal";
                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                     { "user-agent" ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"},
                };

                var body = await url.WithHeaders(headers).GetStringAsync();

                string pattern = @"""reg_timestamp"":\s*(\d+)";

                Match match = Regex.Match(body, pattern);

                if (match.Success)
                {
                    int timestamp = int.Parse(match.Groups[1].Value);  // 匹配到的时间戳
                    Debug.WriteLine(timestamp);
                    DateTime dateTime = DateTime.UnixEpoch.AddSeconds(timestamp);
                    return (dateTime, (int)(DateTime.Now - dateTime).TotalDays);
                }
                else
                {
                    Console.WriteLine("未找到匹配的字符串");
                }
            }
            catch (Exception ex)
            {

            }

            
            return (DateTime.UnixEpoch, 0);
        }


        public static async Task<JObject> GetWgBattleDataBase(string name)
        {
            var url = $"https://wotgame.cn/zh-cn/community/accounts/search/?name={name}&name_gt=";

            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                 { "referer" ,"https://wotgame.cn/zh-cn/community/accounts/"},
                 { "user-agent" ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"},
                 { "x-requested-with" ,"XMLHttpRequest"},
            };

            /*
                {
                  "show_more_accounts": false,
                  "name_gt": "",
                  "response": [
                    {
                      "account_wins": 50.667224026496029,
                      "account_id": 7047921515,
                      "account_battles": 31099,
                      "account_profile": "/zh-cn/community/accounts/7047921515-%E4%B8%BF%E6%B3%B0%E7%91%9E%E5%B0%94%E4%B8%A8/",
                      "clan_url": "https://wgn.wggames.cn/clans/wot/6257/?utm_campaign=wot-portal&utm_medium=link",
                      "account_name": "丿泰瑞尔丨",
                      "account_exp": 18783709,
                      "clan_tag": "新和联胜"
                    }
                  ]
                }
             */


            try
            {
                var body = await url.WithHeaders(headers).GetStringAsync();
                Debug.WriteLine(body);
                return JObject.Parse(body);
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static async Task<JObject> GetWgBattleDataFull(string id)
        {
            var url = $"https://wotgame.cn/wotup/profile/statistics/?spa_id={id}&battle_type=random";
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                 { "referer" ,"https://wotgame.cn/zh-cn/community/accounts/"},
                 { "user-agent" ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"},
                 { "x-requested-with" ,"XMLHttpRequest"},
            };


            try
            {
                var body = await url.WithHeaders(headers).GetStringAsync();
                Debug.WriteLine(body);
                return JObject.Parse(body);
            }
            catch (Exception ex)
            {

            }
            return null;

        }

        public static async Task<JObject> GetWgTanksDataFull(string id)
        {
            var url = $"https://wotgame.cn/wotup/profile/vehicles/list/";

            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                 { "referer" ,"https://wotgame.cn/zh-cn/community/accounts/"},
                 { "user-agent" ,"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36"},
                 { "x-requested-with" ,"XMLHttpRequest"},
            };

            var postData = $"{{\"battle_type\":\"random\",\"only_in_garage\":false,\"spa_id\":{id},\"premium\":[0,1],\"collector_vehicle\":[0,1],\"nation\":[],\"role\":[],\"type\":[],\"tier\":[],\"language\":\"zh-cn\"}}";

            /*
                data.data[]
                data."parameters": [
                      "vehicle_cd",
                      "tech_name",
                      "premium",
                      "collector_vehicle",
                      "nation",
                      "type",
                      "role",
                      "name",
                      "tier",
                      "default_image",
                      "default_icon",
                      "disabled",
                      "tank_nations_list",
                      "wins_ratio",
                      "xp_per_battle_average",
                      "wins_count",
                      "damage_dealt",
                      "damage_received",
                      "hits_count",
                      "xp_amount",
                      "frags_count",
                      "damage_dealt_received_ratio",
                      "marksOnGun",
                      "damage_per_battle_average",
                      "frags_per_battle_average",
                      "frags_deaths_ratio",
                      "markOfMastery",
                      "battles_count",
                      "survived_battles"
                    ]

             */


            try
            {
                var rep = await url.WithHeaders(headers).PostStringAsync(postData);
                var body = await rep.GetStringAsync();
                //Console.WriteLine(body);
                return JObject.Parse(body);
            }
            catch (Exception ex)
            {

            }
            return null;


        }





    }
}
