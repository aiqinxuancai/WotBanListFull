using Newtonsoft.Json.Linq;
using System.Text.Json;
using WotBanListFull.Services;

namespace WotBanListFull
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }


        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("WOT封禁列表CSV细化工具");
            if (args.Length > 0) {
                await CalculateCSV.Start(args[0]);
            } 
            else
            {
                Console.WriteLine("请提供参数：封禁列表CSV文件路径");
            }
        }
    }
}