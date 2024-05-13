using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using BuzzBid.ViewModels;
using System.Linq.Expressions;
using System.Data;
using BuzzBid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Data.SqlClient;

using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

public class DeleteCommentsModel : PageModel
{
    private string _connectionString;

    //public DeleteComments Item { get; set; } = new Item();

    public Item Item { get; set; } = new Item();

    public Rating Rating { get; set; } = new Rating();

    public DeleteCommentsModel(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Buzzbid") ?? throw new InvalidOperationException("Connection string 'Buzzbid' not found.");
    
    }


    [BindProperty(SupportsGet = true)]
    public int ItemId { get; set; }

    [BindProperty]
    public string Description { get; set; }

    public async Task<IActionResult> OnGetAsync(int itemId)
{
    Rating = new Rating(); // Ensure the Rating object is initialized if not done elsewhere
    Item = new Item();

    using (var connection = new SqlConnection(_connectionString))
    {
        await connection.OpenAsync();

        var command = new SqlCommand("SELECT Text, RateTime, Stars, COALESCE(Item.Winner, 'No Winner') AS Winner FROM Rating LEFT JOIN Item ON Rating.ItemId = Item.ItemId WHERE Rating.ItemId = @ItemId", connection);
        command.Parameters.AddWithValue("@ItemId", itemId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            if (!reader.HasRows)
            {
                return NotFound(); // No ratings found for the item
            }
            if (await reader.ReadAsync()) 
            {
              
                Description = reader.GetString(0); // Get Text

               
                Rating.RateTime = reader.GetDateTime(1); // Get RateTime
                Rating.Stars = reader.GetInt32(2); // Get Stars
                Item.Winner = reader.GetString(3);

            }
        }
    }

    ItemId = itemId; 
    return Page(); 
}

    public async Task<IActionResult> OnPostAsync()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            //var command = new SqlCommand("UPDATE Item SET Description = @Description WHERE ItemId = @ItemId", connection);
            var command = new SqlCommand("DELETE FROM dbo.[Rating] WHERE ItemId = @ItemId", connection);
            //command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@ItemId", ItemId);

            var result = await command.ExecuteNonQueryAsync();
            if (result == 0)
            {
                return NotFound();
            }
        }

        return RedirectToPage("./ItemForSale", new { id = ItemId });
    }


}