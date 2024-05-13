using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using BuzzBid.ViewModels;
using BuzzBid.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BuzzBid.Pages;

[Authorize]
public class ItemResultModel : PageModel
{
    public ItemDescription Item { get; set; }
    public List<BiddingHistory> Biddings { get; set; }

    public ItemResultModel()
    {
        this.Item = new ItemDescription();
        this.Biddings = new List<BiddingHistory>();
    }

    public void OnGet()
    {
        var search = HttpContext.Request.Query;

        StringValues str_itemId = string.Empty;
        search.TryGetValue("id", out str_itemId);

        var sql = @"
            SELECT ItemId, ItemName, ItemDesc, CateDesc, Condition, Returnable, GetItNowPrice, AuctionEnds, Status

            FROM
            (
            SELECT Item.ItemId, Item.ItemName,Item.Description ItemDesc,Category.Description CateDesc,Item.Condition,Item.Returnable,Item.GetItNowPrice, 
            (SELECT MIN(v) FROM (VALUES (CancelDate),(WinDate),(DATEADD(day, AuctionLength, ListDate))) AS value(v)) AS AuctionEnds,
            CASE 
                WHEN Winner IS NOT NULL THEN 'WINNER'
                WHEN CancelDate IS NOT NULL THEN 'CANCEL'
                ELSE 'NO_WINNER'
            END AS Status
            FROM Item 
            JOIN Category on Category.CategoryId = Item.CategoryId 
            ) T " +
        $"WHERE T.ItemId = {str_itemId} AND T.AuctionEnds < GETDATE()";


        DBService svc = new DBService();
        var ds = svc.ExecuteSql(sql);


        if (ds.Tables[0].Rows.Count > 0)
        {
            var item = ds.Tables[0].Rows[0];
            Item.ItemId = item[0].ToString();
            Item.ItemName = item[1].ToString();
            Item.Description = item[2].ToString();
            Item.Category = item[3].ToString();
            Item.Condition = item[4].ToString();
            Item.ReturnsAccepted = item[5].ToString();
            Item.GetItNowPrice = item[6].ToString();
            Item.AuctionEnds = item[7].ToString();
            Item.Status = item[8].ToString();
        }

        sql = @"SELECT * 
                FROM 
                ("+
                $"SELECT 'Cancelled' AS 'Bid Amount', CancelDate AS 'Time of Bid', 'Administrator' AS 'Username' FROM Item WHERE ItemId = {str_itemId} AND CancelDate IS NOT NULL "+
                "UNION "+
                $"SELECT  CONVERT(NVARCHAR(20),BidAmount) AS 'Bid Amount', BidTime AS 'Time of Bid', BidBy AS 'Username' from Bidding WHERE BidItem = {str_itemId} "+
                $") T ORDER BY [Time of Bid] DESC";

        ds = svc.ExecuteSql(sql);

        foreach (DataRow row in ds.Tables[0].Rows)
        {
            Biddings.Add(new BiddingHistory(row[0].ToString(), row[1].ToString(), row[2].ToString()));
        }
    }
}
