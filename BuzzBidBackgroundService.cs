using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;

namespace BuzzBid
{
    public class BuzzBidBackgroundService : BackgroundService
    {
        private readonly ILogger<BuzzBidBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BuzzBidBackgroundService(ILogger<BuzzBidBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Update item when auction ends.");
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<BuzzBidContext>();

                        string updateQuery = @"
                            UPDATE Item 
                            SET Item.Winner = Bidding.BidBy, 
                            WinDate = DATEADD(DAY,Item.AuctionLength,Item.ListDate), 
                            SalesPrice = MaxBid.MaxBid 
                            FROM Item 
                            JOIN 
                            (SELECT MAX(BidAmount) MaxBid, BidItem FROM Bidding 
                            GROUP BY BidItem) MaxBid 
                            ON Item.ItemId = MaxBid.BidItem 
                            JOIN 
                            Bidding ON Bidding.BidAmount = MaxBid.MaxBid AND Bidding.BidItem = MaxBid.BidItem 
                            WHERE GETDATE() > DATEADD(DAY, Item.AuctionLength, ITem.ListDate) AND CancelDate IS NULL AND Winner IS NULL 
                        ";
                        await dbContext.Database.ExecuteSqlRawAsync(updateQuery, cancellationToken: stoppingToken);
                        
                        _logger.LogInformation("Item is updated successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating SQL Server.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
