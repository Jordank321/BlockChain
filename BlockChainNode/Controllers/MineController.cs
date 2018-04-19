using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlockChain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockChainNode.Controllers
{
    [Produces("application/json")]
    [Route("api/Mine")]
    public class MineController : Controller
    {
        private readonly Chain _chain;
        private readonly List<Node> _nodes;
        private BlockChainOptions _options;

        public MineController(Chain chain, List<Node> nodes, IOptions<BlockChainOptions> options)
        {
            _chain = chain;
            _nodes = nodes;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult LastBlock()
        {
            return Ok(_chain.Blocks.Last());
        }

        [HttpPost]
        public async Task<IActionResult> MineBlock([FromQuery]int proof, [FromQuery]ulong nodeId)
        {
            if (!_chain.MineBlock(proof, nodeId)) return Ok(false);
            System.IO.File.WriteAllText(_options.ChainFilePath, JsonConvert.SerializeObject(_chain));

            var last2 = _chain.Blocks.TakeLast(2).ToArray();

            var resolveRequest = new ResolveRequest
            {
                LastBlockHash = last2[0].LastBlockHash,
                LastBlockIndex = last2[0].Index,
                NewBlock = last2[1]
            };

            var httpClient = new HttpClient();
            var body = new StringContent(JsonConvert.SerializeObject(resolveRequest), Encoding.UTF8, "application/json");
            foreach (var node in _nodes)
            {
                await httpClient.PostAsync(new Uri(node.Url).AbsoluteUri + "api/resolve", body).ConfigureAwait(false);
            }
            return Ok(true);
        }
    }
}