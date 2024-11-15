using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace JewelryAuction.Services.Services
{
    public class TransactionServices: ITransactionServices
    {
        private readonly ITransactionRepository _repository;
        private readonly IWalletRepository _walletRepository;
        private readonly  IVnPayServices _vpnPayServices;


        public TransactionServices(ITransactionRepository repository, IWalletRepository walletRepository, IVnPayServices vnPayServices)
        {
            _repository = repository;
            _walletRepository = walletRepository;
            _vpnPayServices = vnPayServices;
        }

        public async Task<List<Transactions>> GetAllTransaction()
        {
            return await _repository.GetAllTransaction();
        }

        public async Task<Transactions> GetTransactionsById(string transactionId)
        {
            return await _repository.GetTransactionById(transactionId);
        }

        public async Task<List<Transactions>> GetAllTransactionsNyWalletId(string walletId)
        {
            if (walletId == null) 
            {
                throw new ArgumentNullException("wallet not Found");
            }
            return await _repository.GetAllTransactionsByWalletId(walletId);
        }


        public async Task<List<Transactions>> GetAllTransactionsNyUserId(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("User not Found");
            }
            return await _repository.GetAllTransactionsByUserId(userId);
        }
        public async Task<string> AddTransaction(string walletId,TransactionDto transactions)
        {
            if (transactions == null)
            {
                return "Transaction not found";
            }
            var wallet = await _walletRepository.GetWalletsById(walletId);
            if (wallet == null)
            {
                return "Wallet not found";
            }
          
          
            var transaction = new Transactions()
            {
                ID = transactions.TransactionId,
                Amount = transactions.Amount,
                Status = "Completed",
                CreatedDate = transactions.CreatedDate,
                UpdatedDate = transactions.CreatedDate,
                Payment = transactions.Payment,
                WalletId = walletId,
            };
            await _repository.AddTransaction(transaction);
            return "Success";
        }     
    }
}
