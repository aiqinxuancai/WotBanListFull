using Newtonsoft.Json.Linq;
using System.Text.Json;
using WotBanListFull.Services;

namespace WotBanListFull
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            MainAsync(args).GetAwaiter().GetResult();

            

        }


        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Hello, World!");
            //var result = await WotDataCollectionManager.GetZDL("丿泰瑞尔丨");


            await CalculateCSV.Start();

            //var result2 = await WotDataCollectionManager.GetWgBattleDataBase("老紫蜀道山");


            //string accountId = (string)result2["response"][0]["account_id"];

            //var da = await WotDataCollectionManager.GetWgUserInfoCreateTime(accountId, "老紫蜀道山");
            


            ////var result3= await WotDataCollectionManager.GetWgBattleDataFull(accountId);

            ////var result4 = await WotDataCollectionManager.GetWgTanksDataFull(accountId);

            //WM8 wn8 = new WM8();
            //var wn = await wn8.Calculate(accountId);

            ////Console.WriteLine(result);
            //Console.WriteLine(wn);


        }
    }
}