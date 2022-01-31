using NodeRepository.Entities;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Transactions
{
    public record UpdateTransactionDto
    {
        [Required]
        public int Version { get; init; }
        public string? Name { get; init; }
        [Required]
        public byte[] MerkleHash { get; init; }
        [Required]
        public byte[] Input { get; init; }
        [Required]
        public long Amount { get; init; }
        [Required]
        public byte[] Output { get; init;}
        [Required]
        public bool Delegate { get; init; }
        [Required]
        public byte[] Signature { get; init; }
    }
}

