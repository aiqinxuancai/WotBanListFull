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
            var result = await WotDataCollectionManager.GetZDL("丿泰瑞尔丨");
            var result2 = await WotDataCollectionManager.GetWgBattleDataBase("丿泰瑞尔丨");


            string accountId = (string)result2["response"][0]["account_id"];

            var result3= await WotDataCollectionManager.GetWgBattleDataFull(accountId);

            var result4 = await WotDataCollectionManager.GetWgTanksDataFull(accountId);
            


            Console.WriteLine(result);




        }
    }
}