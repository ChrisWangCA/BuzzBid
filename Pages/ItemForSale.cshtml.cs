using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BuzzBid.Models;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

public class ItemForSaleModel : PageModel
{
    private readonly BuzzBidContext _context;
    public Item? Item { get; set; }
    public List<Bidding>? Bids { get; set; }

    public string Message { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string UserName { get; set; }
    private readonly string _connectionString;
    public string CategoryDescription { get; set; }

    
    private readonly ILogger<ItemForSaleModel> _logger;




    public ItemForSaleModel(ILogger<ItemForSaleModel> logger, IConfiguration configuration, BuzzBidContext context)
    {
        _context = context;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("BuzzBid") ?? throw new InvalidOperationException("Connection string 'BuzzBidConnection' not found.");
    }


    public DateTime? AuctionEnds { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {

        Bids = new List<Bidding>();
        if (!id.HasValue)
        {
            return NotFound();
        }

        if (User.Identity.IsAuthenticated)
        {
            _logger.LogInformation($"Current user: {User.Identity.Name}");
        }
        else
        {
            _logger.LogInformation("No user is currently authenticated.");
        }

        var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        UserName = userNameClaim?.Value;

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var itemQuery = @"
            SELECT Item.*, Category.Description AS CategoryDescription
            FROM Item
            LEFT JOIN Category ON Item.CategoryId = Category.CategoryId
            WHERE ItemId = @id";

            using (var command = new SqlCommand(itemQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.Read())
                    {
                        return NotFound($"Unable to load item with ID '{id}'.");
                    }

                    Item = new Item
                    {

                    };

                    CategoryDescription = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? null : reader.GetString(reader.GetOrdinal("CategoryDescription"));
                }


                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.Read())
                    {
                        return NotFound($"Unable to load item with ID '{id}'.");
                    }

                    Item = new Item
                    {
                        ItemId = reader.GetInt32(reader.GetOrdinal("ItemId")),
                        ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                        ListBy = reader.GetString(reader.GetOrdinal("ListBy")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        GetItNowPrice = reader.IsDBNull(reader.GetOrdinal("GetItNowPrice")) ? 0m : reader.GetDecimal(reader.GetOrdinal("GetItNowPrice")),
                        AuctionLength = reader.GetInt32(reader.GetOrdinal("AuctionLength")),
                        Condition = reader.GetInt32(reader.GetOrdinal("Condition")),
                        Returnable = reader.GetBoolean(reader.GetOrdinal("Returnable")),
                        ListDate = reader.GetDateTime(reader.GetOrdinal("ListDate")),
                        MinSalesPrice = reader.IsDBNull(reader.GetOrdinal("MinSalesPrice")) ? 0m : reader.GetDecimal(reader.GetOrdinal("MinSalesPrice")),
                        StartBid = reader.IsDBNull(reader.GetOrdinal("StartBid")) ? 0m : reader.GetDecimal(reader.GetOrdinal("StartBid"))
                    };


                    AuctionEnds = Item.ListDate;
                }
            }

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }



