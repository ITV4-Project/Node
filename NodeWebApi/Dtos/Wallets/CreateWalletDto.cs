using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Wallets;

public record CreateWalletDto
{
    [Required]
    public byte[]? PublicKey { get; set; }
}
