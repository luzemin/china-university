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
        private static readonly int pages = 138;
        private static readonly string url = "https://gaokao.chsi.com.cn/sch/search--ss-on,searchType-1,option-qg,start-{0}.dhtml";
        private static List<University> result = new List<University>();

        //日志
        private static void Log(string msg)
        {
            Console.WriteLine($"[线程 {Thread.CurrentThread.ManagedThreadId}][时间 {DateTime.Now.ToString("HH:mm:ss:ffffff")}] {msg}");
        }

        //下载
        private static async Task DownloadAsync(int page)
        {
            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Log($"第{page}页HTML下载开始......");
                var htmlStr = await wc.DownloadStringTaskAsync(new Uri(string.Format(url, (page - 1) * 20)));
                Log($"第{page}页HTML下载完成，耗时：" + sw.ElapsedMilliseconds);
                result.AddRange(Parse(htmlStr));
            }
            finally
            {
                wc.Dispose();
            }
        }

        //解析
        private static List<University> Parse(string htmlStr)
        {
            List<University> UniversityList = new List<University>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlStr);
            var trNodes = doc.DocumentNode.SelectNodes("//table[@class='ch-table']/tr");
            University university;
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
            return UniversityList;
        }

        public static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<Task> taskList = new List<Task>();
            for (int i = 1; i <= pages; i++)
            {
                taskList.Add(DownloadAsync(i));
            }
            Task.WhenAll(taskList).Wait();

            long times = sw.ElapsedMilliseconds;
            Console.WriteLine($"耗时{times}毫秒,共{result.Count}条数据");
            Console.ReadKey();
        }
    }
}
