using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChinaUniversity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Start();
            Console.ReadKey();
        }

        public static async void Start()
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 开始抓取......");
            var sw = new Stopwatch();
            sw.Start();
            var list = await GetUniversityList();
            long times = sw.ElapsedMilliseconds;
            Console.WriteLine($"耗时{times}毫秒，共获取到{list.Count}条数据");
            foreach (var item in list)
            {
                Console.WriteLine($"{item.Name} - {item.Area} -  {item.Belong} -  {item.Type} -  {item.Level} -  {item.Tag} -  {item.IsGraduateSchool}");
            }
        }

        public static async Task<List<University>> GetUniversityList()
        {
            int total = 137;
            List<University> UniversityList = new List<University>();
            University university;
            for (int page = 1; page < total; page++)
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 正在解析第{page}页......");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(await DownLoad(page));
                var trNodes = doc.DocumentNode.SelectNodes("//table[@class='ch-table']/tr");
                for (int trIndex = 1; trIndex < trNodes.Count; trIndex++)
                {
                    var tdNodes = trNodes[trIndex].ChildNodes.Where(ele => ele.Name == "td").ToList();
                    university = new University
                    {
                        Name = tdNodes[0].InnerText.Trim().Replace("\r", string.Empty),
                        Area = tdNodes[1].InnerText.Trim().Replace("\r", string.Empty),
                        Belong = tdNodes[2].InnerText.Trim().Replace("\r", string.Empty),
                        Type = tdNodes[3].InnerText.Trim().Replace("\r", string.Empty),
                        Level = tdNodes[4].InnerText.Trim().Replace("\r", string.Empty),
                        Tag = string.Join(",", tdNodes[5].ChildNodes.Where(ele => ele.Name == "span").Select(ele => ele.InnerText)),
                        IsGraduateSchool = !string.IsNullOrEmpty(tdNodes[6].InnerHtml)
                    };
                    UniversityList.Add(university);
                }
            }
            return UniversityList;
        }

        public static async Task<string> DownLoad(int page)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] 正在抓取第{page}页......");
            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            try
            {
                string url = $"https://gaokao.chsi.com.cn/sch/search--ss-on,searchType-1,option-qg,start-{(page - 1) * 20}.dhtml";
                string result = await wc.DownloadStringTaskAsync(new Uri(url));
                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                wc.Dispose();
            }
        }
    }
}
