using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Transactions
{
    public record CreateTransactionDto : TransactionDto
    {
        [Required]
        public new int Version { get; init; }
        [Required]
        public new DateTimeOffset CreationTime { get; init; }
        [Required]
        public new byte[]? MerkleHash { get; init; }
        [Required]
        public new byte[]? Input { get; init; }
        [Required]
        public new long Amount { get; init; }
        [Required]
        public new byte[]? Output { get; init;}
        [Required]
        public new bool IsDelegating { get; init; }
        [Required]
        public new byte[]? Signature { get; init; }
    }
}

