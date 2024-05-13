using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using BuzzBid.ViewModels;
using System.Linq.Expressions;

namespace BuzzBid.Pages;


public class ItemDescriptionModel : PageModel
{
    public ItemDescription Item { get; set; }

    public ItemDescriptionModel()
    {
        this.Item = new ItemDescription();
    }

    public void OnGet()
    {
        var search = HttpContext.Request.Query;

        StringValues str_itemId = string.Empty;
        search.TryGetValue("id", out str_itemId);


        var sql = @"
            SELECT Item.ItemId, Item.ItemName,Item.Description,Category.Description,Item.Condition,Item.Returnable,Item.GetItNowPrice, DATEADD(day, AuctionLength, ListDate) 'AuctionEnd' 
            FROM Item 
            JOIN Category on Category.CategoryId = Item.CategoryId " +
        $"WHERE Item.ItemId = {str_itemId}";


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
        }

    }
}

