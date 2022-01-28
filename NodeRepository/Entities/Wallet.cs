namespace NodeRepository.Entities;

public record Wallet
{
    public Guid Id { get; init; }
    public DateTimeOffset CreationTime { get; init; }
    public byte[]? PublicKey { get; set; }
}