using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace WotBanListFull.Services
{
    public class BanPlayerRead
    {
        //["封禁昵称","封禁原因","封禁时长","封禁日期",""]
        [Index(0)]
        [Name("封禁昵称")]
        public string Name { get; set; }

        [Index(1)]
        [Name("封禁原因")]
        public string Reason { get; set; }

        [Index(2)]
        [Name("封禁时长")]
        public string Time { get; set; }

        [Index(3)]
        [Name("封禁日期")]
        public DateTime BanTime { get; set; }
    }

    public class BanPlayerWrite
    {
        //["封禁昵称","封禁原因","封禁时长","封禁日期",""]
        [Index(0)]
        [Name("封禁昵称")]
        public string Name { get; set; }

        [Index(1)]
        [Name("封禁原因")]
        public string Reason { get; set; }

        [Index(2)]
        [Name("封禁时长")]
        public string Time { get; set; }

        [Index(3)]
        [Name("封禁日期")]
        public DateTime BanTime { get; set; }


        [Name("建号时间")]
        public string CreateTime { get; set; }


        [Name("账户天数")]
        public string CreateCountDay { get; set; }


        [Name("战斗场次")]
        public string BattleCount { get; set; }


        [Name("胜率")]
        public string BattleRate { get; set; }


        [Name("军团名称")]
        public string LegionName { get; set; }


        [Name("WN8")]
        public string WN8 { get; set; }
    }

    internal class CalculateCSV
    {

        public static void Start()
        {
            Console.WriteLine("Hello World!");



            int fullPrice = 0;

            int fullCount = 0;
            Dictionary<string, int> dict = new Dictionary<string, int>();

            using (var reader = new StreamReader("封禁名单-2023-03-29.csv"))
            {
                List<string> badRecord = new List<string>();
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    Mode = CsvMode.NoEscape,
                    HasHeaderRecord = true,
                    BadDataFound = context => badRecord.Add(context.RawRecord)
                };

                using (var csv = new CsvReader(reader, config))
                {
                   
                    var records = csv.GetRecords<BanPlayerRead>(); //一次性读取所有数据，并且转换为对象集合。
                    var recordList = records.ToList();
                    var writeList = new List<BanPlayerWrite>();
                    foreach (var record in recordList)
                    {
                        fullCount++;
                        //计算WN8，填充军团信息

                        BanPlayerWrite banPlayerWrite = new BanPlayerWrite()
                        {
                            BanTime = record.BanTime,
                            Time = record.Time,
                            Reason = record.Reason,
                            Name = record.Name,
                        };

                        writeList.Add(banPlayerWrite);

                    }

                    //var recordList = records.ToList();




                    SaveToCsv("BanList.csv", writeList);

                }

                
            }

            var newDict = dict.OrderBy(a => a.Key);
            foreach (KeyValuePair<string, int> line in newDict)
            {
                Console.WriteLine($"{line.Key}   {line.Value}");
            }

            Console.WriteLine($"{fullCount} {fullPrice}");


        }

        public static void SaveToCsv(string filePath, List<BanPlayerWrite> dataList)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                Mode = CsvMode.RFC4180,
                HasHeaderRecord = true,
                //BadDataFound = context => badRecord.Add(context.RawRecord),

                Encoding = Encoding.UTF8,

            };
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                using (var csv = new CsvWriter(writer, configuration: config))
                {
                    csv.WriteRecords(dataList);
                }
            }

            //File.ReadAllBytes(filePath);
        }

    }


}
