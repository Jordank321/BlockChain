using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockChain;

namespace BlockChainNode
{
    public class ResolveRequest
    {
        public int LastBlockIndex { get; set; }
        public string LastBlockHash { get; set; }
        public Block NewBlock { get; set; }
    }
}
