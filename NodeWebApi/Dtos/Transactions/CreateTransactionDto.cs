using NodeWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Transactions
{
    public class CreateTransactionDto
    {
        [Required]
        public int Version { get; init; }
        public string? Name { get; init; }
        [Required]
        public byte[] MerkleHash { get; init; }
        [Required]
        public byte[] Input { get; init; }
        [Required]
        public int Amount { get; init; }
        [Required]
        public byte[] Output { get; init;}
        [Required]
        public bool Delegate { get; init; }
        [Required]
        public byte[] Signature { get; init; }
    }
}

