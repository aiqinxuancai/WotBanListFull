using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WotBanListFull.Services
{
    internal class WM8
    {
        /**
         * Accurate calculation (removes OP and missing tanks from account summary data).
         */
        private bool search_missing_tanks = false;

        /**
         * Calculate WN8 from all the tanks, even the ones missing.
         */
        private bool accurate_calculation = false;


        private Dictionary<int, JToken> expected_tank_values = new Dictionary<int, JToken>();
     


        public async Task<double> Calculate(string account_id)
        {
            double wn8 = 0;
            if (wn8 == 0)
            {
                //int account_id = this.account_id;

                //dynamic summary = "";
                //this.api.get("wot/account/info", 
                //new { fields = 
                //"statistics.all.battles," +
                //"statistics.all.frags," +
                //"statistics.all.damage_dealt," +
                //"statistics.all.dropped_capture_points," +
                //"statistics.all.spotted," +
                //"statistics.all.wins", 
                //    account_id });
                //summary = summary[account_id].statistics.all;

                var summary = await WotDataCollectionManager.GetWgBattleDataFull(account_id);
                summary = (JObject)summary["data"];

                //this.api.get("wot/account/tanks", new { fields = "tank_id,statistics.battles", account_id });
                var tanks = await WotDataCollectionManager.GetWgTanksDataFull(account_id);
                //ranks = tanks[account_id];

                if (tanks["data"]["data"].Count() == 0)
                {
                    return wn8;
                }
                var expectedTankValues = this.GetExpectedTankValues();
                double expDAMAGE = 0;
                double expFRAGS = 0;
                double expSPOT = 0;
                double expDEF = 0;
                double expWIN = 0;

                //不存在的坦克
                List<int> missing = new List<int>();
                foreach (var tank in tanks["data"]["data"])
                {
                    int tank_id = (int)tank[0];
                    if (expectedTankValues.ContainsKey(tank_id))
                    {
                        /*
                             {
      "IDNum": 1,
      "expDef": 1.135,
      "expFrag": 0.98,
      "expSpot": 1.379,
      "expDamage": 486.672,
      "expWinRate": 54.915
    },
                         */
                        var expected = expectedTankValues[tank_id];
                        var tank_battles = (double)tank[27];
                        expDAMAGE += (double)expected["expDamage"] * tank_battles;
                        expSPOT += (double)expected["expSpot"] * tank_battles;
                        expFRAGS += (double)expected["expFrag"] * tank_battles;
                        expDEF += (double)expected["expDef"] * tank_battles;
                        expWIN += 0.01 * (double)expected["expWinRate"] * tank_battles;
                    }
                    else
                    {
                        missing.Add(tank_id);
                    }
                }
                //if (this.accurate_calculation && missing.Count > 0)
                //{
                //    dynamic missing_tanks = this.api.get("wot/tanks/stats", new { tank_id = string.Join(",", missing), fields = "tank_id,all.battles,all.frags,all.damage_dealt,all.dropped_capture_points,all.spotted,all.wins", account_id });
                //    missing_tanks = missing_tanks[account_id];
                //    foreach (var tank in missing_tanks)
                //    {
                //        summary.damage_dealt -= tank.all.damage_dealt;
                //        summary.spotted -= tank.all.spotted;
                //        summary.frags -= tank.all.frags;
                //        summary.dropped_capture_points -= tank.all.dropped_capture_points;
                //        summary.wins -= tank.all.wins;
                //    }
                //}
                //if (missing.Count > 0 && this.search_missing_tanks)
                //{
                //    this.missing_tanks = this.api.get("wot/encyclopedia/tankinfo", new { tank_id = string.Join(",", missing), fields = "localized_name" });
                //}
                double rDAMAGE = (double)summary["damage_dealt_avg"] * (double)summary["battles_count"] / expDAMAGE;
                double rSPOT = (double)summary["spotted_count_avg"] * (double)summary["battles_count"] / expSPOT;
                double rFRAG = (double)summary["frags_count_avg"] * (double)summary["battles_count"] / expFRAGS;
                double rDEF = (double)summary["dropped_capture_points"] / expDEF;

                double rWIN = (double)summary["wins_count"] / expWIN;
                double rWINc = Math.Max(0f, (rWIN - 0.71) / (1 - 0.71));
                double rDAMAGEc = Math.Max(0f, (rDAMAGE - 0.22) / (1 - 0.22));
                double rFRAGc = Math.Max(0f, Math.Min(rDAMAGEc + 0.2, (rFRAG - 0.12) / (1 - 0.12)));

                double rSPOTc = Math.Max(0f, Math.Min(rDAMAGEc + 0.1, (rSPOT - 0.38) / (1 - 0.38)));
                double rDEFc = Math.Max(0f, Math.Min(rDAMAGEc + 0.1, (rDEF - 0.10) / (1 - 0.10)));

                // Cache WN8 value and return it

                wn8 = 980 * rDAMAGEc + 
                    210 * rDAMAGEc * rFRAGc + 
                    155 * rFRAGc * rSPOTc + 
                    75 * rDEFc * rFRAGc + 
                    145 * Math.Min(1.8, rWINc);

                // Ok we have WN8, store it
                wn8 = Math.Round(wn8, 2);

                return wn8;

            }

            // If WN8 value is already calculated, return it
            return wn8;
        }

        private Dictionary<int, JToken> GetExpectedTankValues()
        {
            if (expected_tank_values.Count == 0)
            {
                LoadExpectedTankValues();
            }

            return expected_tank_values;
        }


        protected void LoadExpectedTankValues()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "expected_tank_values.json");

            if (!File.Exists(path))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://static.modxvm.com/wn8-data-exp/json/wn8exp.json", path);
                }
            }

            /*
                {
                  "IDNum": 1,
                  "expDef": 1.135,
                  "expFrag": 0.98,
                  "expSpot": 1.379,
                  "expDamage": 486.672,
                  "expWinRate": 54.915
                },
             
             */

            // Load expected tank values
            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(path));
            foreach (JToken tank in json.data)
            {
                // Load tanks values and index them by Tank ID
                expected_tank_values[(int)tank["IDNum"]] = tank;
            }
        }
    }
}
