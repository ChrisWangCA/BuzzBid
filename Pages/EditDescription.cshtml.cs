
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using BuzzBid.Models;

public class EditDescriptionModel : PageModel
{
    private string _connectionString;

    public Item Item { get; set; } = new Item();

    public EditDescriptionModel(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Buzzbid") ?? throw new InvalidOperationException("Connection string 'Buzzbid' not found.");
    }


    [BindProperty(SupportsGet = true)]
    public int ItemId { get; set; }

    [BindProperty]
    public string Description { get; set; }

    public async Task<IActionResult> OnGetAsync(int itemId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("SELECT Description FROM Item WHERE ItemId = @ItemId", connection);
            command.Parameters.AddWithValue("@ItemId", itemId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                {
                    return NotFound();
                }
                while (await reader.ReadAsync())
                {
                    Description = reader.GetString(0);
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
            var command = new SqlCommand("UPDATE Item SET Description = @Description WHERE ItemId = @ItemId", connection);
            command.Parameters.AddWithValue("@Description", Description);
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
