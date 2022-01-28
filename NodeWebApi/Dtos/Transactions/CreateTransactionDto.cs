using NodeRepository.Entities;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Transactions
{
    public record CreateTransactionDto
    {
        [Required]
        public int Version { get; init; }
        [Required]
        public DateTimeOffset CreationTime { get; init; }
        [Required]
        public byte[] MerkleHash { get; init; }
        [Required]
        public byte[] Input { get; init; }
        [Required]
        public long Amount { get; init; }
        [Required]
        public byte[] Output { get; init;}
        [Required]
        public bool IsDelegating { get; init; }
        [Required]
        public byte[] Signature { get; init; }
    }
}

