using System;
using System.Collections.Generic;

namespace BlockChain
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public Transaction[] Transactions { get; set; }
        public int Proof { get; set; }
        public string LastBlockHash { get; set; }
    }
}
