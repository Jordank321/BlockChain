using System;
using System.IO;
using System.Net.Http;
using BlockChain;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Miner
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddUserSecrets("Miner")
                    .Build();

                var nodeId = config["NodeId"];
                var mineUrl = new Uri(args[0]);

                var httpClient = new HttpClient();

                var lastBlock = JsonConvert.DeserializeObject<Block>(httpClient
                    .GetAsync(mineUrl.AbsoluteUri + "api/mine").Result
                    .Content.ReadAsStringAsync().Result);

                var random = new Random();

                while (true)
                {
                    var attempt = random.Next(int.MaxValue);
                    if (!Chain.ValidateProof(lastBlock, attempt)) continue;

                    var response = httpClient.PostAsync(mineUrl.AbsoluteUri + $"api/mine?proof={attempt}&nodeId={nodeId}", new StringContent(string.Empty)).Result;

                    if (!JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result)) continue;

                    Console.WriteLine("Success!");

                    break;
                }
            }
        }
    }
}
