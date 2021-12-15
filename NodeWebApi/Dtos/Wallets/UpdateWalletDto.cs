using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Wallets
{
    public record UpdateWalletDto
    {
        [Required]
        public byte[]? PublicKey { get; set; }
    }
}

