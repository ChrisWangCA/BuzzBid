using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BuzzBid.ViewModels;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BuzzBid.Pages;

[Authorize]
public class AuctionResultModel : PageModel
{
    public List<AuctionResult> AuctionResults { get; set; }

    public AuctionResultModel()
    {
        this.AuctionResults = new List<AuctionResult>();
    }

    public void OnGet()
    {
        var sql = @"
            SELECT ItemId, ItemName, SalesPrice, Winner, AuctionEnds
            FROM 
            (
                SELECT ItemId, ItemName, ISNULL(CONVERT(NVARCHAR(20),SalesPrice),'') AS SalesPrice, ISNULL(Winner, '') AS Winner, 
                        
                (SELECT MIN(v) FROM (VALUES (CancelDate),(WinDate),(ListDate)) AS value(v)) AS AuctionEnds
                FROM Item
            ) T
            WHERE AuctionEnds < GETDATE()
            ORDER BY AuctionEnds DESC
            ";

        DBService svc = new DBService();
        var ds = svc.ExecuteSql(sql);

        foreach (DataRow row in ds.Tables[0].Rows)
        {

            AuctionResults.Add(new AuctionResult(
                row[0].ToString(),
                row[1].ToString(),
                row[2].ToString(),
                row[3].ToString(),
                row[4].ToString())
            );
        }
    }
}

