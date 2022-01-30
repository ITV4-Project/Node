using Core;
using Core.Database;
namespace NodeRepository.Entities
{
    public record Node
    {
        public Node(long votes = 0)
        {
            Id = Guid.NewGuid();
            Votes = votes;
            Key = new ECDsaKey();
            Ledger = new();
            CurrentBlock = InitBlock();
        }

        public Guid Id { get; init; }
        public long Votes { get; set; }
        public ECDsaKey Key { get; init; }
        public Ledger Ledger { get; init; }
        public Block CurrentBlock { get; set; }

        public Block InitBlock()
        {
            //Block block = new(
            //    merkleHash: 
            //    );
            throw new NotImplementedException();
        }

        public void SignBlock()
        {
            CurrentBlock.Sign(Key);
        }

        public bool VerifyBlock(Block block)
        {
            return block.VerifySignature();
        }
    }
}
