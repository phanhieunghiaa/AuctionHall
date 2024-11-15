using Jewelry_auction_system.Data;
using JewelryAuction.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Models;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Services.Services.Interfaces;

namespace JewelryAuction.Services.Services.Background
{
    public class AuctionCheckService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionCheckService> _logger;

        public AuctionCheckService(IServiceScopeFactory scopeFactory, ILogger<AuctionCheckService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        #region runtime
        /*protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextMidnight = DateTime.UtcNow.Date.AddDays(1); // Next midnight in UTC

                var timeUntilMidnight = nextMidnight - now;
                //_logger.LogInformation($"Initial delay until next run at midnight: {timeUntilMidnight.TotalMinutes} minutes.");
                //_logger.LogInformation($"Next mid night: {timeUntilMidnight.TotalHours} hours.");
                //_logger.LogInformation($"Next mid night: {nextMidnight}.");

                // Wait until the next 0h (midnight)
                await Task.Delay(timeUntilMidnight, stoppingToken);

                // Execute the task at midnight
                await CheckAuctionsStartDate(stoppingToken);

                // Wait for 24 hours before the next run
                while (!stoppingToken.IsCancellationRequested)
                {
                    var nextRunTime = DateTime.UtcNow.Date.AddDays(1); // Next midnight
                    var delay = nextRunTime - DateTime.UtcNow;

                    _logger.LogInformation($"Next run scheduled at: {nextRunTime} UTC");
                    await Task.Delay(delay, stoppingToken);
                    await CheckAuctionsStartDate(stoppingToken);
                    await CheckAuctionsEndDate(stoppingToken);
                }
            }
        }*/
        #endregion


        #region runtime 5 sec
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int count = 1;
                var now = DateTime.UtcNow;
                var nextMidnight = DateTime.UtcNow.AddSeconds(5); // Chạy sau 5 giây thay vì nửa đêm

                var timeUntilMidnight = nextMidnight - now;
                _logger.LogInformation($"Initial delay until next run in 5 seconds.");

                // Wait for 30 seconds
                await Task.Delay(timeUntilMidnight, stoppingToken);

                // Execute the task
                await CheckAuctionsStartDate(stoppingToken);

                // Wait for 30 seconds before the next run
                while (!stoppingToken.IsCancellationRequested)
                {
                    var nextRunTime = DateTime.UtcNow.AddSeconds(5); // Chạy sau 5 giây
                    var delay = nextRunTime - DateTime.UtcNow;
                    count++;

                    _logger.LogInformation($"Next run scheduled in 5 seconds.");
                    _logger.LogInformation($"Repeated {count} time.");
                    await Task.Delay(delay, stoppingToken);
                    await CheckAuctionsStartDate(stoppingToken);
                    await CheckAuctionsEndDate(stoppingToken);
                }
            }
        }

        #endregion

        #region change auction status
        //change status when meet start date
        private async Task CheckAuctionsStartDate(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var auctionEntity = scope.ServiceProvider.GetRequiredService<IAuctionsRepository>();
                    var auctions = await auctionEntity.GetAllAuctions("");
                    if (auctions == null)
                    {
                        _logger.LogInformation($"There is no auction");
                    }
                    else
                    {
                        foreach (var auction in auctions)
                        {
                            if (auction.StartDate <= DateTime.Now && auction.Status == AuctionStatus.Pending)
                            {
                                auction.Status = AuctionStatus.Active;
                                await auctionEntity.UpdateAuctions(auction);
                                _logger.LogInformation($"Auction {auction.ID} has been change to Active.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking auctions.");
            }

        }

        //change status when meet end date
        private async Task CheckAuctionsEndDate(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var auctionEntity = scope.ServiceProvider.GetRequiredService<IAuctionsRepository>();
                    var auctions = await auctionEntity.GetAllAuctions("");
                    if (auctions == null)
                    {
                        _logger.LogInformation($"There is no auction");
                    }
                    else
                    {
                        foreach (var auction in auctions)
                        {
                            if (auction.EndDate <= DateTime.Now && auction.Status == AuctionStatus.Active)
                            {
                                var bidRepo = scope.ServiceProvider.GetRequiredService<IBidRepository>();
                                var bid = await bidRepo.GetHighestBidByAuctionId(auction.ID, BidStatus.Active);
                                if (bid == null)
                                {
                                    auction.Status = AuctionStatus.None;
                                    await auctionEntity.UpdateAuctions(auction);
                                    _logger.LogInformation($"Auction {auction.ID} has been change to None.");
                                }
                                else
                                {
                                    var walletRepo = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
                                    var Email = scope.ServiceProvider.GetRequiredService<IEmailServices>();
                                    var wallet = await walletRepo.GetWalletByUserIdAsync(bid.UserId);
                                    wallet.Balance -= bid.Value;
                                    await walletRepo.UpdateWallet(wallet);
                                    auction.Status = AuctionStatus.Done;
                                    auction.FinalPrice = auction.StartingPrice;
                                    await auctionEntity.UpdateAuctions(auction);
                                    _logger.LogInformation($"Auction {auction.ID} has been change to Done.");

                                    // Add transaction
                                    var transactionRepo = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                                    var transaction = new Transactions
                                    {
                                        ID = Guid.NewGuid().ToString(), // Assuming ID is a GUID
                                        Amount = bid.Value,
                                        Status = "Completed",
                                        CreatedDate = DateTime.Now,
                                        Payment = "Bid Transaction",
                                        WalletId = wallet.ID
                                    };

                                    await transactionRepo.AddTransaction(transaction);
                                    await Email.SendEmail_WinBid(bid.ID);
                                    _logger.LogInformation($"Auction {auction.ID} has been changed to Done and transaction added.");



                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking auctions.");
            }

        }
        #endregion



    }
}
