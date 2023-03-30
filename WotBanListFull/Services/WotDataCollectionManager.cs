using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            return -1 ;

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

            var body = await url.WithHeaders(headers).GetStringAsync();
            Console.WriteLine(body);
            return JObject.Parse(body);
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

            var body = await url.WithHeaders(headers).GetStringAsync();
            Console.WriteLine(body);
            return JObject.Parse(body);

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

            var rep = await url.WithHeaders(headers).PostStringAsync(postData);
            var body = await rep.GetStringAsync();
            Console.WriteLine(body);
            return JObject.Parse(body);

        }

    }
}
