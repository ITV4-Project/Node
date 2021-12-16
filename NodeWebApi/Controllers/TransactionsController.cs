﻿using Microsoft.AspNetCore.Mvc;
using NodeWebApi.Dtos.Transactions;
using NodeWebApi.Entities;
using NodeWebApi.Repositories;
using NodeWebApi.Repositories.Transactions;
using NodeWebApi.lib;

namespace NodeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepository repository;

        public TransactionsController(ITransactionsRepository repository)
        {
            this.repository = repository;
        }

        // GET /transactions
        [HttpGet]
        public IEnumerable<TransactionDto> GetTransactions()
        {
            var transaction = repository.GetTransactions().Select(transaction => transaction.AsDto());
            return transaction;
        }

        // GET /transactions/{id}
        [HttpGet("{id}")]
        public ActionResult<TransactionDto> GetTransaction(Guid id)
        {
            var transaction = repository.GetTransaction(id);

            if (transaction == null)
            {
                return NotFound();
            }
            return transaction.AsDto();
        }

        // POST /transactions
        [HttpPost]
        public ActionResult<TransactionDto> CreateTransaction(CreateTransactionDto transactionDto)
        {
            Transaction transaction = new()
            {
                Id = Guid.NewGuid(),
                Version = transactionDto.Version,
                CreationDate = DateTimeOffset.UtcNow,
                Name = transactionDto.Name,
                MerkleHash = transactionDto.MerkleHash,
                Input = transactionDto.Input,
                Amount = transactionDto.Amount,
                Output = transactionDto.Output,
                Delegate = transactionDto.Delegate,
                Signature = transactionDto.Signature
            };


            // Opmaak van data benodigd voor het signen
            // transaction amount en de transaction output worden in 1 byte array gezet; transaction output is de public key van de ontvanger
            byte[] data = repository.SignatureDataConvertToBytes(transaction);

            //// Sign data
            //// transaction input is de public key van de verzender
            //using (ECDsaCng dsa = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDsaP256)))
            //{
            //    dsa.HashAlgorithm = CngAlgorithm.Sha256;
            //    transaction.Input = dsa.Key.Export(CngKeyBlobFormat.EccPublicBlob);
            //    transaction.Signature = dsa.SignData(data);
            //}

            // Controleren of signature overeenkomt
            // Public key van verzender wordt gebruikt om te controleren of de data en de signature te verifieren

           
            //using (ECDsaCng ecsdKey = new ECDsaCng(CngKey.Import(transaction.Input, CngKeyBlobFormat.EccPublicBlob)))
            //{
            //    Console.Write(this.GetType().Name);
            //    if (ecsdKey.VerifyData(data, transaction.Signature))
            //        Console.WriteLine(" data is good");
            //    else
            //        Console.WriteLine(" data is bad");
            //}

            ECDsaKey ecdsKey = new ECDsaKey(transaction.Input, false);
            Console.Write(this.GetType().Name);
            if (ecdsKey.Verify(data, transaction.Signature))
            {
                Console.WriteLine(" data is good");
                repository.CreateTransaction(transaction);
            }
            else
            {
                Console.WriteLine(" data is bad");
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction.AsDto());
        }

        // PUT /transactions/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateTransaction(Guid id, UpdateTransactionDto transactionDto)
        {
            var existingTransaction = repository.GetTransaction(id);

            if (existingTransaction == null)
            {
                return NotFound();
            }

            Transaction updatedTransaction = existingTransaction with
            {
                Version = transactionDto.Version,
                Name = transactionDto.Name,
                MerkleHash = transactionDto.MerkleHash,
                Input = transactionDto.Input,
                Amount = transactionDto.Amount,
                Output = transactionDto.Output,
                Delegate = transactionDto.Delegate,
                Signature = transactionDto.Signature
            };

            repository.UpdateTransaction(updatedTransaction);

            return NoContent();
        }

        // DELETE /transactions/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteTransaction(Guid id)
        {
            var existingTransaction = repository.GetTransaction(id);

            if (existingTransaction == null)
            {
                return NotFound();
            }

            repository.DeleteTransaction(id);

            return NoContent();
        }
    }
}
