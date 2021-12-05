namespace NodeWebApi.Dtos.Wallets
{
    public record WalletDto
    {
        public Guid Id { get; init; }
        public DateTimeOffset CreationDate { get; init; }
        public byte[]? PublicKey { get; set; }
    }
}

