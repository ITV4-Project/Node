using Microsoft.AspNetCore.Mvc;
using NodeWebApi.Dtos.Wallets;
using NodeRepository.Entities;
using NodeRepository.Repositories.Wallets;

namespace NodeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletsRepository repository;

        public WalletsController(IWalletsRepository repository)
        {
            this.repository = repository;
        }

        // GET /wallets
        [HttpGet]
        public IEnumerable<WalletDto> GetWallets()
        {
            var wallets = repository.GetWallets().Select(wallet => wallet.AsDto());
            return wallets;
        }

        // GET /wallets/{id}
        [HttpGet("{id}")]
        public ActionResult<WalletDto> GetWallet(Guid id)
        {
            var wallet = repository.GetWallet(id);

            if (wallet == null)
            {
                return NotFound();
            }
            return wallet.AsDto();
        }

        // POST /wallets
        [HttpPost]
        public ActionResult<WalletDto> CreateWallet(CreateWalletDto walletDto)
        {
            Wallet wallet = new()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTimeOffset.UtcNow,
                PublicKey = walletDto.PublicKey
            };

            repository.CreateWallet(wallet);

            return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id}, wallet.AsDto());
        }

        // PUT /wallets/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateWallet(Guid id, UpdateWalletDto walletDto)
        {
            var existingWallet = repository.GetWallet(id);

            if (existingWallet == null)
            {
                return NotFound();
            }

            Wallet updatedWallet = existingWallet with
            {
                PublicKey = walletDto.PublicKey
            };

            repository.UpdateWallet(updatedWallet);

            return NoContent();
        }

        // DELETE /wallets/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteWallet(Guid id)
        {
            var existingWallet = repository.GetWallet(id);

            if (existingWallet == null)
            {
                return NotFound();
            }

            repository.DeleteWallet(id);

            return NoContent();
        }
    }
}
