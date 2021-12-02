namespace NodeWebApi.Entities;

public record Transaction
{
    public Guid Id { get; init; }
    public DateTimeOffset CreationDate { get; init; }
    public List<Wallet> Inputs { get; set; }
    public int Amount { get; set; }
    public List<Wallet> Outputs { get; set;}
}
