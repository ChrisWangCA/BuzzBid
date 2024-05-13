using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using BuzzBid.ViewModels;
using System.Linq.Expressions;
using System.Data;
using BuzzBid.Models;
using System.Security.Claims;

namespace BuzzBid.Pages;


public class ViewRatingsModel : PageModel
{
    public ViewRatings Item { get; set; }

    public List<LoadCommentsModel> RatingTexts {get; set;}

    public string UserName { get; set; }

    public ViewRatingsModel(UserManager userManager)
    {
        this.Item = new ViewRatings();
        _userManager = userManager;
        RateItems = new List<RateItemModel>();
        RatingTexts = new List<LoadCommentsModel>();
    
    }
    private readonly UserManager _userManager;

    public bool IsAdmin { get; set; }

    public List<RateItemModel> RateItems { get; set; }


    public void OnGet()
{
    var search = HttpContext.Request.Query;
    IsAdmin = _userManager.IsAdminUser();
    UserName = User.FindFirstValue(ClaimTypes.NameIdentifier);


    if (search.TryGetValue("id", out StringValues str_itemId) && !string.IsNullOrEmpty(str_itemId))
    {
        // Initialize your DBService
        DBService svc = new DBService();

        // WHERE Item.ItemId = '{str_itemId}'

        
        var sql_averageRating = $@"
            SELECT Rating.Stars
            FROM Item
            JOIN Rating ON Rating.ItemId = Item.ItemId
            WHERE Item.ItemId = '{str_itemId}'"; 

        
        var ds_average = svc.ExecuteSql(sql_averageRating);
            


        if (ds_average != null && ds_average.Tables[0].Rows.Count > 0)
        {
            var avgRow = ds_average.Tables[0].Rows[0];
            Item.Stars = Convert.ToInt32(avgRow[0]);
            //Item.Text = avgRow[0].ToString();
            //foreach (DataRow row in ds_average.Tables[0].Rows)
            //{
            //    this.RatingTexts.Add(new ViewRatings(row["Text"].ToString()));
            //}
        }
        
        var sql = $@"
            SELECT Item.ItemId, Item.ItemName, Rating.RateTime, Rating.Text, Item.Winner
            FROM Item 
            LEFT JOIN Rating ON Rating.ItemId = Item.ItemId
            JOIN Category ON Category.CategoryId = Item.CategoryId
            WHERE Item.ItemId = '{str_itemId}'";

        
        var ds_details = svc.ExecuteSql(sql);

        
        if (ds_details.Tables[0].Rows.Count > 0)
        {
            var detailRow = ds_details.Tables[0].Rows[0];
            Item.ItemId = detailRow["ItemId"].ToString();
            Item.ItemName = detailRow["ItemName"].ToString();
            Item.RateTime = detailRow["RateTime"].ToString();
            Item.Text = detailRow["Text"].ToString();
            Item.Winner = detailRow["Winner"].ToString();
        }

        var sql2 = @"SELECT ItemName , ROUND(AVG(CAST(Stars AS FLOAT)), 1), Count(Stars)
			FROM Rating INNER JOIN Item ON Rating.ItemId = Item.ItemId
			GROUP BY ItemName
			ORDER BY ROUND(AVG(CAST(Stars AS FLOAT)), 1)";

			DBService svc2 = new DBService();
			var ds = svc2.ExecuteSql(sql2);
			DataRowCollection categoryTable = ds.Tables[0].Rows;

			foreach (DataRow row in categoryTable)
			{
				RateItemModel rateItemModel = new RateItemModel();
				rateItemModel.ItemName = row[0].ToString();
				rateItemModel.AveStars = row[1].ToString();
				rateItemModel.totalStars = row[2].ToString();
                
				RateItems.Add(rateItemModel);
			}

        var sql3 = @"SELECT Item.ItemId, Item.ItemName, MAX(Text) AS MaxText, MIN(RateTime), MAX(Stars), MAX(item.Winner)
FROM Rating
INNER JOIN Item ON Rating.ItemId = Item.ItemId
GROUP BY Item.ItemName, Item.ItemId
ORDER BY MAX(RateTime) DESC";

			DBService svc3 = new DBService();
			var ds3 = svc3.ExecuteSql(sql3);
			DataRowCollection comments = ds3.Tables[0].Rows;

			foreach (DataRow row in comments)
			{
				LoadCommentsModel loadCommentsModel = new LoadCommentsModel();
				loadCommentsModel.ItemId = row[0].ToString();
				loadCommentsModel.ItemName = row[1].ToString();
				loadCommentsModel.CommentsText = row[2].ToString();
                loadCommentsModel.RatedTime = row[3].ToString();
                loadCommentsModel.Stars = Convert.ToInt32(row[4]);
                loadCommentsModel.RatedBy = row[5].ToString();                
                
				RatingTexts.Add(loadCommentsModel);
			}
    }
}
    
}