            Bids = new List<Bidding>();
            var bidsQuery = "SELECT * FROM Bidding WHERE BidItem = @id ORDER BY BidTime DESC";
            using (var command = new SqlCommand(bidsQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        Bids.Add(new Bidding
                        {
                            BidId = reader.GetInt32(reader.GetOrdinal("BidId")),
                            BidItem = reader.GetInt32(reader.GetOrdinal("BidItem")),
                            BidBy = reader.GetString(reader.GetOrdinal("BidBy")),
                            BidTime = reader.GetDateTime(reader.GetOrdinal("BidTime")),
                            BidAmount = reader.GetDecimal(reader.GetOrdinal("BidAmount")),
                        });
                    }
                }
            }
        }

        return Page();
    }



    public async Task<IActionResult> OnPostEditDescriptionAsync(int itemId, string description)
    {
        var item = await _context.Items.FindAsync(itemId);


        if (item == null)
        {
            return NotFound();
        }

        item.Description = description;
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id = itemId });
    }




    public async Task<IActionResult> OnPostCancelItemAsync(int itemId, string cancelReason)
    {
        _logger.LogInformation("Attempting to cancel item with ID {ItemId}", itemId);

        if (string.IsNullOrWhiteSpace(cancelReason))
        {
            _logger.LogWarning("Cancellation attempt failed for item {ItemId}: Cancel reason is required.", itemId);
            ModelState.AddModelError("cancelReason", "Cancellation reason is required.");
            return Page();
        }

        if (!User.IsInRole("admin"))
        {
            _logger.LogWarning("User {UserName} attempted to cancel item {ItemId} without admin role.", User.Identity?.Name, itemId);
            TempData["ErrorMessage"] = "Only admins can cancel items.";
            return RedirectToPage(new { id = itemId });
        }

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cancelItemQuery = @"
                UPDATE Item
                SET CancelReason = @CancelReason, CancelDate = GETDATE()
                WHERE ItemId = @ItemId AND CancelDate IS NULL";

                using (var command = new SqlCommand(cancelItemQuery, connection))
                {
                    command.Parameters.AddWithValue("@CancelReason", cancelReason);
                    command.Parameters.AddWithValue("@ItemId", itemId);

                    var result = await command.ExecuteNonQueryAsync();
                    if (result == 0)
                    {
                        _logger.LogWarning("Item {ItemId} could not be found or has already been cancelled.", itemId);
                        Message = "Item could not be found or has already been cancelled.";
                        return RedirectToPage("/SearchItems");
                    }
                }
            }

            _logger.LogInformation("Item {ItemId} was successfully cancelled.", itemId);
            return RedirectToPage("/SearchItems");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while attempting to cancel item {ItemId}.", itemId);
            Message = "An error occurred while processing your request. Please try again.";
            return RedirectToPage();
        }
    }



    public async Task<IActionResult> OnPostBidAsync(int itemId, decimal bidAmount)
    {



        var item = await _context.Items.Include(i => i.Biddings).FirstOrDefaultAsync(i => i.ItemId == itemId);

        var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var userName = userNameClaim?.Value;

        _logger.LogInformation("Item id is {ItemId}", item.ItemId);

        if (item == null)
        {
            return NotFound("The item does not exist.");
        }


        if (item.ListBy == userName)
        {
            TempData["ErrorMessage"] = "You cannot bid on your own items.";
            return RedirectToPage(new { id = itemId });
        }


        var highestBid = item.Biddings.Any() ? item.Biddings.Max(b => b.BidAmount) : item.StartBid;


        if (bidAmount <= highestBid)
        {
            TempData["ErrorMessage"] = "Your bid must be higher than the current highest bid.";
            return RedirectToPage(new { id = itemId });
        }
        if (item.GetItNowPrice > 0 && bidAmount >= item.GetItNowPrice)
        {
            TempData["ErrorMessage"] = "Your bid exceeds the Get It Now price. Please use the Get It Now option.";
            return RedirectToPage(new { id = itemId });
        }


        if (string.IsNullOrEmpty(User.Identity?.Name))
        {
            TempData["ErrorMessage"] = "You must be logged in to place a bid.";
            return RedirectToPage(new { id = itemId });
        }



        var newBid = new Bidding
        {
            BidItem = itemId,
            BidBy = userName,
            BidAmount = bidAmount,
            BidTime = DateTime.UtcNow
        };

        _context.Biddings.Add(newBid);
        await _context.SaveChangesAsync();


        return RedirectToPage("/ItemResult", new { id = itemId });
    }



    public async Task<IActionResult> OnPostGetItNowAsync(int itemId)
    {

        if (!User.Identity.IsAuthenticated)
        {
            TempData["ErrorMessage"] = "You must be logged in to use Get It Now.";
            return RedirectToPage("/Login");
        }
        var userNameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        var userName = userNameClaim?.Value;
        // var userName = User.Identity.Name;

        _logger.LogInformation("User name from Identity: {UserName}", userName);



        var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == itemId);


        if (item == null)
        {
            TempData["ErrorMessage"] = "Item does not exist or cannot be purchased now.";
            return RedirectToPage(new { id = itemId });
        }
        if (item.ListBy == userName)
        {
            TempData["ErrorMessage"] = "You cannot buy your own items.";
            return RedirectToPage(new { id = itemId });
        }


        try
        {
            item.Winner = userName;
            item.SalesPrice = item.GetItNowPrice;
            item.WinDate = DateTime.UtcNow;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToPage("/AuctionResult", new { id = itemId });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency error occurred while updating the item with ID {ItemId}.", itemId);
            TempData["ErrorMessage"] = "Concurrency error occurred. Please try again.";
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database update error occurred while updating the item with ID {ItemId}.", itemId);
            TempData["ErrorMessage"] = "An error occurred while updating the item. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating the item with ID {ItemId}.", itemId);
            TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
        }

        return RedirectToPage(new { id = itemId });

    }


}