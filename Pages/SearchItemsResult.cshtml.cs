using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using BuzzBid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BuzzBid.ViewModels;
using System.Data;
using Microsoft.AspNetCore.Authorization;


namespace BuzzBid.Pages;

[Authorize]
public class SearchItemsResultModel : PageModel
{
    public List<SearchItemsResult> SearchResult { get; set; }

    public SearchItemsResultModel()
    {
        this.SearchResult = new List<SearchItemsResult>();
    }

    public void OnGet()
    {
        var search = HttpContext.Request.Query;

        StringValues str_keyword = string.Empty;
        search.TryGetValue("keyword", out str_keyword);
        StringValues str_category = string.Empty;;
        search.TryGetValue("category", out str_category);
        str_category = str_category == string.Empty?"0": str_category;
        StringValues str_minp = string.Empty;
        search.TryGetValue("minp", out str_minp);
        str_minp = str_minp == string.Empty?"0": str_minp;
        StringValues str_maxp = string.Empty;
        search.TryGetValue("maxp", out str_maxp);
        str_maxp = str_maxp == string.Empty?"0": str_maxp;
        StringValues str_condition = string.Empty;
        search.TryGetValue("condition", out str_condition);

        var sql = @"
            SELECT ItemId, ItemName, T.MaxBidAmount as CurrentBid, Bidding.BidBy as HighBidder, GetItNowPrice, ListDate as AuctionEnds
            FROM Item
            LEFT JOIN (SELECT BidItem, MAX(BidAmount) MaxBidAmount FROM Bidding  GROUP BY BidItem) T on Item.ItemId = T.BidItem
            LEFT JOIN Bidding on T.BidItem = Bidding.BidItem and T.MaxBidAmount = Bidding.BidAmount
            JOIN Category on Category.CategoryId = Item.CategoryId " +
            $"WHERE CancelDate is NULL AND WinDate is NULL AND ItemName like '%{str_keyword}%'" +
            $"AND ({str_category} = 0 OR ({str_category} <> 0 AND Item.CategoryId = {str_category}))" +
            $"AND ({str_minp} = 0 OR ({str_minp} > 0 AND ((Bidding.BidId IS NOT NULL AND T.MaxBidAmount > {str_minp}) OR (Bidding.BidId IS NULL AND Item.StartBid > {str_minp}))))" +
            $"AND ({str_maxp} = 0 OR ({str_maxp} > 0 AND ((Bidding.BidId IS NOT NULL AND T.MaxBidAmount < {str_maxp}) OR (Bidding.BidId IS NULL AND Item.StartBid < {str_maxp}))))" + 
            $"AND ({str_condition} = 0 OR ({str_condition} > 0 AND Item.Condition >= {str_condition}))" +
            $"AND (ListDate>GETDATE())" +
            @"ORDER BY DATEADD(day, AuctionLength, ListDate)";

        DBService svc = new DBService();
        var ds = svc.ExecuteSql(sql);

        foreach (DataRow row in ds.Tables[0].Rows)
        {

            SearchResult.Add(new SearchItemsResult(
                row[0].ToString(),
                row[1].ToString(),
                row[2].ToString(),
                row[3].ToString(),
                row[4].ToString(),
                row[5].ToString())
            );
        }
    }
}

