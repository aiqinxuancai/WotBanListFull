using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotBanListFull.Services
{
    internal class WM8
    {
        /**
         * Accurate calculation (removes OP and missing tanks from account summary data).
         *
         * @var bool
         */
        private bool search_missing_tanks = false;

        /**
         * Calculate WN8 from all the tanks, even the ones missing.
         *
         * @var bool
         */
        private bool accurate_calculation = false;

        public int Calculate()
        {
            int wn8 = 0;
            if (wn8 == null)
            {
                int account_id = this.account_id;
                dynamic summary = this.api.get("wot/account/info", new { fields = "statistics.all.battles,statistics.all.frags,statistics.all.damage_dealt,statistics.all.dropped_capture_points,statistics.all.spotted,statistics.all.wins", account_id });
                summary = summary[account_id].statistics.all;
                dynamic tanks = this.api.get("wot/account/tanks", new { fields = "tank_id,statistics.battles", account_id });
                tanks = tanks[account_id];
                if (tanks.Count == 0)
                {
                    
                    return wn8;
                }
                var expectedTankValues = this.GetExpectedTankValues();
                double expDAMAGE = 0;
                double expFRAGS = 0;
                double expSPOT = 0;
                double expDEF = 0;
                double expWIN = 0;
                List<int> missing = new List<int>();
                foreach (var tank in tanks)
                {
                    if (expectedTankValues.ContainsKey(tank.tank_id))
                    {
                        var expected = expectedTankValues[tank.tank_id];
                        var tank_battles = tank.statistics.battles;
                        expDAMAGE += expected.expDamage * tank_battles;
                        expSPOT += expected.expSpot * tank_battles;
                        expFRAGS += expected.expFrag * tank_battles;
                        expDEF += expected.expDef * tank_battles;
                        expWIN += 0.01 * expected.expWinRate * tank_battles;
                    }
                    else
                    {
                        missing.Add(tank.tank_id);
                    }
                }
                if (this.accurate_calculation && missing.Count > 0)
                {
                    dynamic missing_tanks = this.api.get("wot/tanks/stats", new { tank_id = string.Join(",", missing), fields = "tank_id,all.battles,all.frags,all.damage_dealt,all.dropped_capture_points,all.spotted,all.wins", account_id });
                    missing_tanks = missing_tanks[account_id];
                    foreach (var tank in missing_tanks)
                    {
                        summary.damage_dealt -= tank.all.damage_dealt;
                        summary.spotted -= tank.all.spotted;
                        summary.frags -= tank.all.frags;
                        summary.dropped_capture_points -= tank.all.dropped_capture_points;
                        summary.wins -= tank.all.wins;
                    }
                }
                if (missing.Count > 0 && this.search_missing_tanks)
                {
                    this.missing_tanks = this.api.get("wot/encyclopedia/tankinfo", new { tank_id = string.Join(",", missing), fields = "localized_name" });
                }
                double rDAMAGE = summary.damage_dealt / expDAMAGE;
                double rSPOT = summary.spotted / expSPOT;
                double rFRAG = summary.frags / expFRAGS;
                double rDEF = summary.dropped_capture_points / expDEF;
                double rWIN = summary.wins / expWIN;
                double rWINc = Math.Max(0, (rWIN - 0.71) / (1 - 0.71));
                double rDAMAGEc = Math.Max(0, (rDAMAGE - 0.22) / (1 - 0.22));
                double rFRAGc = Math.Max(0, Math.Min(rDAMAGEc + 0.2, (rFRAG - 0.12) / (1 - 0.12)));
                // Cache WN8 value and return it
                return wn8;

            }

            // If WN8 value is already calculated, return it
            return wn8;
        }
    }
}
