namespace NodeWebApi.Entities
{
    public record Transaction
    {
        public Guid Id { get; init; }
        public DateTimeOffset CreationDate { get; init; }
        public Wallet Input { get; set; }
        public int Amount { get; set; }
        public Wallet Output { get; set;}
    }
}
