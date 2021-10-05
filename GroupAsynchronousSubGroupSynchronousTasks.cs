using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskParllelLibrary
{
    class GroupAsynchronousSubGroupSynchronousTasks
    {
        static Dictionary<int, List<Task<string>>> dictionaryTasks = new Dictionary<int, List<Task<string>>>();

        static void Main()
        {
            var flows = PrepareData();
            foreach (var item in flows)
            {
                if (dictionaryTasks.ContainsKey(item.FlowId))
                    dictionaryTasks[item.FlowId].Add(item.FlowTask);
                else
                {
                    var tasks = new List<Task<string>>();
                    tasks.Add(item.FlowTask);
                    dictionaryTasks.Add(item.FlowId, tasks);
                }
            }
            Parallel.ForEach(dictionaryTasks.Keys, (key) =>
            {
                Console.WriteLine($"Running Parllely - {key}");
                Task.Factory.StartNew(() =>
                {
                    foreach (var task in dictionaryTasks[key])
                    {
                        task.RunSynchronously();
                    }
                    
                });
            });
            Console.ReadLine();

        }
        static string DownloadAsync(string message)
        {
            Console.WriteLine($"{message}");
            using (HttpClient httpClient = new HttpClient())
            {
                string response = httpClient.GetStringAsync("https://www.google.com/").GetAwaiter().GetResult();
                Console.WriteLine($"{message} - Response Received");
                return response;
            }
        }

        static List<Flow> PrepareData()
        {
            List<Flow> flows = new List<Flow>();
            int flowId = 1;
            for (int i = 0; i < 10; i++)
            {
                Flow oFlow = new Flow()
                {
                    FlowId = flowId,
                    FlowTask = new Task<string>(() => DownloadAsync($"Message-{flowId}- 0"))
                };

                Flow oFlow1 = new Flow()
                {
                    FlowId = flowId,
                    FlowTask = new Task<string>(() => DownloadAsync($"Message-{flowId}-1"))
                };

                Flow oFlow2 = new Flow()
                {
                    FlowId = flowId,
                    FlowTask = new Task<string>(() => DownloadAsync($"Message-{flowId}-2"))
                };

                flows.Add(oFlow);
                flows.Add(oFlow1);
                flows.Add(oFlow2);
                flowId = flowId + 1;
            }
            return flows;
        }

        public class Flow
        {
            public int FlowId { get; set; }
            public Task<string> FlowTask { get; set; }
        }

    }
}


