namespace NodeWebApi.Entities;

public record Wallet
{
    public Guid Id { get; init; }
    public DateTimeOffset CreationDate { get; init; }
    public byte[]? PublicKey { get; set; }
}