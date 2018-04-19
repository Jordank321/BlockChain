using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlockChain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockChainNode.Controllers
{
    [Produces("application/json")]
    [Route("api/Node")]
    public class NodeController : Controller
    {
        private readonly List<Node> _nodes;
        private readonly Chain _chain;
        private readonly BlockChainOptions _options;

        public NodeController(List<Node> nodes, IOptions<BlockChainOptions> options, Chain chain)
        {
            _nodes = nodes;
            _chain = chain;
            _options = options.Value;
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            return Ok();
        }

        [HttpPost("register/{url}")]
        public IActionResult RegisterNode([FromRoute]string url)
        {
            if (_nodes.Count(n => n.Url == url) > 0) return NoContent();
            if (!new HttpClient().GetAsync(new Uri(url).AbsoluteUri + "api/Node/status").Result.IsSuccessStatusCode) return new StatusCodeResult(410);

            _nodes.Add(new Node
            {
                Url = url
            });

            System.IO.File.WriteAllText(_options.NodesFilePath, JsonConvert.SerializeObject(_nodes));

            foreach (var node in _nodes)
            {
                new HttpClient().PostAsync(new Uri(node.Url) + $"register/{url}", new StringContent(string.Empty));
            }

            return Accepted();
        }

        [HttpPost("resolve")]
        public IActionResult ResolveBlocks([FromBody] ResolveRequest resolveRequest)
        {
            _chain.MineBlock(resolveRequest.NewBlock.Proof, resolveRequest.NewBlock.Transactions.First(t => t.From == 0).To);
            return Ok();
        }
    }
}