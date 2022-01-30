using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetworking
{
    public record Node
    {
        public Node(long votes = 0)
        {
            Id = Guid.NewGuid();
            Votes = votes;
            Key = new ECDsaKey();
        }

        public Guid Id { get; init; }
        public long Votes { get; set; }
        public ECDsaKey Key { get; init; }


        public void SignBlock(Block block)
        {
            block.Sign(Key);
        }

        public bool VerifyBlock(Block block)
        {
            return block.VerifySignature();
        }
    }
}
