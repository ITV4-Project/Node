using NodeRepository.Entities;

namespace NodeWebApi.Dtos.Transactions
{
    public record TransactionDto
    {
        public Guid Id { get; init; }
        public int Version { get; init; }
        public DateTimeOffset CreationTime { get; init; }
        public byte[] MerkleHash { get; init; }
        public byte[] Input { get; init; }
        public int Amount { get; init; }
        public byte[] Output { get; init;}
        public bool IsDelegating { get; init; }
        public byte[] Signature { get; init; }
    }
}



