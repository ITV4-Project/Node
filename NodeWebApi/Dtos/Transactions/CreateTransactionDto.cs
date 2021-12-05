using NodeWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace NodeWebApi.Dtos.Transactions
{
    public class CreateTransactionDto
    {
        [Required]
        public Wallet Input { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public Wallet Output { get; set; }
    }
}
